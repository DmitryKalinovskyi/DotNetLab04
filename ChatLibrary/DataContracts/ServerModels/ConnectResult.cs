using ChatLibrary.DataContracts.Common;
using System.Runtime.Serialization;

namespace ChatLibrary.DataContracts.ServerModels
{
    [DataContract]
    public class ConnectResult
    {
        [DataMember]
        public ConnectContract Contract { get; set; }

        [DataMember]
        public ClientModel ClientModel { get; set; }

        public ConnectResult(ConnectContract contract, ClientModel client) 
        { 
            Contract = contract;
            ClientModel = client;
        }
    }
}
