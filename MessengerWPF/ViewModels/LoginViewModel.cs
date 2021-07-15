using MessengerWPF.Business;
using MessengerWPF.Stores;
using MessengerWPF.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MessengerWPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly RegistrationAndLoginLogic registrationAndLogin;
        private readonly BackgroundWorker worker;
        private readonly MessengerViewModel messengerViewModel;
        private readonly NavigationStore navigationStore;
        private string userName;
        private string password;

        public MyICommand LoginCommand { get; set; }
        public MyICommand RegisterCommand { get; set; }

        public LoginViewModel(RegistrationAndLoginLogic registrationAndLogin, BackgroundWorker worker, MessengerViewModel messengerViewModel, NavigationStore navigationStore)
        {
            this.registrationAndLogin = registrationAndLogin;
            this.worker = worker;
            this.messengerViewModel = messengerViewModel;
            this.navigationStore = navigationStore;
            LoginCommand = new MyICommand(async () =>
            {
                await OnLoginAsync();
            }, CanLogin);
            RegisterCommand = new MyICommand(async () =>
            {
                await OnRegisterAsync();
            }, CanRegister);
        }

        public string UserName
        {
            get { return userName; }
            set { 
                userName = value;  
                RaisePropertyChanged(nameof(UserName));
                LoginCommand.RaiseCanExecuteChanged();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return password; }
            set { 
                password = value; 
                RaisePropertyChanged(nameof(Password));
                LoginCommand.RaiseCanExecuteChanged();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task OnLoginAsync()
        {
            if (UserName != null && Password != null)
            {
                if(await registrationAndLogin.LoginUserAsync(UserName, Password))
                {
                    UserName = null;
                    Password = null;
                    navigationStore.CurrentViewModel = messengerViewModel;
                    worker.RunWorkerAsync();
                    await messengerViewModel.GetGroups();
                    //var messageDialog = new MessengerView();
                    //messageDialog.Show();
                    //TODO Fenster öffnen und schließen
                }
                else
                {
                    MessageBox.Show("Wrong username or password.");
                    UserName = null;
                    Password = null;
                }

            }
        }

        private bool CanLogin() => UserName != null && Password != null;

        private async Task OnRegisterAsync()
        {
            if(UserName != null && Password != null)
            {
                if (await registrationAndLogin.RegisterUserAsync(UserName, Password))
                {
                    worker.RunWorkerAsync();
                    navigationStore.CurrentViewModel = messengerViewModel;
                }
            }           
        }

        private bool CanRegister() => UserName != null && Password != null;


    }
}
