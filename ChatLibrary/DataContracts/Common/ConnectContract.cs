using System;
using System.Runtime.Serialization;

namespace ChatLibrary.DataContracts.Common
{
    [DataContract]
    public class ConnectContract
    {
        [DataMember]
        public Guid ContractID { get; set; }

        public ConnectContract()
        {
            ContractID = Guid.NewGuid();
        }
    }
}
