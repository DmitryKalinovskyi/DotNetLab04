using System;
using System.Runtime.Serialization;

namespace ChatLibrary.DataContracts.ClientModels
{
    [DataContract]
    public class ClientMessageModel
    {
        [DataMember]
        public Guid ChatId { get; set; }

        [DataMember]
        public string Message { get; set; }

        public ClientMessageModel(Guid chatId, string message)
        {
            ChatId = chatId;
            Message = message;
        }
    }
}
