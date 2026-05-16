using System.Windows;
using System.Linq;
using StorageManage.Models; // Đảm bảo đúng namespace đã thống nhất

namespace Storage_Manage.ViewModels
{
    public class PasswordViewModel : BaseViewModel
    {
        // Các thuộc tính để Binding với UI
        private string _oldPassword;
        public string OldPassword { get => _oldPassword; set { _oldPassword = value; OnPropertyChanged(); } }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(); } }

        private string _confirmPassword;
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }

        public System.Windows.Input.ICommand ChangePasswordCommand { get; set; }

        public PasswordViewModel()
        {
            ChangePasswordCommand = new RelayCommand<object>((p) => true, (p) => {
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

            using (var db = new StorageManageEntities())
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
                        // Xóa trắng ô nhập sau khi xong
                        OldPassword = NewPassword = ConfirmPassword = "";
                    }
                    else { MessageBox.Show("Mật khẩu cũ không chính xác!"); }
                }
            }
        }
    }
}