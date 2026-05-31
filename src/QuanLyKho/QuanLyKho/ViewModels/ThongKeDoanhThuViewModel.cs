using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
  
    public class ThongKeUI
    {
        public string TenNhom { get; set; } 
        public int SoLuongHoaDon { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class ThongKeDoanhThuViewModel : BaseViewModel
    {
        #region Properties
        private DateTime _tuNgay;
        public DateTime TuNgay { get => _tuNgay; set { _tuNgay = value; OnPropertyChanged(); } }

        private DateTime _denNgay;
        public DateTime DenNgay { get => _denNgay; set { _denNgay = value; OnPropertyChanged(); } }

        public ObservableCollection<string> DanhSachTieuChi { get; set; }

        private string _tieuChiGomNhom;
        public string TieuChiGomNhom { get => _tieuChiGomNhom; set { _tieuChiGomNhom = value; OnPropertyChanged(); } }

        private decimal _tongDoanhThu;
        public decimal TongDoanhThu { get => _tongDoanhThu; set { _tongDoanhThu = value; OnPropertyChanged(); } }

        public ObservableCollection<ThongKeUI> DanhSachThongKe { get; set; }
        public ICommand ThongKeCommand { get; set; }
        #endregion

        public ThongKeDoanhThuViewModel()
        {
           
            TuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DenNgay = DateTime.Now;

            
            DanhSachTieuChi = new ObservableCollection<string>
            {
                "Gom nhóm theo Ngày",
                "Gom nhóm theo Đại Lý"
            };
            TieuChiGomNhom = "Gom nhóm theo Ngày"; 

            DanhSachThongKe = new ObservableCollection<ThongKeUI>();
            ThongKeCommand = new RelayCommand(ExecuteThongKe);

           
            ExecuteThongKe(null);
        }

        private void ExecuteThongKe(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
               
                DateTime startDate = TuNgay.Date;
                DateTime endDate = DenNgay.Date.AddDays(1).AddSeconds(-1);

                var phieuXuats = db.PhieuXuats.Include("DaiLy")
                                   .Where(x => x.NgayXuat >= startDate && x.NgayXuat <= endDate)
                                   .ToList();

                DanhSachThongKe.Clear();
                decimal tongTien = 0;

             
                if (TieuChiGomNhom == "Gom nhóm theo Ngày")
                {
                    var groupedByDate = phieuXuats
                        .GroupBy(x => x.NgayXuat.HasValue ? x.NgayXuat.Value.Date : DateTime.MinValue)
                        .OrderBy(g => g.Key)
                        .ToList();

                    foreach (var group in groupedByDate)
                    {
                        var doanhThuNhom = group.Sum(x => x.TongTien ?? 0);
                        tongTien += doanhThuNhom;

                        DanhSachThongKe.Add(new ThongKeUI
                        {
                            TenNhom = group.Key.ToString("dd/MM/yyyy"),
                            SoLuongHoaDon = group.Count(),
                            DoanhThu = doanhThuNhom
                        });
                    }
                }
                else if (TieuChiGomNhom == "Gom nhóm theo Đại Lý")
                {
                    var groupedByDaiLy = phieuXuats
                        .GroupBy(x => x.DaiLy != null ? x.DaiLy.TenDL : "Khách Lẻ")
                        .OrderBy(g => g.Key)
                        .ToList();

                    foreach (var group in groupedByDaiLy)
                    {
                        var doanhThuNhom = group.Sum(x => x.TongTien ?? 0);
                        tongTien += doanhThuNhom;

                        DanhSachThongKe.Add(new ThongKeUI
                        {
                            TenNhom = group.Key, 
                            SoLuongHoaDon = group.Count(),
                            DoanhThu = doanhThuNhom
                        });
                    }
                }

                
                TongDoanhThu = tongTien;
            }
        }
    }
}