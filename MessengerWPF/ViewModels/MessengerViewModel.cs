using MessengerWPF.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.ViewModels
{
    public class MessengerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Group selectedGroup;
        private ObservableCollection<Group> groups = new ObservableCollection<Group>();
        private string messageInput;


        public MyICommand SendMessageCommand { get; set; }
        public MyICommand SelectGroupCommand { get; set; }

        public MessengerViewModel()
        {

            SendMessageCommand = new MyICommand(OnSendMessage, CanSendMessage);
            SelectGroupCommand = new MyICommand(OnSelectGroup);

        }

        private void OnSelectGroup()
        {
            throw new NotImplementedException();
        }

        private bool CanSendMessage() => selectedGroup == null && messageInput == null;

        private void OnSendMessage()
        {
            throw new NotImplementedException();
        }

        public string MessageInput
        {
            get { return messageInput; }
            set {
                messageInput = value;
                RaisePropertyChanged(nameof(MessageInput));
            }
        }

        public ObservableCollection<Group> Groups
        {
            get { return groups; }
            set 
            { 
                groups = value;
                RaisePropertyChanged(nameof(Groups));
            }
        }

        public Group SelectedGroup
        {
            get { return selectedGroup; }
            set { 
                selectedGroup = value;
                RaisePropertyChanged(nameof(SelectedGroup));
            }
        }


        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
