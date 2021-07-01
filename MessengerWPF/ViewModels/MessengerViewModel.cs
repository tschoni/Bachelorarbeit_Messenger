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
        private ObservableCollection<Group> groups;



        public MessengerViewModel()
        {

        }

 

        public ObservableCollection<Group> Groups
        {
            get { return groups; }
            set { groups = value; }
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
