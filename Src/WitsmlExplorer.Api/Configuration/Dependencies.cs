using System;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NetCore.AutoRegisterDi;

using Serilog;

using WitsmlExplorer.Api.Models;
using WitsmlExplorer.Api.Repositories;
using WitsmlExplorer.Api.Services;
using WitsmlExplorer.Api.Workers;
using WitsmlExplorer.Api.Workers.Copy;

namespace WitsmlExplorer.Api.Configuration
{
    public static class Dependencies
    {
        public static void ConfigureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(Program)))
                .IgnoreThisInterface<ICopyLogDataWorker>()
                .IgnoreThisInterface<ICredentials>()
                .IgnoreThisInterface<ICredentialsService>()
                .IgnoreThisInterface<IJobCache>()
                .IgnoreThisInterface<IJobQueue>()
                .IgnoreThisInterface<IWitsmlSystemCredentials>()
                .IgnoreThisInterface<IWitsmlClientProvider>()
                .IgnoreThisInterface<ICredentialsCache>()
                .AsPublicImplementedInterfaces();
            AddRepository<Server, Guid>(services, configuration);
            services.AddSingleton<ICredentialsService, CredentialsService>();
            services.AddSingleton<IJobCache, JobCache>();
            services.AddSingleton<IJobQueue, JobQueue>();
            services.AddSingleton<IWitsmlSystemCredentials, WitsmlSystemCredentials>();
            services.AddScoped<IWitsmlClientProvider, WitsmlClientProvider>();
            services.AddSingleton<ICredentialsCache, CredentialsCache>();
        }

        private static void AddRepository<TDocument, T>(IServiceCollection services, IConfiguration configuration) where TDocument : DbDocument<T>
        {
            if (!string.IsNullOrEmpty(configuration["MongoDb:Name"]))
            {
                Log.Information("Detected database config for MongoDB");
                services.AddSingleton<IDocumentRepository<TDocument, T>, MongoRepository<TDocument, T>>();
            }
            else if (!string.IsNullOrEmpty(configuration["CosmosDb:Name"]))
            {
                Log.Information("Detected database config for CosmosDB");
                services.AddSingleton<IDocumentRepository<TDocument, T>, CosmosRepository<TDocument, T>>();
            }
            else
            {
                Log.Error("Did not detect any configuration for database");
                return;
            }

        }

        public static void InitializeRepository(this IApplicationBuilder app)
        {
            IDocumentRepository<Server, Guid> repository = app.ApplicationServices.GetService<IDocumentRepository<Server, Guid>>();
            repository?.InitClientAsync().GetAwaiter().GetResult();
        }
    }
}
