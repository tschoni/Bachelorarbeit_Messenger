using MessengerApiClient;
using MessengerWPF.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MessengerWPF.Cryptography;

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
        public async Task RegisterUserAsync(string name, string password)
        {
            //TODO: Generate public and private Keys
            var registerDTO = new UserRegisterDTO() { Name = name, Password = password };// public Keys
            var tokenDTO = await apiClient.RegisterUserAsync(registerDTO);
            tokenAndId.Id = tokenDTO.UserID;
            tokenAndId.Token = tokenDTO.UserToken;
            var me = new User() { Name = name, Id = tokenDTO.UserID};
            me.Keys = KeyGenerationLogic.GenerateUserKeyList(me);
            dbContext.Users.Add(me);//, Keys = 
            await dbContext.SaveChangesAsync();
            throw new NotImplementedException();
        }
        

        /// <summary>
        /// Login user 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task LoginUserAsync(string name, string password)
        {
            var loginDTO = new UserLoginDTO() { Name = name, Password = password };
            var tokenDTO = await apiClient.LoginUserAsync(loginDTO);
            tokenAndId.Id = tokenDTO.UserID;
            tokenAndId.Token = tokenDTO.UserToken;
        }
    }
}
