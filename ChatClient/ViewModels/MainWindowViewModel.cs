
using ChatClient.ChatService;
using ChatClient.Commands;
using ChatClient.ViewModels.ChatViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient.ViewModels
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MainWindowViewModel: NotifyViewModel, IChatServiceCallback
    {
        private RelayCommand _sendCommand;

        public RelayCommand SendCommand
        {
            get
            {
                if(_sendCommand != null) return _sendCommand;

                return _sendCommand = new RelayCommand((obj) =>
                {
                    SendMessage(Message);
                    Message = "";
                });
            }
        }

        private ChatViewModel _selectedChat;

        public ChatViewModel SelectedChat
        {
            get
            {
                return _selectedChat;
            }

            set
            {
                _selectedChat = value; 
                OnPropertyChanged(nameof(SelectedChat));
            }
        }

        public ObservableCollection<ChatViewModel> Chats { get; set; }

        private Dictionary<Guid, ChatViewModel> _chatMap { get; set; }

        private bool _isConnected = false;

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));   
                OnPropertyChanged(nameof(IsNotConnected));   
            }
        }

        public bool IsNotConnected
        {
            get
            {
                return !_isConnected;
            }
            set 
            {
                IsConnected = !value;
            }
        }

        private string _message;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private ChatServiceClient _client;

        public ClientModel ClientModel;

        private ConnectContract _connectContract;

        public MainWindowViewModel()
        {
            Chats = new ObservableCollection<ChatViewModel>();
            _chatMap = new Dictionary<Guid, ChatViewModel>();
            //_generalChat = new GeneralChat();
            //_selectedChat = _generalChat;
            //Chats.Add(_generalChat);


# if DEBUG

            //var testClient = new ClientModel()
            //{
            //    Username = "Dmytro"
            //};

            //Chats.Add(new PrivateChat(testClient));
            //Chats.Add(new PrivateChat(testClient));

            //var testModel = new MessageModel()
            //{
            //    SenderName = "Aboba",
            //    Message = "Hello !! I am aboba",
            //    DateTime = DateTime.Now,
            //};


            //SelectedChat.Messages.Add(new MessageViewModel(testModel));
            //SelectedChat.Messages.Add(new MessageViewModel(testModel));
            //SelectedChat.Messages.Add(new MessageViewModel(testModel));
            //SelectedChat.Messages.Add(new MessageViewModel(testModel));
#endif
        }

        private SemaphoreSlim _connectionSemaphore = new SemaphoreSlim(1, 1);

        public async void Connect(string username)
        {
            // Acquire the semaphore, preventing multiple connections at the same time
            await _connectionSemaphore.WaitAsync();

            if (IsConnected) return;

            Trace.WriteLine($"Connecting to server with name {username}...");

            try
            {
                _client = new ChatServiceClient(new InstanceContext(this));

                var connectResult = await _client.ConnectAsync(username);

                ClientModel = connectResult.ClientModel;
                _connectContract = connectResult.Contract;

                IsConnected = true;
                Trace.WriteLine($"Sucessfully connected to the server! Now you have id: {ClientModel.Id}");

                //var chatModel = await _client.GetChatByIdAsync(Guid.Empty);
                //// add general chat
                //AddChat()
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to connect to the server!");
                Trace.WriteLine(ex.ToString()); 
            }
            finally
            {
                _connectionSemaphore.Release();
            }
        }

        public async void Disconnect()
        {
            if (IsConnected == false) return;

            if (_client == null || ClientModel == null) return;
            try
            {
                await _client.DisconnectAsync(_connectContract);
                IsConnected = false;    
            }
            catch
            {
                Trace.WriteLine("Failed to disconnect from the server!");
            }
        }

        public void SendMessageToServer(string s)
        {
            _client?.SendToServerAsync(_connectContract, s);
        }

        public void SendMessage(string s)
        {
            _client?.SendMessage(_connectContract, new ClientMessageModel()
            {
                ChatId = SelectedChat.Id,
                Message = s
            }) ;
        }

        private async Task AddChat(Guid chatId)
        {
            var chatModel = await _client.GetChatByIdAsync(chatId);
            if (chatModel == null)
            {
                Trace.WriteLine("You are not allowed or chat is not exist!");
                return;
            }

            var chatViewModel = new ChatViewModel(chatModel);

            Chats.Add(chatViewModel);
            _chatMap[chatId] = chatViewModel;
        }

        public async void ReceiveMessage(ConnectContract contract, ServerMessageModel message)
        {
            await _connectionSemaphore.WaitAsync();

            if(IsConnected == false) return;

            // Get chat, if chat not in list, retrive data from server
            if(_chatMap.ContainsKey(message.ChatId) == false)
            {
                await AddChat(message.ChatId);
            }
            else
            {
                var chat = _chatMap[message.ChatId];

                // update message info
                chat.Messages.Add(new MessageViewModel(message));
            }

            _connectionSemaphore.Release();
        }

        public void OnClientDisconnect(ConnectContract contract, ClientModel client)
        {
            // nothing...
        }

        public void ReceiveServerMessage(ConnectContract contract, string message)
        {
            Trace.WriteLine(message);
        }

        private async void AddConversation(int clientId)
        {
            if (clientId == ClientModel.Id) return;
            try
            {

                var conversation = await _client.GetConversationAsync(_connectContract, clientId).ConfigureAwait(false);

                // Use the Dispatcher to update the UI on the main thread
                App.Current.Dispatcher.Invoke(() =>
                {
                    var conversationViewModel = new ChatViewModel(conversation);
                    Chats.Add(conversationViewModel);
                    _chatMap[conversation.Id] = conversationViewModel;
                });

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public void OnClientConnect(ConnectContract contract, ClientModel client)
        {
            if(IsConnected == false) return;

            // get conversation with that client
            AddConversation(client.Id);
        }

        public ClientModel GetClient(int clientId)
        {
            return _client.GetClientById(clientId);
        }
    }
}
