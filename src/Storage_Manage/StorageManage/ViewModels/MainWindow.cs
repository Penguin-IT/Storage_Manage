using System.Windows.Input;
using StorageManage.Views; // Để gọi các UserControl (AccountView, PasswordView)

namespace StorageManage.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        // Các Command tương ứng với các nút trong MainWindow.xaml
        public ICommand HomeViewCommand { get; set; }
        public ICommand AccountViewCommand { get; set; }
        public ICommand PasswordViewCommand { get; set; }

        public MainViewModel(string role)
        {
            CurrentView = new PasswordView();

            
            AccountViewCommand = new RelayCommand((p) => {
                CurrentView = new AccountView();
            });

            PasswordViewCommand = new RelayCommand((p) => {
                CurrentView = new PasswordView();
            });
        }
    }
}