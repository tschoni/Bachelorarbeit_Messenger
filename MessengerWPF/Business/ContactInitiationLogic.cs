using MessengerApiClient;
using MessengerWPF.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerWPF.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace MessengerWPF.Business
{
    public class ContactInitiationLogic : BusinessLogicBase
    {
        public ContactInitiationLogic(IMApiClient apiClient, IMClientDbContext dbContext, TokenAndIdProvider tokenAndId) : base(apiClient, dbContext, tokenAndId)
        {
                
        }

        public async Task InitiateKeyExchangeByNameAsync(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentException("UserName not valid.");
            }

                var userDetails = await apiClient.GetUserByNameAsync(userName);
                await InitiateKeyExchangeAsync(userDetails);

        }

        public async Task InitiateKeyExchangeByIdAsync(long userId)
        {
            if (userId == null)
            {
                throw new ArgumentException("UserId not valid.");
            }
            var userDetails = await apiClient.GetUserByIdAsync(userId);
            await InitiateKeyExchangeAsync(userDetails);
        }

        private async Task InitiateKeyExchangeAsync(UserDetailsDTO userDetails)
        {
            var myself = await dbContext.Users.Include(x => x.Keys).Include(x => x.Contacts).FirstAsync(x => x.Id == tokenAndId.Id);
            var myKeys = myself.Keys;

            var userKeys = userDetails.PublicKeys.ToList();
            var newContact = new User() { Id = userDetails.Id, Name = userDetails.Name, AdminOfGroups = new List<Group>(), Contacts = new List<User>(), Groups = new List<Group>(), Keys=new List<Key>(), Messages=new List<Message>() };
            newContact.Contacts.Add(myself);
            if(myself.Contacts == null)
            {
                myself.Contacts = new List<User>();
            }
            myself.Contacts.Add(newContact);
            var myEphemKeys = KeyGenerationLogic.GenerateKeyPair(KeyGenerationLogic.CreateCngKey());
            await apiClient.PostEphemeralKeyAsync( tokenAndId.Token, new EphemKeyDTO() { Initiator = new UserDTO() { Id = myself.Id, Name = myself.Name}, Owner = new UserDTO() { Id = newContact.Id, Name = newContact.Name }, KeyBytes = myEphemKeys.PublicKey });
            var masterKey = KeyGenerationLogic.GenerateMasterKeyAsInitiator(myKeys.Find(x => x.KeyType == KeyType.IdentityKeyPrivate), myEphemKeys.PrivateKey, userKeys.Find(x => x.KeyType == PublicKeyType.SignedKey), userKeys.Find(x => x.KeyType == PublicKeyType.IdKey), newContact);
            
            dbContext.Keys.Add(masterKey);
            await dbContext.SaveChangesAsync();
            
        }

        public async Task ReactOnKeyExchangeInitiationAsync()
        {
            var ephemKeyList = await apiClient.GetEphemeralKeysAsync(tokenAndId.Id, tokenAndId.Token);
            if (ephemKeyList == null)
            {
                return;
            }
            var myself = await dbContext.Users.Include(x => x.Contacts).Include(x => x.Keys).FirstAsync(x => x.Id == tokenAndId.Id);
            foreach( var ephemKey in ephemKeyList.KeyDTOs)
            {
                var contact = await dbContext.Users.FindAsync(ephemKey.Initiator.Id);
                if (contact != null)
                {
                    continue;
                }
                var userDetails = await apiClient.GetUserByIdAsync(ephemKey.Initiator.Id);
                var userKeys = userDetails.PublicKeys.ToList();
                var newContact = new User() { Id = userDetails.Id, Name = userDetails.Name, AdminOfGroups = new List<Group>(), Contacts = new List<User>(), Groups = new List<Group>(), Keys = new List<Key>(), Messages = new List<Message>() };
                newContact.Contacts.Add(myself);
                if (myself.Contacts == null)
                {
                    myself.Contacts = new List<User>();
                }
                myself.Contacts.Add(newContact);
                var masterKey = KeyGenerationLogic.GenerateMasterKeyAsReactor(myself.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPrivate), myself.Keys.Find(x => x.KeyType == KeyType.SignedKeyPrivate), ephemKey, userKeys.Find(x => x.KeyType == PublicKeyType.IdKey), contact);
                dbContext.Keys.Add(masterKey);
                await dbContext.SaveChangesAsync();
            }

        }

    }
}
