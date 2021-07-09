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
        static IUnityContainer container = new UnityContainer();
        static BackgroundWorker worker = new BackgroundWorker();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            

            // TODO Add exception Handler
            var httpClient = new HttpClient(new HttpClientHandler()
            {
                UseDefaultCredentials = true
            });
            container.RegisterInstance(new IMApiClient("https://localhost:44384/", httpClient));
            container.RegisterSingleton<TokenAndIdProvider>();
            container.RegisterSingleton<SignalRClient>();

            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();


            var messengerViewModel = container.Resolve<MessengerViewModel>();
            var messengerView = new MessengerView { DataContext = messengerViewModel };

            messengerView.Show();

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var signalRClient = container.Resolve<SignalRClient>();
            signalRClient.StartAsync().GetAwaiter().GetResult();
        }
    }
}
