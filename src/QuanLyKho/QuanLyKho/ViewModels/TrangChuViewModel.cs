using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    public class GiaoDichDTO
    {
        public string MaPhieu { get; set; }
        public string LoaiGiaoDich { get; set; }
        public string DoiTac { get; set; }
        public DateTime NgayGiaoDich { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
    }

    public class TrangChuViewModel : BaseViewModel
    {
        private int _tongDonNhap;
        public int TongDonNhap { get => _tongDonNhap; set { _tongDonNhap = value; OnPropertyChanged(); } }

        private int _tongDonXuat;
        public int TongDonXuat { get => _tongDonXuat; set { _tongDonXuat = value; OnPropertyChanged(); } }

        private int _canhBaoTonKho;
        public int CanhBaoTonKho { get => _canhBaoTonKho; set { _canhBaoTonKho = value; OnPropertyChanged(); } }

        private decimal _doanhThuThang;
        public decimal DoanhThuThang { get => _doanhThuThang; set { _doanhThuThang = value; OnPropertyChanged(); } }

        public ObservableCollection<GiaoDichDTO> DanhSachGiaoDich { get; set; }
        public ICommand RefreshCommand { get; set; }

        public TrangChuViewModel()
        {
            DanhSachGiaoDich = new ObservableCollection<GiaoDichDTO>();
            RefreshCommand = new RelayCommand(o => LoadTongQuan());
            LoadTongQuan();
        }

        private void LoadTongQuan()
        {
            using (var db = new QL_KHOEntities3())
            {
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

       
                TongDonNhap = db.PhieuNhaps.Count(x => x.NgayNhap.HasValue && x.NgayNhap.Value.Month == currentMonth && x.NgayNhap.Value.Year == currentYear);

                TongDonXuat = db.PhieuXuats.Count(x => x.NgayXuat.HasValue && x.NgayXuat.Value.Month == currentMonth && x.NgayXuat.Value.Year == currentYear);

                CanhBaoTonKho = db.SanPhams.Count(x => x.SLTon.HasValue && x.SLTon.Value < 10);

                var dsPhieuXuatThang = db.PhieuXuats.Where(x => x.NgayXuat.HasValue && x.NgayXuat.Value.Month == currentMonth && x.NgayXuat.Value.Year == currentYear).ToList();
                DoanhThuThang = dsPhieuXuatThang.Any() ? dsPhieuXuatThang.Sum(x => x.TongTien).GetValueOrDefault() : 0;

                DanhSachGiaoDich.Clear();

   
                var listXuat = db.PhieuXuats
                    .OrderByDescending(x => x.NgayXuat)
                    .Take(5)
                    .Select(x => new GiaoDichDTO
                    {
                        MaPhieu = x.MaPX,
                        LoaiGiaoDich = "Phiếu Xuất",
                        DoiTac = x.DaiLy.TenDL,
                        NgayGiaoDich = x.NgayXuat ?? DateTime.Now,
                        TongTien = x.TongTien ?? 0,
                        TrangThai = "Hoàn thành"
                    }).ToList();

             
                var listNhap = db.PhieuNhaps
                    .OrderByDescending(x => x.NgayNhap)
                    .Take(5)
                    .Select(x => new GiaoDichDTO
                    {
                        MaPhieu = x.MaPN,
                        LoaiGiaoDich = "Phiếu Nhập",
                        DoiTac = x.NhaCungCap.TenNCC,
                        NgayGiaoDich = x.NgayNhap ?? DateTime.Now,
                        TongTien = x.TongTien ?? 0,
                        TrangThai = "Hoàn thành"
                    }).ToList();

             
                var listGiaoDich = listXuat.Concat(listNhap).OrderByDescending(x => x.NgayGiaoDich).Take(10).ToList();

                foreach (var item in listGiaoDich)
                {
                    DanhSachGiaoDich.Add(item);
                }
            }
        }
    }
}