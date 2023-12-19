using ChatLibrary.DataContracts;
using ChatLibrary.DataContracts.ClientModels;
using ChatLibrary.DataContracts.Common;
using ChatLibrary.DataContracts.ServerModels;
using ChatLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Configuration;

namespace ChatLibrary
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Multiple)]
    public class ChatService : IChatService
    {
        public static ChatService Instance { get; private set; }

        public ChatModel _generalChat { get; private set; }

        public Dictionary<Guid, ChatModel> Chats { get; private set; }

        // Map contractId to clientModel
        public Dictionary<Guid, ClientModel> ClientsByTransaction { get; private set; }

        public Dictionary<int, ClientModel> Clients { get; private set; }
        public ChatService()
        {
            if(Instance != null)
            {
                throw new InvalidOperationException("One instance of this service is created!");
            }

            Instance = this;


            ClientsByTransaction = new Dictionary<Guid, ClientModel>();
            Clients = new Dictionary<int, ClientModel>();
            Chats = new Dictionary<Guid, ChatModel>();
            _generalChat = new ChatModel();
            _generalChat.Name = "#general chat";
            _generalChat.IsGroup = true;

            Chats.Add(_generalChat.Id, _generalChat);
        }

        private bool IsContractValid(ConnectContract contract)
        {
            if (contract == null) return false; 
            return ClientsByTransaction.ContainsKey(contract.ContractID);
        }

        #region IChatServiceImplementation

        public ConnectResult Connect(string username)
        {
            // create new contract
            var contract = new ConnectContract();

            // create new user
            var registeredClient = new ClientModel(username, OperationContext.Current);
            registeredClient.ConnectContract = contract;

            // form result
            var connectResult = new ConnectResult(contract, registeredClient);

            ClientsByTransaction[contract.ContractID] = registeredClient;
            Clients[registeredClient.Id] = registeredClient;

            // add user to general chat
            _generalChat.Members.Add(registeredClient.Id);

            // send message about connection
            _generalChat.SendMessage(new ServerMessageModel("Server", $"{username} connected to the server!", _generalChat.Id));
            
            // notify by special event
            foreach(var client in Clients.Values)
            {
                client.GetCallback().OnClientConnect(client.ConnectContract, registeredClient);
            }

            return connectResult;
        }

        public void SendToServer(ConnectContract contract, string message)
        {
            // check is user valid
            if (IsContractValid(contract) == false) return;

            var client = ClientsByTransaction[contract.ContractID];

            Console.WriteLine($"{client.Username}: {message}");
        }

        public void Disconnect(ConnectContract contract)
        {
            var client = ClientsByTransaction[contract.ContractID];
            ServerMessageModel msg = new ServerMessageModel(client.Username, $"Client {client.Username} - disconnected", _generalChat.Id);

            // release client resources and broke the contract
            ClientsByTransaction.Remove(contract.ContractID);

            // notify all connected users about disconnect
            _generalChat.SendMessage(msg);
        }

        public void SendMessage(ConnectContract contract, ClientMessageModel message)
        {
            
            if(IsContractValid(contract) == false) return;

            // client encode path to which corresponding chat it should be sended

            var sender = ClientsByTransaction[contract.ContractID];

            // send message to given chat if have access
            if (Chats.ContainsKey(message.ChatId) == false) return;

            var chat = Chats[message.ChatId];
            if (chat.Members.Contains(sender.Id) == false) return;

            chat.SendMessage(new ServerMessageModel(sender.Username, message.Message, message.ChatId));
        }

        public List<ClientModel> GetAllClients(ConnectContract contract)
        {
            if (IsContractValid(contract) == false) return null;
            return ClientsByTransaction.Values.ToList();
        }

        public ChatModel GetChatById(Guid chatId)
        {
            if(Chats.ContainsKey(chatId) == false) return null;
            return Chats[chatId];
        }

        public ChatModel GetConversation(ConnectContract contract, int clientId)
        {
            if (IsContractValid(contract) == false) return null;

            // check exist or not?
            if (Clients.ContainsKey(clientId) == false) return null;

            var client1 = ClientsByTransaction[contract.ContractID];
            var client2 = Clients[clientId];

            Guid expectedId = IdHelper.MungeTwoInt(client1.Id, clientId);

            if (Chats.ContainsKey(expectedId) == false)
            {
                // create new conversation
                Chats[expectedId] = ChatModel.CreateConversation(client1, client2);
            }

            return Chats[expectedId];
        }

        public ClientModel GetClientById(int id)
        {
            return Clients[id];
        }


        #endregion
    }
}
