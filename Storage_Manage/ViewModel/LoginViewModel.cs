using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Storage_Manage.Model;
using StorageManage.Models;

namespace Storage_Manage.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }

        public System.Windows.Input.ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand<PasswordBox>((p) => true, (p) => ExecuteLogin(p));
        }

        private void ExecuteLogin(PasswordBox p)
        {
            string password = p.Password;
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ!");
                return;
            }

            using (var db = new StorageManageEntities())
            {
                // Sử dụng Model TaiKhoan để tìm kiếm
                var account = db.TaiKhoans
                    .Where(x => x.TenDangNhap == Username && x.MatKhau == password)
                    .FirstOrDefault();

                if (account != null)
                {
                    // Lưu User vào hệ thống
                    Application.Current.Properties["LoggedInUser"] = Username;

                    // Lấy quyền từ Model NhanVien liên kết
                    string role = account.NhanVien?.NhomQuyen?.TenQuyen ?? "Nhân viên";

                    MainWindow main = new MainWindow(role);
                    main.Show();
                    CloseLoginWindow();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
                }
            }
        }

        private void CloseLoginWindow()
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item.DataContext == this) { item.Close(); break; }
            }
        }
    }
}