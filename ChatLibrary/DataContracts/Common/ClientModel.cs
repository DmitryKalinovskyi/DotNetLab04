using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ChatLibrary.DataContracts.Common
{
    [DataContract]
    public class ClientModel
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string Username { get; set; } = "";

        [DataMember]
        public List<Guid> Conversations { get; set; } 

        [IgnoreDataMember]
        public OperationContext Context { get; set; }

        [IgnoreDataMember]
        public ConnectContract ConnectContract { get; set; }

        private static int GetId()
        {
            return _idAutoIncrement++;
        }

        private static int _idAutoIncrement = 0;

        public ClientModel(string username, OperationContext context)
        {
            Id = GetId();
            Conversations = new List<Guid>();
            Username = username;
            Context = context;
        }

        public IChatServiceCallback GetCallback()
        {
            return Context.GetCallbackChannel<IChatServiceCallback>();
        }
    }
}
