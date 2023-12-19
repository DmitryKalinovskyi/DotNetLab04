using ChatClient.ChatService;
using System;

namespace ChatClient.ViewModels
{
    public class MessageViewModel
    {
        public string SenderName
        {
            get
            {
                return _model.SenderName;
            }
        }

        public string Message
        {
            get
            {
                return _model.Message;
            }
        }

        public DateTime DateTime
        {
            get
            {
                return _model.DateTime;
            }
        }

        private readonly ServerMessageModel _model;

        public MessageViewModel(ServerMessageModel model)
        {
            _model = model;
        }
    }
}
