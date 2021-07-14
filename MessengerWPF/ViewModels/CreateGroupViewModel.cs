using MessengerWPF.Business;
using MessengerWPF.Models.DbModels;
using MessengerWPF.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MessengerWPF.ViewModels
{
    public class CreateGroupViewModel : ViewModelBase
    {
        private ObservableCollection<User> contacts = new ObservableCollection<User>();//

        private string groupName;

        private ObservableCollection<User> selectedItems = new ObservableCollection<User>();
        private readonly MessengerViewModel messengerViewModel;
        private readonly NavigationStore navigationStore;
        private readonly GroupManagementLogic groupManagement;

        public MyICommand AddContactToListCommand { get; set; }
        public MyICommand SubmitGroupCommand { get; set; }

        public CreateGroupViewModel(MessengerViewModel messengerViewModel, NavigationStore navigationStore, User myself, GroupManagementLogic groupManagement)
        {
            //AddContactToListCommand = new MyICommand(OnAddContactToList);
            SubmitGroupCommand = new MyICommand(async () =>
            {
                await OnSubmitGroup();
            }, CanSubmitGroup);
            this.messengerViewModel = messengerViewModel;
            this.navigationStore = navigationStore;
            this.groupManagement = groupManagement;
            Contacts= new ObservableCollection<User>(myself.Contacts);
        }

        //private bool CanAddContactToList() => SelectedItem != null;, CanAddContactToList)


        //private void OnAddContactToList()
        //{
        //    if (SelectedItems.Contains(SelectedItem))
        //    {
        //        selectedUsers.Remove(SelectedItem);
        //    }
        //    else
        //    {
        //        SelectedItems.Add(SelectedItem);
        //    }
        //    AddContactToListCommand.RaiseCanExecuteChanged();
        //}

        private bool CanSubmitGroup() => GroupName != null; 

        private async Task OnSubmitGroup()
        {
            if(SelectedItems.Count >= 1)
            {
                await groupManagement.AddGroupAsync(GroupName, SelectedItems.ToList());
                await messengerViewModel.GetGroups();
                navigationStore.CurrentViewModel = messengerViewModel;
            }
            else
            {
                MessageBox.Show("You must add at least one member to the Group.");
            }
        }

        public string GroupName
        {
            get { return groupName; }
            set { groupName = value;
                RaisePropertyChanged(nameof(GroupName));
                SubmitGroupCommand.RaiseCanExecuteChanged();
            }
        }

        //private User selectedItem;

        //public User SelectedItem
        //{
        //    get { return selectedItem; }
        //    set { selectedItem = value; 
        //        RaisePropertyChanged(nameof(SelectedItem)); 
        //        AddContactToListCommand.RaiseCanExecuteChanged(); }
        //}


        public ObservableCollection<User> SelectedItems
        {
            get { return selectedItems; }
            set { selectedItems = value; 
                RaisePropertyChanged(nameof(SelectedItems)); 
                SubmitGroupCommand.RaiseCanExecuteChanged(); }
        }


        public ObservableCollection<User> Contacts
        {
            get { return contacts; }
            set { contacts = value; RaisePropertyChanged(nameof(selectedItems)); }
        }

    }
}
