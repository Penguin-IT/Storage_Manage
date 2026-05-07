using Storage_Manage.ViewModels;
using StorageManage.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StorageManage.ViewModels
{
    public class ChiTietPNView
    {
        public int STT { get; set; }
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public decimal? ThanhTien { get; set; }
        public ChiTietPN ChiTiet { get; set; }
    }

    public class NhapHoaDon : BaseViewModel
    {
        private QLKEntities db = new QLKEntities();

        private ObservableCollection<ChiTietPNView> _dsCTSP;
        public ObservableCollection<ChiTietPNView> DsCTSP
        {
            get { return _dsCTSP; }
            set { _dsCTSP = value; OnPropertyChanged("DsCTSP"); }
        }

        private string _maPhieu;
        public string MaPhieu
        {
            get { return _maPhieu; }
            set { _maPhieu = value; OnPropertyChanged("MaPhieu"); }
        }

        private string _maNCC;
        public string MaNCC
        {
            get { return _maNCC; }
            set { _maNCC = value; OnPropertyChanged("MaNCC"); }
        }

        private DateTime _ngayNhap;
        public DateTime NgayNhap
        {
            get { return _ngayNhap; }
            set { _ngayNhap = value; OnPropertyChanged("NgayNhap"); }
        }

        private string _maSP;
        public string MaSP
        {
            get { return _maSP; }
            set
            {
                _maSP = value;
                OnPropertyChanged("MaSP");

                if (!string.IsNullOrWhiteSpace(value))
                {
                    var sp = db.SanPhams.FirstOrDefault(x => x.MaSP == value);
                    if (sp != null)
                    {
                        TenSanPham = sp.TenSP;
                        DonGiaNhap = sp.DonGia.HasValue ? sp.DonGia.Value : 0m;
                    }
                    else
                    {
                        TenSanPham = "Không tìm thấy sản phẩm";
                        DonGiaNhap = 0m;
                    }
                }
                else
                {
                    TenSanPham = string.Empty;
                    DonGiaNhap = 0m;
                }
            }
        }

        private string _tenSanPham;
        public string TenSanPham
        {
            get { return _tenSanPham; }
            set { _tenSanPham = value; OnPropertyChanged("TenSanPham"); }
        }

        private decimal _donGiaNhap;
        public decimal DonGiaNhap
        {
            get { return _donGiaNhap; }
            set { _donGiaNhap = value; OnPropertyChanged("DonGiaNhap"); }
        }

        private int _soLuong;
        public int SoLuong
        {
            get { return _soLuong; }
            set { _soLuong = value; OnPropertyChanged("SoLuong"); }
        }

        private decimal _tongTien;
        public decimal TongTien
        {
            get { return _tongTien; }
            set { _tongTien = value; OnPropertyChanged("TongTien"); }
        }

        private ChiTietPNView _selectedCTSP;
        public ChiTietPNView SelectedCTSP
        {
            get { return _selectedCTSP; }
            set { _selectedCTSP = value; OnPropertyChanged("SelectedCTSP"); }
        }

        public ICommand ThemChiTietCommand { get; set; }
        public ICommand LuuHoaDonCommand { get; set; }

        public NhapHoaDon()
        {
            NgayNhap = DateTime.Today;
            DsCTSP = new ObservableCollection<ChiTietPNView>();
            MaPhieu = TaoMaPhieuTuDong();

            ThemChiTietCommand = new RelayCommand(p => ThemChiTiet());
            LuuHoaDonCommand = new RelayCommand(p => LuuHoaDon());
        }

        private string TaoMaPhieuTuDong()
        {
            var danhSachMa = db.PhieuNhaps
                .Where(p => p.MaPN.StartsWith("PN"))
                .Select(p => p.MaPN)
                .ToList();

            int soLonNhat = 0;
            foreach (var ma in danhSachMa)
            {
                string phanSo = ma.Substring(2);
                int soThuTu;
                if (int.TryParse(phanSo, out soThuTu))
                    if (soThuTu > soLonNhat)
                        soLonNhat = soThuTu;
            }

            return $"PN{(soLonNhat + 1):D2}";
        }

        private void ThemChiTiet()
        {
            if (string.IsNullOrWhiteSpace(MaNCC))
            {
                MessageBox.Show("Vui lòng nhập mã Nhà Cung Cấp!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NgayNhap == default(DateTime) || NgayNhap > DateTime.Now)
            {
                MessageBox.Show("Ngày nhập không hợp lệ!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(MaSP) || TenSanPham == "Không tìm thấy sản phẩm")
            {
                MessageBox.Show("Mã sản phẩm không hợp lệ!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SoLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DonGiaNhap < 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var chiTiet = new ChiTietPN
            {
                MaSP = this.MaSP,
                SoLuong = this.SoLuong,
                DonGiaNhap = this.DonGiaNhap,
            };
            chiTiet.TinhThanhTien();

            var chiTietView = new ChiTietPNView
            {
                STT = DsCTSP.Count + 1,
                MaSP = this.MaSP,
                TenSP = this.TenSanPham,
                SoLuong = this.SoLuong,
                DonGiaNhap = this.DonGiaNhap,
                ThanhTien = chiTiet.ThanhTien,
                ChiTiet = chiTiet
            };

            DsCTSP.Add(chiTietView);
            CapNhatTongTien();

            MaSP = string.Empty;
            TenSanPham = string.Empty;
            SoLuong = 0;
            DonGiaNhap = 0m;
        }

        private void CapNhatTongTien()
        {
            TongTien = DsCTSP.Sum(ct => ct.ThanhTien.HasValue ? ct.ThanhTien.Value : 0m);
        }

        private void LuuHoaDon()
        {
            if (DsCTSP == null || DsCTSP.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm nào trong hóa đơn!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var phieuNhap = new PhieuNhap
                {
                    MaPN = this.MaPhieu,
                    MaNCC = this.MaNCC,
                    NgayNhap = this.NgayNhap,
                    TongTien = this.TongTien
                };

                db.PhieuNhaps.Add(phieuNhap);
                db.SaveChanges();

                foreach (var view in DsCTSP)
                {
                    view.ChiTiet.MaPN = phieuNhap.MaPN;
                    db.ChiTietPNs.Add(view.ChiTiet);
                }

                db.SaveChanges();

                MessageBox.Show("Lưu hóa đơn thành công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DsCTSP = new ObservableCollection<ChiTietPNView>();
                MaPhieu = TaoMaPhieuTuDong();
                MaNCC = string.Empty;
                MaSP = string.Empty;
                TongTien = 0m;
                NgayNhap = DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}