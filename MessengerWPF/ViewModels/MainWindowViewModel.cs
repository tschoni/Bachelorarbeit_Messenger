using MessengerWPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;



        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        
        public MainWindowViewModel(NavigationStore navigationStore, LoginViewModel loginViewModel )
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _navigationStore.CurrentViewModel = loginViewModel;
        }        


        //public ViewModelBase CurrentViewModel 
        //{
        //    get { return currentViewModel; }
        //    set { 
        //        currentViewModel = value;
        //        RaisePropertyChanged(nameof(CurrentViewModel));
        //    }
        //}

        private void OnCurrentViewModelChanged()
        {
            RaisePropertyChanged(nameof(CurrentViewModel));
        }
    }
}




