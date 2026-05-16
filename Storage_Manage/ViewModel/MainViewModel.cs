using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Storage_Manage.ViewModels;

namespace Storage_Manage.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        // 1. Quản lý View đang hiển thị
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        // 2. Các Command cho Menu
        public ICommand HomeViewCommand { get; set; }
        public ICommand AccountViewCommand { get; set; }
        public ICommand PasswordViewCommand { get; set; }

        // Khai báo các ViewModel con
        public AccountViewModel AccountVM { get; set; }
        public PasswordViewModel PasswordVM { get; set; }

        public MainViewModel()
        {
            // Khởi tạo các ViewModel con
            AccountVM = new AccountViewModel();
            PasswordVM = new PasswordViewModel();

            // QUAN TRỌNG: Thiết lập trang mặc định là TRANG TRẮNG (null)
            // Khi vừa đăng nhập, CurrentView sẽ trống, không hiện Đổi mật khẩu nữa.
            CurrentView = null;

            // Thiết lập lệnh điều hướng cho nút Trang Chủ
            HomeViewCommand = new RelayCommand<object>((p) => true, (p) => {
                CurrentView = null; // Bấm trang chủ thì hiện trang trắng
            });

            // Thiết lập lệnh điều hướng cho nút Quản lý tài khoản
            AccountViewCommand = new RelayCommand<object>((p) => true, (p) => {
                CurrentView = AccountVM;
            });

            // Thiết lập lệnh điều hướng cho nút Đổi mật khẩu
            PasswordViewCommand = new RelayCommand<object>((p) => true, (p) => {
                CurrentView = PasswordVM;
            });
        }
    }
}