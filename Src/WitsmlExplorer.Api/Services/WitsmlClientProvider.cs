using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using Witsml;
using Witsml.Data;

using WitsmlExplorer.Api.Configuration;

namespace WitsmlExplorer.Api.Services
{
    public interface IWitsmlClientProvider
    {
        IWitsmlClient GetClient();
        IWitsmlClient GetSourceClient();
    }

    public class WitsmlClientProvider : IWitsmlClientProvider
    {
        public const string WitsmlTargetServerHeader = "WitsmlTargetServer";
        public const string WitsmlSourceServerHeader = "WitsmlSourceServer";

        private readonly ServerCredentials _targetCreds;
        private readonly ServerCredentials _sourceCreds;
        private readonly WitsmlClient _witsmlClient;
        private readonly WitsmlClient _witsmlSourceClient;
        private readonly WitsmlClientCapabilities _clientCapabilities;

        public WitsmlClientProvider(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ICredentialsService credentialsService, IOptions<WitsmlClientCapabilities> witsmlClientCapabilities)
        {
            if (httpContextAccessor.HttpContext?.Request.Headers[WitsmlTargetServerHeader].Count == 0)
            {
                return;
            }
            _clientCapabilities = witsmlClientCapabilities.Value;
            bool logQueries = StringHelpers.ToBoolean(configuration[ConfigConstants.LogQueries]);

            StringValues? authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
            StringValues? targetServerHeader = httpContextAccessor.HttpContext?.Request.Headers[WitsmlTargetServerHeader];
            StringValues? sourceServerHeader = httpContextAccessor.HttpContext?.Request.Headers[WitsmlSourceServerHeader];

            // Use system creds by role from Bearer token
            if (authorizationHeader?.Count > 0)
            {
                string bearerToken = authorizationHeader.ToString().Split()[1];

                Task<ServerCredentials> targetCredsTask = credentialsService.GetCredsWithToken(bearerToken, targetServerHeader.ToString());
                Task<ServerCredentials> sourceCredsTask = credentialsService.GetCredsWithToken(bearerToken, sourceServerHeader.ToString());
                Task.WaitAll(targetCredsTask, sourceCredsTask);
                _targetCreds = targetCredsTask.Result;
                _sourceCreds = sourceCredsTask.Result;

            }
            // Use b64 encoded Basic creds
            else if (authorizationHeader?.Count == 0 && targetServerHeader?.Count > 0)
            {
                _targetCreds = credentialsService.GetBasicCredsFromHeader(targetServerHeader.ToString());
                _sourceCreds = credentialsService.GetBasicCredsFromHeader(sourceServerHeader.ToString());
            }

            _witsmlClient = !_targetCreds.IsNullOrEmpty() ? new WitsmlClient(_targetCreds.Host, _targetCreds.UserId, _targetCreds.Password, _clientCapabilities, null, logQueries) : null;
            _witsmlSourceClient = !_sourceCreds.IsNullOrEmpty() ? new WitsmlClient(_sourceCreds.Host, _sourceCreds.UserId, _sourceCreds.Password, _clientCapabilities, null, logQueries) : null;
        }

        internal WitsmlClientProvider(IConfiguration configuration)
        {
            (string serverUrl, string username, string password) = GetCredentialsFromConfiguration(configuration);
            _witsmlClient = new WitsmlClient(serverUrl, username, password, new WitsmlClientCapabilities(), null, true);
        }

        private static (string, string, string) GetCredentialsFromConfiguration(IConfiguration configuration)
        {
            string serverUrl = configuration["Witsml:Host"];
            string username = configuration["Witsml:Username"];
            string password = configuration["Witsml:Password"];

            return (serverUrl, username, password);
        }

        public IWitsmlClient GetClient()
        {
            return _witsmlClient;
        }

        public IWitsmlClient GetSourceClient()
        {
            return _witsmlSourceClient;
        }
    }
}
