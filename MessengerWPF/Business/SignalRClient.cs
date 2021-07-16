using MessengerCommon;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerWPF.Business;

namespace MessengerWPF.Business
{
    public class SignalRClient : IDisposable
    {
        HubConnection hubConnection;
        private readonly TokenAndIdProvider tokenAndId;
        private readonly GroupManagementLogic groupManagement;
        private readonly MessagingLogic messagingLogic;
        private readonly ContactInitiationLogic contactInitiation;

        public event Func<long, Task> OnNewMessages;
        public event Func<long, Task> OnGroupChange;

        public SignalRClient(TokenAndIdProvider tokenAndId, ContactInitiationLogic contactInitiation, GroupManagementLogic groupManagement, MessagingLogic messagingLogic  ) //
        {
            this.tokenAndId = tokenAndId;
            this.groupManagement = groupManagement;
            this.messagingLogic = messagingLogic;
            this.contactInitiation = contactInitiation;
            hubConnection = new HubConnectionBuilder()
                 .WithUrl($"https://localhost:44384/signalr")
                //.WithUrl($"https://asjhasdjhdjkashdjahsd/signalr")
                 .WithAutomaticReconnect()
                 .Build();


            hubConnection.Closed += HubConnection_Closed;
            hubConnection.Reconnected += HubConnection_Reconnected;
            hubConnection.Reconnecting += HubConnection_Reconnecting;

            /**/
            hubConnection.On<NotifyMessage>(nameof(INotificationClient.NotifyMessage), (message) =>
            {
                if (this.tokenAndId.Id == message.UserId)
                {
                    switch (message.MessageType)
                    {
                        case MessageType.EphemKeyPosted:
                            contactInitiation.ReactOnKeyExchangeInitiationAsync().GetAwaiter().GetResult();
                            break;
                        case MessageType.GroupCreated:
                        case MessageType.GroupAdminAdded:
                        case MessageType.GroupAdminRemoved:
                        case MessageType.GroupMemberAdded:
                        case MessageType.GroupMemberRemoved:
                            if (message.GroupId != null)
                            {
                                groupManagement.UpdateGroupByIdAsync((long)message.GroupId).GetAwaiter().GetResult();
                                OnGroupChange?.Invoke((long)message.GroupId);
                            }                            
                            break;
                        case MessageType.GroupDeleted:
                            if (message.GroupId != null)
                            {
                                groupManagement.DeleteGroupAsync((long)message.GroupId).GetAwaiter().GetResult();
                            }
                            break;
                        case MessageType.MessagePosted:
                            messagingLogic.RetreiveMessagesAsync().GetAwaiter().GetResult();
                            OnNewMessages?.Invoke(0/*message.GroupId*/);


                            // 
                            break;
                    }
                }
            });

        }
        public bool IsConnected
        {
            get
            {
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public  Task StartAsync()
        {
            try
            {
                hubConnection.StartAsync().GetAwaiter().GetResult();
                contactInitiation.ReactOnKeyExchangeInitiationAsync().GetAwaiter().GetResult();
                groupManagement.UpdateAllGroupAsync().GetAwaiter().GetResult();
                
                messagingLogic.RetreiveMessagesAsync().GetAwaiter().GetResult();
                messagingLogic.SendPendingMessagesAsync().GetAwaiter().GetResult();
                OnGroupChange?.Invoke(0);
                OnNewMessages?.Invoke(0);/*message.GroupId*/
            }
            catch (Exception ex )
            {

                throw ;
            }

            return Task.CompletedTask;
        }

        private Task HubConnection_Reconnecting(Exception arg)
        {
            return Task.CompletedTask;
        }

        private Task HubConnection_Reconnected(string arg)
        {
            contactInitiation.ReactOnKeyExchangeInitiationAsync().GetAwaiter().GetResult();
            groupManagement.UpdateAllGroupAsync().GetAwaiter().GetResult();
            messagingLogic.RetreiveMessagesAsync().GetAwaiter().GetResult();
            messagingLogic.SendPendingMessagesAsync().GetAwaiter().GetResult();
            OnGroupChange?.Invoke(0);
            OnNewMessages?.Invoke(0);/*message.GroupId*/
            return Task.CompletedTask;
        }

        private Task HubConnection_Closed(Exception arg)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
