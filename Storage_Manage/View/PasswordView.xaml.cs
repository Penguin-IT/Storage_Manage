using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity; // Sau khi làm Bước 1, dòng này sẽ hết lỗi
using StorageManage.Models; // Đảm bảo đúng namespace của file TaiKhoan.cs

namespace Storage_Manage.View
{
    public partial class PasswordView : UserControl
    {
        public PasswordView()
        {
            InitializeComponent();
        }

        private void btnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldP = oldPassBox.Password;
            string newP = newPassBox.Password;
            string confirmP = confirmPassBox.Password;

            if (string.IsNullOrEmpty(oldP) || string.IsNullOrEmpty(newP))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (newP != confirmP)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            try
            {
                // QUAN TRỌNG: Dùng StorageManageEntities (lớp bạn đã sửa) 
                // thay vì DbContext thuần để tránh lỗi "not part of model"
                using (var db = new StorageManageEntities())
                {
                    string user = Application.Current.Properties["LoggedInUser"]?.ToString();

                    // Truy cập thông qua DbSet TaiKhoans đã khai báo trong Entities
                    var account = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == user);

                    if (account != null)
                    {
                        if (account.MatKhau == oldP)
                        {
                            account.MatKhau = newP;
                            db.SaveChanges();
                            MessageBox.Show("Đổi mật khẩu thành công!");

                            // Xóa trống các ô nhập sau khi thành công
                            oldPassBox.Password = "";
                            newPassBox.Password = "";
                            confirmPassBox.Password = "";
                        }
                        else
                        {
                            MessageBox.Show("Mật khẩu cũ không chính xác!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin tài khoản!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
            }
        }
    }
}