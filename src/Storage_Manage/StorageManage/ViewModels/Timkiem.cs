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
using System.Data.Entity;

namespace Storage_Manage.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private ObservableCollection<PhieuNhap> _outputList;
        public ObservableCollection<PhieuNhap> OutputList
        {
            get => _outputList;
            set { _outputList = value; OnPropertyChanged(); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> SearchCriteria { get; set; }

        private string _selectedCriteria;
        public string SelectedCriteria
        {
            get => _selectedCriteria;
            set
            {
                _selectedCriteria = value;
                OnPropertyChanged();
                // Thông báo cho View cập nhật lại trạng thái ẩn/hiện của DatePicker
                OnPropertyChanged(nameof(IsTimeSearchSelected));
            }
        }

        // Thuộc tính mới để Binding Visibility trong XAML
        public bool IsTimeSearchSelected => SelectedCriteria == "Khoảng thời gian";

        private DateTime _fromDate = DateTime.Now.AddMonths(-1);
        public DateTime FromDate { get => _fromDate; set { _fromDate = value; OnPropertyChanged(); } }

        private DateTime _toDate = DateTime.Now;
        public DateTime ToDate { get => _toDate; set { _toDate = value; OnPropertyChanged(); } }

        public ICommand SearchCommand { get; set; }

        public SearchViewModel()
        {
            OutputList = new ObservableCollection<PhieuNhap>();
            SearchCriteria = new ObservableCollection<string> { "Khoảng thời gian", "Mã phiếu", "Nhân viên", "NCC" };
            SelectedCriteria = "Mã phiếu";

            SearchCommand = new RelayCommand(_ => { ExecuteSearch(); });
        }

        void ExecuteSearch()
        {
            using (var db = new QLKEntities())
            {
                var query = db.PhieuNhaps
                    .Include(x => x.NhaCungCap)
                    .Include(x => x.NhanVien)
                    .AsQueryable();

                switch (SelectedCriteria)
                {
                    case "Khoảng thời gian":
                        query = query.Where(x => x.NgayNhap >= FromDate && x.NgayNhap <= ToDate);
                        break;
                    case "Mã phiếu":
                        if (!string.IsNullOrEmpty(SearchText))
                            query = query.Where(x => x.MaPN.Contains(SearchText));
                        break;
                    case "Nhân viên":
                        if (!string.IsNullOrEmpty(SearchText))
                            query = query.Where(x => x.NhanVien.HoTen.Contains(SearchText));
                        break;
                    case "NCC":
                        if (!string.IsNullOrEmpty(SearchText))
                            query = query.Where(x => x.NhaCungCap.TenNCC.Contains(SearchText));
                        break;
                }

                OutputList = new ObservableCollection<PhieuNhap>(query.ToList());
            }
        }
    }
}