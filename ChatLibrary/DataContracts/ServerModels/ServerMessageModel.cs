using System;
using System.Runtime.Serialization;

namespace ChatLibrary.DataContracts.ServerModels
{
    [DataContract]
    public class ServerMessageModel
    {
        [DataMember]
        public string SenderName { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public Guid ChatId { get; set; }

        public ServerMessageModel(string senderName, string message, Guid chatId)
        {
            SenderName = senderName;
            Message = message;
            DateTime = DateTime.Now;
            ChatId = chatId;
        }
    }
}
