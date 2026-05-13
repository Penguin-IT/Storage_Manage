using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Storage_Manage.ViewModels
{
    public class StatisticViewModel : BaseViewModel
    {
        private DateTime _fromDate = DateTime.Now.AddDays(-7);
        public DateTime FromDate
        {
            get => _fromDate;
            set { _fromDate = value; OnPropertyChanged("FromDate"); }
        }

        private double _totalRevenue;
        public double TotalRevenue
        {
            get => _totalRevenue;
            set { _totalRevenue = value; OnPropertyChanged("ToTalRevenue"); }
        }

        public ICommand StatisticCommand { get; set; }

        public StatisticViewModel()
        {
            TotalRevenue = 0;
           
        }
    }
}
