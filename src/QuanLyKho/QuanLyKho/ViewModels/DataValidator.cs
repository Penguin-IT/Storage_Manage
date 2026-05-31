using System.Linq;

namespace QuanLyKho.ViewModels
{
    public static class DataValidator
    {
        public static string KiemTraRong(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return $"{fieldName} không được để trống.";
            return null;
        }

        public static string KiemTraChieuDai(string value, string fieldName, int maxLength)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > maxLength)
                return $"{fieldName} không được vượt quá {maxLength} ký tự.";
            return null;
        }

        public static string KiemTraSoDienThoai(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && !value.All(char.IsDigit))
                return "Số điện thoại chỉ được chứa chữ số.";
            return null;
        }

        public static string KiemTraMatKhau(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Mật khẩu không được để trống.";
            if (password.Length < 6)
                return "Mật khẩu phải có ít nhất 6 ký tự.";
            return null;
        }
    }
}