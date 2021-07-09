using MessengerApiClient;
using MessengerWPF.Cryptography;
using MessengerWPF.Models;
using MessengerWPF.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Business
{
    public class MessagingLogic : BusinessLogicBase
    {
        private readonly SignalRClient signalRClient;

        public MessagingLogic(IMClientDbContext dbContext, IMApiClient apiClient, TokenAndIdProvider tokenAndId, SignalRClient signalRClient) : base(apiClient, dbContext, tokenAndId)
        {
            this.signalRClient = signalRClient;
        }

        public async Task RetreiveMessagesAsync()
        {

            var messages = await apiClient.GetUsersReceivedMessagesAsync(new TokenDTO() { UserID = tokenAndId.Id, UserToken = tokenAndId.Token });
            if (messages == null)
            {
                return;
            }
            foreach(var message in messages.ReceiveMessageList)
            {
                var sender = await dbContext.Users.FindAsync(message.SenderId);

                var plaintext = AuthenticatedEncrytionLogic.Decrypt(message.CipherText, sender.Keys.Find(x => x.KeyType == KeyType.MasterKey).KeyString);
                var messageContent = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageContent>(plaintext);
                var group = await dbContext.Groups.FindAsync(messageContent.GroupId);
                var grTextMessage = new GroupTextMessage() { Sender = sender, Text = messageContent.Plaintext, Group= group, MessageState = MessageState.Resolved, TimeStamp=message.TimeStamp };
                dbContext.GroupTextMessages.Add(grTextMessage);
                await dbContext.SaveChangesAsync();
            }
        }// Frage: jedensmal safen oder am ende?

        public async Task SendPendingMessagesAsync()
        {

            var messages = (List<GroupTextMessage>)dbContext.GroupTextMessages.Where(x => x.MessageState == MessageState.Pending && x.Sender.Id == tokenAndId.Id);
            if (messages == null)
            {
                return;
            }
            var encryptedMessages = new List<MessageSendDTO>();
            foreach (var message in messages)
            {
                encryptedMessages.AddRange(GenerateMessageDTOs(message));
                message.TimeStamp = DateTime.Now;
                message.MessageState = MessageState.Resolved;
            }
            await apiClient.PostMultipleMessagesAsync(tokenAndId.Token, new MessageSendListDTO() { MessageList = encryptedMessages });
        }

        public async Task SendMessageAsync(string plaintext, Group group)
        {
            var groupTextMessage = new GroupTextMessage() { Group = group, Text = plaintext, Sender = group.Members.Find(x => x.Id == tokenAndId.Id) };
            if (signalRClient.IsConnected)
            {
                var messageDTOs =  GenerateMessageDTOs(groupTextMessage);
                await apiClient.PostMultipleMessagesAsync(tokenAndId.Token, new MessageSendListDTO() { MessageList = messageDTOs });
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

        public List<MessageSendDTO> GenerateMessageDTOs(GroupTextMessage message)
        {
            var messageContent = new MessageContent() { GroupId = message.Group.Id, Plaintext = message.Text };
            var plaintext = Newtonsoft.Json.JsonConvert.SerializeObject(messageContent);
            var encryptedMessages = new List<MessageSendDTO>();
            foreach (var member in message.Group.Members)
            {
                if (member.Id != tokenAndId.Id)
                {
                    var ciphertext = AuthenticatedEncrytionLogic.Encrypt(plaintext, member.Keys.Find(x => x.KeyType == KeyType.MasterKey).KeyString);
                    var messageDTO = new MessageSendDTO() { SenderId = tokenAndId.Id, RecipientId = member.Id, CipherText = ciphertext };
                    encryptedMessages.Add(messageDTO);
                }
            }
            return encryptedMessages;
        }


    }
}
