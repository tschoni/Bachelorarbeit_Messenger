using MessengerCommon;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRConsole
{
    public class SignalRClient
    {
        HubConnection hubConnection;

        public SignalRClient()
        {
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
                Console.WriteLine("UserId: {0}",message.UserId.ToString());
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

        public async Task StartAsync()
        {
            try
            {
                await hubConnection.StartAsync();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private Task HubConnection_Reconnecting(Exception arg)
        {
            return Task.CompletedTask;
        }

        private Task HubConnection_Reconnected(string arg)
        {
            return Task.CompletedTask;
        }

        private Task HubConnection_Closed(Exception arg)
        {
            return Task.CompletedTask;
        }
    }
}
