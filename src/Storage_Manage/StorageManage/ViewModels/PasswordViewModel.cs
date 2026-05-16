using System.Windows;
using System.Linq;
using StorageManage.Models;

namespace StorageManage.ViewModels
{
    public class PasswordViewModel : BaseViewModel
    {
        private string _oldPassword;
        public string OldPassword { get => _oldPassword; set { _oldPassword = value; OnPropertyChanged(nameof(OldPassword)); } }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(nameof(NewPassword)); } }

        private string _confirmPassword;
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); } }

        public System.Windows.Input.ICommand ChangePasswordCommand { get; set; }

        public PasswordViewModel()
        {
            ChangePasswordCommand = new RelayCommand((p) => {
                ExecuteChangePassword();
            });
        }

        private void ExecuteChangePassword()
        {
            if (string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu mới không khớp!");
                return;
            }

            using (var db = new QLKEntities())
            {
                string userName = Application.Current.Properties["LoggedInUser"]?.ToString();
                var account = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == userName);

                if (account != null)
                {
                    if (account.MatKhau == OldPassword)
                    {
                        account.MatKhau = NewPassword;
                        db.SaveChanges();
                        MessageBox.Show("Đổi mật khẩu thành công!");
                        OldPassword = NewPassword = ConfirmPassword = "";
                    }
                    else { MessageBox.Show("Mật khẩu cũ không chính xác!"); }
                }
            }
        }
    }
}