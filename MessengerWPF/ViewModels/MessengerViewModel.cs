using MessengerWPF.Cryptography;
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
        private string searchInput;

        public MyICommand SendMessageCommand { get; set; }
        public MyICommand SelectGroupCommand { get; set; }

        public MessengerViewModel()
        {

            SendMessageCommand = new MyICommand(OnSendMessage, CanSendMessage);
            SelectGroupCommand = new MyICommand(OnSelectGroup);

            var alice = new User() { Name = "Alice", Id = 1 };
            alice.Keys = KeyGenerationLogic.GenerateUserKeyList(alice);
            var alSigKey = (SignedKey)alice.Keys.Find(x => x.KeyType == KeyType.SignedKeyPublic);
            var alIdKey = alice.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPublic);
            if (KeyGenerationLogic.VerifySignedKey(alSigKey.Signature, alSigKey.KeyString, alIdKey.KeyString))
            {
                SearchInput = "valid";
            }
            var bob = new User() { Name = "Bob", Id = 1 };
            bob.Keys = KeyGenerationLogic.GenerateUserKeyList(bob);
            var aliceEph = KeyGenerationLogic.GenerateKeyPair( KeyGenerationLogic.CreateCngKey());
            var sharedKeyA = KeyGenerationLogic.GenerateMasterKeyAsInitiator(alice.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPrivate), aliceEph.PrivateKey, new MessengerApiClient.PublicKeyDTO() { KeyString = bob.Keys.Find(x => x.KeyType == KeyType.SignedKeyPublic).KeyString }, new MessengerApiClient.PublicKeyDTO() { KeyString = bob.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPublic).KeyString }, bob);
            var sharedKeyB = KeyGenerationLogic.GenerateMasterKeyAsReactor(bob.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPrivate), bob.Keys.Find(x => x.KeyType == KeyType.SignedKeyPrivate), new MessengerApiClient.PublicKeyDTO() { KeyString = aliceEph.PublicKey }, new MessengerApiClient.PublicKeyDTO() { KeyString = alice.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPublic).KeyString }, alice);
            MessageInput = "Alice Master Key: " + Convert.ToBase64String(sharedKeyA.KeyString) + "\n";
            MessageInput += "Bob Master Key: " + Convert.ToBase64String(sharedKeyB.KeyString);

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



        public string SearchInput
        {
            get { return searchInput; }
            set { searchInput = value; RaisePropertyChanged(nameof(SearchInput)); }
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
