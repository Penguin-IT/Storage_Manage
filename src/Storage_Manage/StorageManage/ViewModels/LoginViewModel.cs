using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StorageManage.Models;
using StorageManage;
using Storage_Manage; // Bổ sung để gọi được MainWindow

namespace StorageManage.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(nameof(Username)); } }

        public System.Windows.Input.ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand((p) => ExecuteLogin(p as PasswordBox));
        }

        private void ExecuteLogin(PasswordBox p)
        {
            string password = p.Password;
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ!");
                return;
            }

            using (var db = new QLKEntities())
            {
                var account = db.TaiKhoans
                    .Where(x => x.TenDangNhap == Username && x.MatKhau == password)
                    .FirstOrDefault();

                if (account != null)
                {
                    Application.Current.Properties["LoggedInUser"] = Username;

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