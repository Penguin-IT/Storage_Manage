using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    // Class phụ để hứng dữ liệu lên giao diện
    public class TopNhaCungCapUI
    {
        public string TenNCC { get; set; }
        public decimal TongTien { get; set; }
        public double PhanTram { get; set; } // Dùng để quyết định chiều dài của thanh ngang màu xanh
    }

    public class BaoCaoPhanTichViewModel : BaseViewModel
    {
        #region Properties
        // Bộ lọc thời gian (ComboBox)
        public ObservableCollection<string> DanhSachKy { get; set; }
        private string _kyDuocChon;
        public string KyDuocChon
        {
            get => _kyDuocChon;
            set
            {
                _kyDuocChon = value;
                OnPropertyChanged();
                ExecuteLamMoi(null); // Tự động load lại dữ liệu khi đổi ComboBox
            }
        }

        public ObservableCollection<TopNhaCungCapUI> DanhSachTopNCC { get; set; }
        public ICommand LamMoiCommand { get; set; }
        #endregion

        public BaoCaoPhanTichViewModel()
        {
            DanhSachTopNCC = new ObservableCollection<TopNhaCungCapUI>();
            LamMoiCommand = new RelayCommand(ExecuteLamMoi);

            // Tự động tạo danh sách thời gian dựa vào thời gian thực tế của máy tính
            int namHienTai = DateTime.Now.Year;
            int thangHienTai = DateTime.Now.Month;
            int quyHienTai = (thangHienTai - 1) / 3 + 1;

            DanhSachKy = new ObservableCollection<string>
            {
                $"Tháng {thangHienTai}/{namHienTai}",
                $"Quý {quyHienTai}/{namHienTai}",
                $"Năm {namHienTai}",
                "Tất cả các năm"
            };

            _kyDuocChon = DanhSachKy[1]; 
            ExecuteLamMoi(null); 
        }

        private void ExecuteLamMoi(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;
                DateTime now = DateTime.Now;

             
                if (KyDuocChon.StartsWith("Tháng"))
                {
                    startDate = new DateTime(now.Year, now.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);
                }
                else if (KyDuocChon.StartsWith("Quý"))
                {
                    int currentQuarter = (now.Month - 1) / 3 + 1;
                    startDate = new DateTime(now.Year, 3 * currentQuarter - 2, 1);
                    endDate = startDate.AddMonths(3).AddDays(-1).AddHours(23).AddMinutes(59);
                }
                else if (KyDuocChon.StartsWith("Năm"))
                {
                    startDate = new DateTime(now.Year, 1, 1);
                    endDate = new DateTime(now.Year, 12, 31).AddHours(23).AddMinutes(59);
                }

                
                var query = db.PhieuNhaps.Include("NhaCungCap")
                    .Where(x => x.NgayNhap >= startDate && x.NgayNhap <= endDate);

                
                var topNCC = query
                    .GroupBy(x => x.NhaCungCap != null ? x.NhaCungCap.TenNCC : "Không xác định")
                    .Select(g => new
                    {
                        TenNCC = g.Key,
                        TongTien = g.Sum(x => x.TongTien ?? 0)
                    })
                    .OrderByDescending(x => x.TongTien)
                    .Take(5)
                    .ToList();

                DanhSachTopNCC.Clear();

              
                decimal maxTien = topNCC.Count > 0 ? topNCC.Max(x => x.TongTien) : 1;

                foreach (var item in topNCC)
                {
                    DanhSachTopNCC.Add(new TopNhaCungCapUI
                    {
                        TenNCC = item.TenNCC,
                        TongTien = item.TongTien,
                   
                        PhanTram = (double)(item.TongTien / (maxTien == 0 ? 1 : maxTien)) * 100
                    });
                }
            }
        }
    }
}