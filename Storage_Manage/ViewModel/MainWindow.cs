using System.Windows.Input;
using Storage_Manage.View; // Để gọi các UserControl (AccountView, PasswordView)

namespace Storage_Manage.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        // Các Command tương ứng với các nút trong MainWindow.xaml
        public ICommand HomeViewCommand { get; set; }
        public ICommand AccountViewCommand { get; set; }
        public ICommand PasswordViewCommand { get; set; }

        public MainViewModel(string role)
        {
            // Mặc định khi mở app sẽ hiện trang Password (hoặc trang nào bạn thích)
            CurrentView = new PasswordView();

            // Xử lý khi nhấn nút Quản lý Tài khoản
            AccountViewCommand = new RelayCommand<object>((p) => true, (p) => {
                CurrentView = new AccountView();
            });

            // Xử lý khi nhấn nút Đổi mật khẩu
            PasswordViewCommand = new RelayCommand<object>((p) => true, (p) => {
                CurrentView = new PasswordView();
            });

            // Nếu có trang chủ, bạn tạo thêm:
            // HomeViewCommand = new RelayCommand<object>((p) => true, (p) => { CurrentView = new HomeView(); });
        }
    }
}