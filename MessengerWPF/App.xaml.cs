using MessengerApiClient;
using MessengerWPF.Business;
using MessengerWPF.Stores;
using MessengerWPF.ViewModels;
using MessengerWPF.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
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
        //static BackgroundWorker worker = new BackgroundWorker();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            var clientDbContext = new IMClientDbContext();
            clientDbContext.Database.Migrate();
            // TODO Add exception Handler
            var httpClient = new HttpClient(new HttpClientHandler()
            {
                UseDefaultCredentials = true               
            });
            httpClient.BaseAddress = new Uri("https://localhost:44384/");
            container.RegisterInstance(httpClient);
            //container.RegisterInstance(new IMApiClient("https://localhost:44384/", httpClient));
            container.RegisterSingleton<TokenAndIdProvider>();
            container.RegisterSingleton<SignalRClient>();
            container.RegisterSingleton<NavigationStore>();

            container.RegisterSingleton<BackgroundWorker>();
            var worker = container.Resolve<BackgroundWorker>();
            worker.DoWork += Worker_DoWork;
            //worker.RunWorkerAsync();

            var mainWindowViewModel = container.Resolve<MainWindowViewModel>();
            var mainWindowView = new MainWindowView { DataContext = mainWindowViewModel };



            mainWindowView.Show();

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var signalRClient = container.Resolve<SignalRClient>();
                signalRClient.StartAsync().GetAwaiter().GetResult();
            }
            catch(Exception ex)
            {
                var m = ex.Message;
                var st = ex.StackTrace;
            }
            
        }
    }
}
