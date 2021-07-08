using MessengerApiClient;
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

        public MessagingLogic(IMClientDbContext dbContext, IMApiClient apiClient, TokenAndIdProvider tokenAndId) : base(apiClient, dbContext, tokenAndId)
        {

        }

        public async Task RetreiveMessages()
        {

            var messages = await apiClient.GetUsersReceivedMessagesAsync(new TokenDTO() { UserID = tokenAndId.Id, UserToken = tokenAndId.Token });
            foreach(var message in messages.ReceiveMessageList)
            {
                
            }
        }

        public async Task SendMultipleMessages()
        {

            var messages = (List<GroupTextMessage>)dbContext.GroupTextMessages.Where(x => x.MessageState == MessageState.Pending && x.Sender.Id == tokenAndId.Id);
            var encryptedMessages = new List<MessageSendDTO>();
            foreach (var message in messages)
            {

            }

            foreach(var encrypted in encryptedMessages)
            {

            }
        }

        public async Task SendMessage(GroupTextMessage message)
        {
            foreach (var member in message.Group.Members)
            {
                if (member.Id != tokenAndId.Id)
                {
                    
                }
            }

        }


        // wahrscheinlich bs
        public void SaveGroupMessage(string plaintext, long groupId, long senderId, MessageState state)
        {

        }

    }
}
