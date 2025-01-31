using System.Collections.Generic;
using System.Linq;

using Witsml.Data;
using Witsml.Extensions;

using WitsmlExplorer.Api.Models;

namespace WitsmlExplorer.Api.Query
{
    public class MessageQueries
    {
        public static WitsmlMessages GetMessageById(string wellUid, string wellboreUid, string messageUid)
        {
            return new WitsmlMessages
            {
                Messages = new WitsmlMessage
                {
                    UidWellbore = wellboreUid,
                    UidWell = wellUid,
                    Uid = messageUid
                }.AsSingletonList()
            };
        }

        public static WitsmlMessages GetMessageByWellbore(string wellUid, string wellboreUid)
        {
            return new WitsmlMessages
            {
                Messages = new WitsmlMessage
                {
                    UidWellbore = wellboreUid,
                    UidWell = wellUid,
                    NameWellbore = "",
                    NameWell = "",
                    Uid = "",
                    Name = "",
                    MessageText = "",
                    TypeMessage = "",
                    DTim = "",
                    CommonData = new WitsmlCommonData()
                    {
                        SourceName = "",
                        DTimCreation = "",
                        DTimLastChange = "",
                        Comments = ""
                    }
                }.AsSingletonList()
            };
        }

        public static IEnumerable<WitsmlMessage> DeleteMessageQuery(string wellUid, string wellboreUid, string[] messageUids)
        {
            return messageUids.Select((messageUid) =>
                new WitsmlMessage
                {
                    Uid = messageUid,
                    UidWell = wellUid,
                    UidWellbore = wellboreUid
                }
            );
        }

        public static WitsmlMessages CreateMessageObject(MessageObject messageObject)
        {
            return new WitsmlMessages
            {
                Messages = new WitsmlMessage
                {
                    UidWell = messageObject.WellUid,
                    NameWell = messageObject.WellName,
                    UidWellbore = messageObject.WellboreUid,
                    NameWellbore = messageObject.WellboreName,
                    Uid = messageObject.Uid,
                    Name = messageObject.Name,
                    MessageText = messageObject.MessageText
                }.AsSingletonList()
            };
        }
    }
}
