using System.Runtime.Serialization;

namespace ChatLibrary.DataContracts.Common
{
    [DataContract]
    public class GroupModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        public GroupModel()
        {
            Name = string.Empty;
            Description = string.Empty; 
        }

        public GroupModel(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
