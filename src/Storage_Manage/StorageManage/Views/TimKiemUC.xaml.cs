using Storage_Manage.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Storage_Manage.Views
{
    public partial class TimKiemUC : UserControl
    {
        public TimKiemUC()
        {
            InitializeComponent();

            // Gán DataContext trực tiếp tại đây để fix lỗi XDG-0001
            this.DataContext = new SearchViewModel();
        }
    }
}
