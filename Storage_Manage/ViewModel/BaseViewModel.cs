using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Storage_Manage.ViewModels
{
    // Lớp này giúp thông báo cho giao diện (UI) cập nhật khi dữ liệu trong Code thay đổi
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}