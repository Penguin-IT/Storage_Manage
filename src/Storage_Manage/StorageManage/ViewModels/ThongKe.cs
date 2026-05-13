using StorageManage.Models;
using StorageManage.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StorageManage.ViewModels
{
    public class StatisticViewModel:BaseViewModel
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
        private ObservableCollection<PhieuXuat> _outputList;
        public ObservableCollection<PhieuXuat> OutputList
        {
            get => _outputList;
            set
            {
                _outputList = value;
                OnPropertyChanged("OutputList");
            }
        }

        public ICommand StatisticCommand { get; set; }

        public StatisticViewModel()
        {
            OutputList = new ObservableCollection<PhieuXuat>();


            StatisticCommand = new RelayCommand((p) => CalculateStatistic());


            CalculateStatistic();
        }

        void CalculateStatistic()
        {
            using (var db = new QLKEntities())
            {

                var data = db.PhieuXuats.Where(x => x.NgayXuat >= FromDate).ToList();

                OutputList = new ObservableCollection<PhieuXuat>(data);


                TotalRevenue = (double)data.Sum(x => x.TongTien ?? 0);
            }
        }
    }
}