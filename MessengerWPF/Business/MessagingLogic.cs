using MessengerApiClient;
using MessengerWPF.Cryptography;
using MessengerWPF.Models;
using MessengerWPF.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Business
{
    public class MessagingLogic : BusinessLogicBase
    {
        public MessagingLogic(IMClientDbContext dbContext, IMApiClient apiClient, TokenAndIdProvider tokenAndId) : base(apiClient, dbContext, tokenAndId)
        {

        }

        public async Task RetreiveMessagesAsync()
        {

            var messages = await apiClient.GetUsersReceivedMessagesAsync(new TokenDTO() { Id = tokenAndId.Id, UserToken = tokenAndId.Token });
            if (messages == null)
            {
                return;
            }
            foreach(var message in messages.ToList())
            {
                var sender = await dbContext.Users.Include(x => x.Keys).FirstAsync(x => x.Id == message.Sender.Id);

                var plaintext = AuthenticatedEncrytionLogic.Decrypt(message.CipherText, sender.Keys.Find(x => x.KeyType == KeyType.MasterKey).KeyBytes);
                var messageContent = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageContent>(plaintext);
                var group = await dbContext.Groups.Include(x => x.Members).FirstAsync(x => x.Id == messageContent.GroupId);
                if (!group.Members.Exists(x => x.Id == message.Sender.Id))
                {
                    continue;
                }
                var grTextMessage = new GroupTextMessage() { Sender = sender, Text = messageContent.Plaintext, Group = group, MessageState = MessageState.Resolved, TimeStamp = message.TimeStamp };
                dbContext.GroupTextMessages.Add(grTextMessage);
                await dbContext.SaveChangesAsync();
                await apiClient.DeleteMessageAsync(message.Id);
            }
        }// Frage: jedensmal safen oder am ende?(List<GroupTextMessage>)

        public async Task SendPendingMessagesAsync()
        {

            var messages = dbContext.GroupTextMessages.Include(x => x.Group).ThenInclude(x => x.Members).ThenInclude(x => x.Keys).Where(x => x.MessageState == MessageState.Pending && x.Sender.Id == tokenAndId.Id);
            if (messages == null)
            {
                return;
            }
            var encryptedMessages = new List<MessageSendDTO>();
            foreach (var message in messages)
            {
                encryptedMessages.AddRange(await GenerateMessageDTOs(message));
                message.TimeStamp = DateTime.Now;
                message.MessageState = MessageState.Resolved;
            }
            await apiClient.PostMultipleMessagesAsync(tokenAndId.Token,  encryptedMessages );
        }

        public async Task SendMessageAsync(string plaintext, Group selectedGroup, SignalRClient signalRClient)
        {
            var group = await dbContext.Groups.Include(x => x.Members).ThenInclude(x => x.Keys).FirstAsync(x => x.Id == selectedGroup.Id);
            var groupTextMessage = new GroupTextMessage() { Group = group, Text = plaintext, Sender = group.Members.Find(x => x.Id == tokenAndId.Id) };
            if (signalRClient.IsConnected)
            {
                var messageDTOs = await  GenerateMessageDTOs(groupTextMessage);
                await apiClient.PostMultipleMessagesAsync(tokenAndId.Token, messageDTOs );
                groupTextMessage.TimeStamp =  DateTime.Now;
                groupTextMessage.MessageState = MessageState.Resolved;
            }
            else
            {
                groupTextMessage.MessageState = MessageState.Pending;
            }
            dbContext.GroupTextMessages.Add(groupTextMessage);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<MessageSendDTO>> GenerateMessageDTOs(GroupTextMessage message)
        {
            var messageContent = new MessageContent() { GroupId = message.Group.Id, Plaintext = message.Text };
            var plaintext = Newtonsoft.Json.JsonConvert.SerializeObject(messageContent);
            var encryptedMessages = new List<MessageSendDTO>();
            var me = await dbContext.Users.Include(x => x.Keys).FirstAsync(x => x.Id == tokenAndId.Id);
            foreach (var member in message.Group.Members)
            {
                if (member.Id != tokenAndId.Id)
                {
                    var ciphertext = AuthenticatedEncrytionLogic.Encrypt(plaintext, member.Keys.Find(x => x.KeyType == KeyType.MasterKey).KeyBytes);
                    var messageDTO = new MessageSendDTO() { Sender = new UserDTO() { Id = me.Id, Name =me.Name} , Recipient = new UserDTO() { Id = member.Id, Name = member.Name }, CipherText = ciphertext };
                    encryptedMessages.Add(messageDTO);
                }
            }
            return encryptedMessages;
        }


    }
}
