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
    class ChangeMembersViewModel : ViewModelBase
    {
        private ObservableCollection<User> members = new ObservableCollection<User>();//

        private string pursuedAction;




        private ObservableCollection<User> selectedItems = new ObservableCollection<User>();
        private readonly MessengerViewModel messengerViewModel;
        private readonly NavigationStore navigationStore;
        private readonly bool add;
        private readonly bool member;
        private readonly GroupManagementLogic groupManagement;

        public MyICommand AddContactToListCommand { get; set; }
        public MyICommand SubmitCommand { get; set; }

        public ChangeMembersViewModel(MessengerViewModel messengerViewModel, NavigationStore navigationStore, Group selectedGroup, User me, bool add, bool member, GroupManagementLogic groupManagement)
        {
            //AddContactToListCommand = new MyICommand(OnAddContactToList);
            SubmitCommand = new MyICommand(async () =>
            {
                await OnSubmit();
            }, CanSubmit);
            this.messengerViewModel = messengerViewModel;
            this.navigationStore = navigationStore;
            this.add = add;
            this.member = member;
            this.groupManagement = groupManagement;
            Members = new ObservableCollection<User>(selectedGroup.Members);
        }
        public string PursuedAction
        {
            get { return pursuedAction; }
            set { pursuedAction = value; }
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
        private void SetContactSelection(Group group, User me)
        {
            switch (add, member)
            {
                case (true, true):
                    Members = new ObservableCollection<User>(me.Contacts);
                    PursuedAction = "Add Members";
                    break;
                case (true, false):
                    Members = new ObservableCollection<User>(group.Members);
                    PursuedAction = "Add Admins";
                    break;
                case (false, true):
                    Members = new ObservableCollection<User>(group.Members);
                    PursuedAction = "Remove Members";
                    break;
                case (false, false):
                    PursuedAction = "Remove Admins";
                    Members = new ObservableCollection<User>(group.Admins);
                    break;
            }
        }
        private bool CanSubmit() => SelectedItems.Count >= 1;

        private async Task OnSubmit()
        {
            if (SelectedItems.Count >= 1)
            {
                switch(add, member)
                {
                    case (true, true):
                        foreach (var selected in SelectedItems)
                        {
                            await groupManagement.AddGroupMemberAsync(selected.Id);
                        }
                        break;
                    case (true, false):
                        foreach (var selected in SelectedItems)
                        {
                            await groupManagement.AddGroupAdminAsync(selected.Id);
                        }
                        break;
                    case (false, true):
                        foreach (var selected in SelectedItems)
                        {
                            await groupManagement.RemoveGroupMemberAsync(selected.Id);
                        }
                        break;
                    case (false, false):
                        foreach (var selected in SelectedItems)
                        {
                            await groupManagement.RemoveGroupAdminAsync(selected.Id);
                        }
                        break;
                }
                await messengerViewModel.GetGroups();
                navigationStore.CurrentViewModel = messengerViewModel;
            }
            else
            {
                MessageBox.Show("You must choose at least one member.");
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
            set
            {
                selectedItems = value;
                RaisePropertyChanged(nameof(SelectedItems));
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }


        public ObservableCollection<User> Members
        {
            get { return members; }
            set { members = value; RaisePropertyChanged(nameof(selectedItems)); }
        }

    }
}
