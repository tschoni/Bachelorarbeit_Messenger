using MessengerApiClient;
using MessengerWPF.Business;
using MessengerWPF.ViewModels;
using MessengerWPF.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace MessengerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static SignalRClient signalRClient = new SignalRClient();
        static BackgroundWorker worker = new BackgroundWorker();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer container = new UnityContainer();

            // TODO Add exception Handler
            var httpClient = new HttpClient(new HttpClientHandler()
            {
                UseDefaultCredentials = true
            });
            container.RegisterInstance(new IMApiClient("https://localhost:44384/", httpClient));
            container.RegisterSingleton<TokenAndIdProvider>();

            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();


            var messengerViewModel = container.Resolve<MessengerViewModel>();
            var messengerView = new MessengerView { DataContext = messengerViewModel };

            messengerView.Show();

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            signalRClient.StartAsync().GetAwaiter().GetResult();
        }
    }
}
