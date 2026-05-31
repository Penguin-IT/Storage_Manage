using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    
    public class ChungTuUI
    {
        public string LoaiPhieu { get; set; }
        public string MaPhieu { get; set; }
        public DateTime NgayLap { get; set; }
        public string TenNhanVien { get; set; }
        public string DoiTac { get; set; }
        public decimal TongTien { get; set; }
    }

    public class TimKiemChungTuViewModel : BaseViewModel
    {
      
        public ObservableCollection<string> DanhSachLoaiChungTu { get; set; }

        private string _loaiChungTuLoc;
        public string LoaiChungTuLoc { get => _loaiChungTuLoc; set { _loaiChungTuLoc = value; OnPropertyChanged(); } }

        private DateTime _tuNgay;
        public DateTime TuNgay { get => _tuNgay; set { _tuNgay = value; OnPropertyChanged(); } }

        private DateTime _denNgay;
        public DateTime DenNgay { get => _denNgay; set { _denNgay = value; OnPropertyChanged(); } }

        private string _tuKhoa;
        public string TuKhoa { get => _tuKhoa; set { _tuKhoa = value; OnPropertyChanged(); } }

        public ObservableCollection<ChungTuUI> DanhSachChungTu { get; set; }

        public ICommand SearchCommand { get; set; }
        

        public TimKiemChungTuViewModel()
        {
       
            DanhSachLoaiChungTu = new ObservableCollection<string>
            {
                "Tất cả",
                "Phiếu Nhập",
                "Phiếu Xuất",
                "Phiếu Kiểm Kê"
            };
            LoaiChungTuLoc = "Tất cả"; 

           
            TuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DenNgay = DateTime.Now;

            DanhSachChungTu = new ObservableCollection<ChungTuUI>();
            SearchCommand = new RelayCommand(ExecuteSearch);

          
            ExecuteSearch(null);
        }

        private void ExecuteSearch(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                var ketQua = new List<ChungTuUI>();
                string keyword = string.IsNullOrWhiteSpace(TuKhoa) ? "" : TuKhoa.Trim().ToLower();

             
                DateTime endDate = DenNgay.Date.AddDays(1).AddSeconds(-1);
                DateTime startDate = TuNgay.Date;

               
                if (LoaiChungTuLoc == "Tất cả" || LoaiChungTuLoc == "Phiếu Nhập")
                {
                    var phieuNhaps = db.PhieuNhaps.Include("NhanVien").Include("NhaCungCap")
                        .Where(x => x.NgayNhap >= startDate && x.NgayNhap <= endDate).ToList();

                    foreach (var pn in phieuNhaps)
                    {
                        if (string.IsNullOrEmpty(keyword) ||
                            pn.MaPN.ToLower().Contains(keyword) ||
                            (pn.NhaCungCap != null && pn.NhaCungCap.TenNCC.ToLower().Contains(keyword)))
                        {
                            ketQua.Add(new ChungTuUI
                            {
                                LoaiPhieu = "Phiếu Nhập",
                                MaPhieu = pn.MaPN,
                                NgayLap = pn.NgayNhap ?? DateTime.MinValue,
                                TenNhanVien = pn.NhanVien != null ? pn.NhanVien.HoTen : "Không xác định", 
                                DoiTac = pn.NhaCungCap != null ? pn.NhaCungCap.TenNCC : "",
                                TongTien = pn.TongTien ?? 0
                            });
                        }
                    }
                }

                
                if (LoaiChungTuLoc == "Tất cả" || LoaiChungTuLoc == "Phiếu Xuất")
                {
                    var phieuXuats = db.PhieuXuats.Include("NhanVien").Include("DaiLy")
                        .Where(x => x.NgayXuat >= startDate && x.NgayXuat <= endDate).ToList();

                    foreach (var px in phieuXuats)
                    {
                        if (string.IsNullOrEmpty(keyword) ||
                            px.MaPX.ToLower().Contains(keyword) ||
                            (px.DaiLy != null && px.DaiLy.TenDL.ToLower().Contains(keyword)))
                        {
                            ketQua.Add(new ChungTuUI
                            {
                                LoaiPhieu = "Phiếu Xuất",
                                MaPhieu = px.MaPX,
                                NgayLap = px.NgayXuat ?? DateTime.MinValue,
                                TenNhanVien = px.NhanVien != null ? px.NhanVien.HoTen : "Không xác định",
                                DoiTac = px.DaiLy != null ? px.DaiLy.TenDL : "",
                                TongTien = px.TongTien ?? 0
                            });
                        }
                    }
                }

              
                if (LoaiChungTuLoc == "Tất cả" || LoaiChungTuLoc == "Phiếu Kiểm Kê")
                {
                    var phieuKiemKes = db.PhieuKiemKes.Include("NhanVien")
                        .Where(x => x.NgayKiemKe >= startDate && x.NgayKiemKe <= endDate).ToList();

                    foreach (var pk in phieuKiemKes)
                    {
                        if (string.IsNullOrEmpty(keyword) || pk.MaPKK.ToLower().Contains(keyword))
                        {
                            ketQua.Add(new ChungTuUI
                            {
                                LoaiPhieu = "Phiếu Kiểm Kê",
                                MaPhieu = pk.MaPKK,
                                NgayLap = pk.NgayKiemKe ?? DateTime.MinValue,
                                TenNhanVien = pk.NhanVien != null ? pk.NhanVien.HoTen : "Không xác định", 
                                DoiTac = "Kiểm Kê Nội Bộ",
                                TongTien = 0 
                            });
                        }
                    }
                }

                // Sắp xếp dữ liệu mới nhất lên đầu (Giảm dần theo ngày)
                var ketQuaSapXep = ketQua.OrderByDescending(x => x.NgayLap).ToList();

                // Đổ ra giao diện DataGrid
                DanhSachChungTu.Clear();
                foreach (var item in ketQuaSapXep)
                {
                    DanhSachChungTu.Add(item);
                }
            }
        }
    }
}