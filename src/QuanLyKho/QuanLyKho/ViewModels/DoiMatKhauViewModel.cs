using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    public class MultiPasswordConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DoiMatKhauViewModel : BaseViewModel
    {
        public ICommand UpdatePasswordCommand { get; set; }

        public DoiMatKhauViewModel()
        {
            UpdatePasswordCommand = new RelayCommand(ExecuteUpdate);
        }

        private void ExecuteUpdate(object obj)
        {
            var values = obj as object[];
            if (values == null || values.Length != 3) return;

            var pbHienTai = values[0] as PasswordBox;
            var pbMoi = values[1] as PasswordBox;
            var pbXacNhan = values[2] as PasswordBox;

            string matKhauHienTai = pbHienTai?.Password;
            string matKhauMoi = pbMoi?.Password;
            string xacNhan = pbXacNhan?.Password;

            if (string.IsNullOrWhiteSpace(matKhauHienTai) || string.IsNullOrWhiteSpace(matKhauMoi) || string.IsNullOrWhiteSpace(xacNhan))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các trường mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (matKhauMoi.Length < 3)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 3 ký tự!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (matKhauMoi != xacNhan)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DangNhapViewModel.CurrentUser == null) return;
            string userName = DangNhapViewModel.CurrentUser.TenDangNhap;

            using (var db = new QL_KHOEntities3())
            {
                var tk = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == userName && x.MatKhau == matKhauHienTai);
                if (tk != null)
                {
                    tk.MatKhau = matKhauMoi;
                    db.SaveChanges();

                    DangNhapViewModel.CurrentUser.MatKhau = matKhauMoi;
                    MessageBox.Show("Cập nhật mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (pbHienTai != null) pbHienTai.Password = "";
                    if (pbMoi != null) pbMoi.Password = "";
                    if (pbXacNhan != null) pbXacNhan.Password = "";
                }
                else
                {
                    MessageBox.Show("Mật khẩu hiện tại không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

 
    
}