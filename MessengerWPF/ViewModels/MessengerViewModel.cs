using MessengerWPF.Cryptography;
using MessengerWPF.Models.DbModels;
using MessengerWPF.Business;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerWPF.Stores;
using System.Net.Http;
using System.Windows;
using MessengerApiClient;
using Microsoft.EntityFrameworkCore;

namespace MessengerWPF.ViewModels
{
    public class MessengerViewModel : ViewModelBase
    {
        private Group selectedGroup;
        private ObservableCollection<Group> groups = new ObservableCollection<Group>();

        //private ObservableCollection<GroupMessage> groupMessages = new ObservableCollection<GroupMessage>();        
        private string messageInput;
        private string contactName;
        private readonly MessagingLogic messagingLogic;
        private readonly GroupManagementLogic groupManagementLogic; // vllt unnötig
        private readonly SignalRClient signalRClient;
        private readonly NavigationStore navigationStore;
        private readonly IMClientDbContext iMClientDb;
        private readonly TokenAndIdProvider tokenAndId;
        private readonly ContactInitiationLogic contactInitiationLogic;

        public MyICommand SendMessageCommand { get; set; }
        public MyICommand AddContactCommand { get; set; }
        public MyICommand AddGroupCommand { get; set; }

        //public MyICommand SelectGroupCommand { get; set; }

        public MessengerViewModel(MessagingLogic messagingLogic, 
            ContactInitiationLogic contactInitiationLogic, 
            GroupManagementLogic groupManagementLogic,
            SignalRClient signalRClient, 
            NavigationStore navigationStore, 
            IMClientDbContext iMClientDb, 
            TokenAndIdProvider tokenAndId)
        {
            this.messagingLogic = messagingLogic;
            this.groupManagementLogic = groupManagementLogic;
            this.signalRClient = signalRClient;
            this.navigationStore = navigationStore;
            this.iMClientDb = iMClientDb;
            this.tokenAndId = tokenAndId;
            this.contactInitiationLogic = contactInitiationLogic;


            SendMessageCommand = new MyICommand(async () =>
            {
                await OnSendMessage();
            }, CanSendMessage);

            AddContactCommand = new MyICommand(async () => {
                await OnAddContact();
                }, CanAddContact);
            AddGroupCommand = new MyICommand(async () => {
                await OnAddGroup();
            }, CanAddGroup);

            GetGroups().GetAwaiter();

            //SelectGroupCommand = new MyICommand(OnSelectGroup);

            /*var alice = new User() { Name = "Alice", Id = 1 };
            alice.Keys = KeyGenerationLogic.GenerateUserKeyList(alice);
            var alSigKey = (SignedKey)alice.Keys.Find(x => x.KeyType == KeyType.SignedKeyPublic);
            var alIdKey = alice.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPublic);
            if (KeyGenerationLogic.VerifySignedKey(alSigKey.Signature, alSigKey.KeyBytes, alIdKey.KeyBytes))
            {
                SearchInput = "valid";
            }
            var bob = new User() { Name = "Bob", Id = 1 };
            bob.Keys = KeyGenerationLogic.GenerateUserKeyList(bob);
            var aliceEph = KeyGenerationLogic.GenerateKeyPair( KeyGenerationLogic.CreateCngKey());
            var sharedKeyA = KeyGenerationLogic.GenerateMasterKeyAsInitiator(alice.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPrivate), aliceEph.PrivateKey, new MessengerApiClient.PublicKeyDTO() { KeyBytes = bob.Keys.Find(x => x.KeyType == KeyType.SignedKeyPublic).KeyBytes }, new MessengerApiClient.PublicKeyDTO() { KeyBytes = bob.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPublic).KeyBytes }, bob);
            var sharedKeyB = KeyGenerationLogic.GenerateMasterKeyAsReactor(bob.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPrivate), bob.Keys.Find(x => x.KeyType == KeyType.SignedKeyPrivate), new MessengerApiClient.EphemKeyDTO() { KeyBytes = aliceEph.PublicKey }, new MessengerApiClient.PublicKeyDTO() { KeyBytes = alice.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPublic).KeyBytes }, alice);
            MessageInput = "Alice Master Key: " + Convert.ToBase64String(sharedKeyA.KeyBytes) + "\n";
            MessageInput += "Bob Master Key: " + Convert.ToBase64String(sharedKeyB.KeyBytes);*/

        }

        private ObservableCollection<GroupMessage> groupMessages = new ObservableCollection<GroupMessage>();

        public ObservableCollection<GroupMessage> GroupMessages
        {
            get { return groupMessages; }
            set { groupMessages = value;
                RaisePropertyChanged(nameof(GroupMessages));

            }
        }


        private bool CanAddContact() => ContactName != String.Empty;

        private async Task OnAddContact()
        {
            try
            {
                await contactInitiationLogic.InitiateKeyExchangeByNameAsync(ContactName);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Unable to connect with server.");
            }
            catch (ApiException)
            {
                MessageBox.Show("There is no user with this name.");
            }
            catch (Exception)
            {
                MessageBox.Show("There is no user with this name.");
            }
            //throw new NotImplementedException();
        }
        private bool CanAddGroup() => true;


        private async Task OnAddGroup()
        {
            var me = await iMClientDb.Users.Include(x => x.Contacts).FirstAsync(x => x.Id == tokenAndId.Id);
            navigationStore.CurrentViewModel = new CreateGroupViewModel(this, navigationStore, me, groupManagementLogic);
            
            //throw new NotImplementedException();
        }



        //private void OnSelectGroup()
        //{
        //    GroupMessages. SelectedGroup.Messages;
        //}

        private bool CanSendMessage() => selectedGroup != null && messageInput != null;

        private async Task OnSendMessage()
        {
            await messagingLogic.SendMessageAsync(MessageInput, SelectedGroup, signalRClient);
        }

        public string MessageInput
        {
            get { return messageInput; }
            set {
                messageInput = value;
                RaisePropertyChanged(nameof(MessageInput));
                SendMessageCommand.RaiseCanExecuteChanged();
            }
        }



        public string ContactName
        {
            get { return contactName; }
            set { contactName = value; 
                RaisePropertyChanged(nameof(ContactName));
                AddContactCommand.RaiseCanExecuteChanged();
            }
        }

  

        //public ObservableCollection<GroupMessage> GroupMessages
        //{
        //    get { return groupMessages; }
        //    set { 
        //        groupMessages = value;
        //        RaisePropertyChanged(nameof(GroupMessages));
        //    }
        //}

        public async Task GetGroups()
        {

            var me = await iMClientDb.Users.Include(x => x.Groups).FirstAsync(x => x.Id == tokenAndId.Id);
            if (me == null || me.Groups == null)
            {
                return;
            }
            Groups = new ObservableCollection<Group>(me.Groups);

        }
        public void GetMessages()
        {
            var messages = iMClientDb.GroupTextMessages.Where(x => x.Group.Id == selectedGroup.Id).ToList();
            GroupMessages = new ObservableCollection<GroupMessage>(messages);
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
                GetMessages();
               // RaisePropertyChanged(nameof(GroupMessages));
                SendMessageCommand.RaiseCanExecuteChanged();
            }
        }

    }
}
/*
            catch (HttpRequestException)
            {
                MessageBox.Show("Unable to connect with server.");
            }
            catch (ApiException)
            {
                MessageBox.Show("There are no documents uploaded or unable to connect with server.");
            }
*/