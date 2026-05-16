using System;
using System.Windows;
using Storage_Manage.ViewModels;

// 1. Sửa namespace cho khớp hoàn toàn với x:Class trong XAML
namespace Storage_Manage.View
{
    public partial class Login_UI : Window
    {
        public Login_UI()
        {
            // 2. Hàm này phải để hệ thống tự xử lý, KHÔNG ĐƯỢC tự viết lại ở dưới
            InitializeComponent();

            this.DataContext = new LoginViewModel();
        }

        // 3. Hàm xử lý nút thoát
        public void HandleExit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}