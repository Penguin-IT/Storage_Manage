using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using StorageManage.ViewModels;
using StorageManage.Views;

namespace StorageManage.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        public ICommand HomeViewCommand { get; set; }
        public ICommand AccountViewCommand { get; set; }
        public ICommand PasswordViewCommand { get; set; }

        public AccountViewModel AccountVM { get; set; }
        public PasswordViewModel PasswordVM { get; set; }

        public MainViewModel()
        {
            AccountVM = new AccountViewModel();
            PasswordVM = new PasswordViewModel();

            CurrentView = null;

            HomeViewCommand = new RelayCommand((p) => {
                CurrentView = null;
            });

            AccountViewCommand = new RelayCommand((p) => {
                CurrentView = AccountVM;
            });

            PasswordViewCommand = new RelayCommand((p) => {
                CurrentView = PasswordVM;
            });
        }
    }
}