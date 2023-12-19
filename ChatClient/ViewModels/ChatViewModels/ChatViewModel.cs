using ChatClient.ChatService;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChatClient.ViewModels.ChatViewModels
{
    public class ChatViewModel: NotifyViewModel
    {
        public string Title
        {
            get
            {
                if (IsGroup)
                {
                    return Name;
                }

                var windowContext = (MainWindowViewModel)App.Current.MainWindow.DataContext;

                var clientModel = windowContext.ClientModel;

                var another = Members[0] == clientModel.Id ? Members[1] : Members[0];

                var anotherClientModel = windowContext.GetClient(another);

                return $"@{anotherClientModel.Username}";
            }
        }
        
        public Guid Id { get; set; }    

        public ObservableCollection<MessageViewModel> Messages { get; set; }
        
        public bool IsGroup { get; set; }

        public string Name { get; set; }

        public int[] Members { get; set; }

        public ChatViewModel(ChatModel chatModel) 
        {
            Id = chatModel.Id;
            IsGroup = chatModel.IsGroup;
            Name = chatModel.Name;
            Members = chatModel.Members;
            Messages = new ObservableCollection<MessageViewModel>(chatModel.Messages.Select(serverMessageModel => new MessageViewModel(serverMessageModel)));
        }
    }
}
