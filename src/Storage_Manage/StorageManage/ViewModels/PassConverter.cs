using System;
using System.Globalization;
using System.Windows.Data;

namespace StorageManage.ViewModels
{
    // Phải có PUBLIC và kế thừa IMultiValueConverter
    public class PassConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Trả về mảng để ViewModel xử lý
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}