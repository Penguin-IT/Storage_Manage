using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System; 
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    public class DangNhapViewModel : BaseViewModel
    {
        public static TaiKhoan CurrentUser { get; private set; }
        private readonly Action _onLoginSuccess;

        private string _tenDangNhap;
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set
            {
                _tenDangNhap = value;
                ClearErrors(nameof(TenDangNhap));
                string err = DataValidator.KiemTraRong(value, "Tên đăng nhập");
                if (err != null) AddError(nameof(TenDangNhap), err);
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; set; }

        public DangNhapViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, CanLogin);
        }
        public DangNhapViewModel(Action onLoginSuccess) : this() 
        {
            _onLoginSuccess = onLoginSuccess;
        }

        private bool CanLogin(object obj)
        {
            return !HasErrors && !string.IsNullOrWhiteSpace(TenDangNhap);
        }

        private void ExecuteLogin(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string matKhau = passwordBox?.Password;

          
            if (string.IsNullOrWhiteSpace(TenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new QL_KHOEntities3())
            {
               
                var user = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == TenDangNhap && x.MatKhau == matKhau);

                if (user != null)
                {
                    
                    if (user.TrangThai == true)
                    {
                        
                        CurrentUser = user;
                        _onLoginSuccess?.Invoke();
                        MessageBox.Show($"Xin chào {user.TenDangNhap}, đăng nhập thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        
                        MessageBox.Show("Tài khoản này đã bị khóa hoặc đã xóa khỏi hệ thống.\nVui lòng liên hệ Quản lý!", "Tài khoản bị khóa", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}