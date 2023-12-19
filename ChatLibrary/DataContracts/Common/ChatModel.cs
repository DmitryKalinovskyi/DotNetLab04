using ChatLibrary.DataContracts.ServerModels;
using ChatLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChatLibrary.DataContracts.Common
{
    [DataContract]
    public class ChatModel
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public List<int> Members { get; set; }

        [DataMember]
        public List<ServerMessageModel> Messages { get; set; }

        [DataMember]
        public bool IsGroup { get; set; }

        [DataMember]
        public string Name { get; set; }    

        public ChatModel()
        {
            Id = Guid.NewGuid();
            IsGroup = false;
            Name = "";
            Messages = new List<ServerMessageModel>();
            Members = new List<int>();
        }

        public ChatModel(List<int> members)
        {
            Id = Guid.NewGuid();
            IsGroup = false;
            Name = "";
            Messages = new List<ServerMessageModel>();
            Members = members;
        }

        public void SendMessage(ServerMessageModel message)
        {
            Messages.Add(message);

            // notify all users
            foreach (var memberId in Members)
            {
                var member = ChatService.Instance.Clients[memberId];

                member.GetCallback().ReceiveMessage(member.ConnectContract, message);
            }
        }

        public static ChatModel CreateConversation(ClientModel a, ClientModel b)
        {
            var chat = new ChatModel(new List<int>() { a.Id, b.Id });

            // assign special id
            chat.Id = IdHelper.MungeTwoInt(a.Id, b.Id);
            a.Conversations.Add(chat.Id);
            b.Conversations.Add(chat.Id);

            chat.Messages.Add(new ServerMessageModel("Server", "This is beggining of your chat!", chat.Id));

            return chat;
        }
    }
}
