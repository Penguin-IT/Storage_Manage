using System.Windows;
using StorageManage.ViewModels; // Cần có dòng này để gọi MainViewModel

namespace Storage_Manage
{
    public partial class MainWindow : Window
    {
        public string CurrentRole { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.CurrentRole = "Nhân viên";

            // GẮN DATA CONTEXT: Để các Binding Command trong XAML hoạt động
            this.DataContext = new MainViewModel(this.CurrentRole);

            ApplyPermissions();
        }

        public MainWindow(string role)
        {
            InitializeComponent();
            this.CurrentRole = role;

            // GẮN DATA CONTEXT: Truyền Role vào bộ não xử lý
            this.DataContext = new MainViewModel(this.CurrentRole);

            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            if (CurrentRole == "Nhân viên")
            {
                this.Title = "Quản lý kho - (Quyền: Nhân viên)";
                // PHẢI BỎ DẤU // Ở DÒNG DƯỚI ĐÂY:
                btnAccountManage.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Title = "Quản lý kho - (Quyền: Quản lý)";
                btnAccountManage.Visibility = Visibility.Visible;
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            StorageManage.Views.Login_UI login = new StorageManage.Views.Login_UI();
            login.Show();
            this.Close();
        }
    }
}