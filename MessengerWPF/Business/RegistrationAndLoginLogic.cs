using MessengerApiClient;
using MessengerWPF.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MessengerWPF.Cryptography;
using System.Net.Http;

namespace MessengerWPF.Business
{
    public class RegistrationAndLoginLogic : BusinessLogicBase
    {


        public RegistrationAndLoginLogic(IMApiClient apiClient, IMClientDbContext dbContext, TokenAndIdProvider tokenAndId) : base(apiClient, dbContext, tokenAndId)
        {

        }

        /// <summary>
        /// Register user with public keys and save user with public and private keys in db
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> RegisterUserAsync(string name, string password)
        {
            var me = new User() { Name = name, AdminOfGroups = new List<Group>(), Contacts = new List<User>(), Groups = new List<Group>(), Keys = new List<Key>(), Messages = new List<Message>() };
            me.Keys = KeyGenerationLogic.GenerateUserKeyList(me);
            var idKey = me.Keys.Find(x => x.KeyType == KeyType.IdentityKeyPublic);
            var sigKey = (SignedKey)me.Keys.Find(x => x.KeyType == KeyType.SignedKeyPublic);
            var publicKeys = new List<PublicKeyDTO> { new PublicKeyDTO { KeyBytes = idKey.KeyBytes, KeyType = PublicKeyType.IdKey }, new PublicKeyDTO { KeyBytes =sigKey.KeyBytes, KeyType = PublicKeyType.SignedKey, Signature=sigKey.Signature } };
            //TODO: Generate public and private Keys
            var registerDTO = new UserRegisterDTO() { Name = name, Password = password, PublicKeys = publicKeys};// public Keys Signature
            try
            {
                var tokenDTO = await apiClient.RegisterUserAsync(registerDTO);
                tokenAndId.Id = tokenDTO.Id;
                tokenAndId.Token = tokenDTO.UserToken;
                me.Id = tokenAndId.Id;
                dbContext.Users.Add(me);//, Keys = 
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (ApiException)
            {
                return false;
            }   
        }
        

        /// <summary>
        /// Login user 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> LoginUserAsync(string name, string password)
        {
            var loginDTO = new UserLoginDTO() { Name = name, Password = password };
            try
            {
                var tokenDTO = await apiClient.LoginUserAsync(loginDTO);
                tokenAndId.Id = tokenDTO.Id;
                tokenAndId.Token = tokenDTO.UserToken;
                /*var me = await  dbContext.Users.FindAsync(tokenAndId.Id);
                if (me == null)
                {
                    me = new User() {Id= tokenAndId.Id,  Name = name, AdminOfGroups = new List<Group>(), Contacts = new List<User>(), Groups = new List<Group>(), Keys = new List<Key>(), Messages = new List<Message>() };
                    await dbContext.SaveChangesAsync();
                }*/

                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch(ApiException)
            {
                return false;
            }
            
        }
    }
}
