using StorageManage.ViewModels;
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

        private ObservableCollection<SanPham> _danhSachSP;

        public ObservableCollection<SanPham> DanhSachSP
        {
            get { return _danhSachSP; }
            set { _danhSachSP = value; OnPropertyChanged("DanhSachSP"); }
        }
        private SanPham _selectedSanPham;
        public SanPham SelectedSanPham
        {
            get { return _selectedSanPham; }
            set
            {
                _selectedSanPham = value;
                OnPropertyChanged("SelectedSanPham");


                if (value != null)
                {
                    MaSP = value.MaSP;
                    DonGiaNhap = value.DonGia.HasValue ? (decimal)value.DonGia.Value : 0m;
                }
                else
                {
                    MaSP = string.Empty;
                    DonGiaNhap = 0m;
                }
            }
        }

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
                        if (_selectedSanPham==null || _selectedSanPham.MaSP != sp.MaSP)
                        {
                            SelectedSanPham = sp;
                        }
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
            set
            {
                _selectedCTSP = value;
                OnPropertyChanged("SelectedCTSP");
            }
        }

      
        public ICommand AddDetailCommand { get; set; }
        public ICommand SaveInvoiceCommand { get; set; }
        public ICommand DeleteDetailCommand { get; set; }
        public ICommand EditDetailCommand { get; set; }

        // Hàm khởi tạo: thiết lập giá trị mặc định và đăng ký các lệnh
        public NhapHoaDon()
        {
            NgayNhap = DateTime.Today;
            DsCTSP = new ObservableCollection<ChiTietPNView>();
            MaPhieu = GenerateReceiptCode();
            DanhSachSP = new ObservableCollection<SanPham>(db.SanPhams.ToList());
            AddDetailCommand    = new RelayCommand(p => AddDetail());
            SaveInvoiceCommand  = new RelayCommand(p => SaveInvoice());
           
            DeleteDetailCommand = new RelayCommand(p => DeleteDetail(p as ChiTietPNView));
            EditDetailCommand   = new RelayCommand(p => LoadDetailToForm(p as ChiTietPNView));
        }

        // Hàm tự động tạo mã phiếu nhập theo định dạng PNxx
        private string GenerateReceiptCode()
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

        // Hàm thêm chi tiết sản phẩm vào danh sách hóa đơn
        private void AddDetail()
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
                MaSP       = this.MaSP,
                SoLuong    = this.SoLuong,
                DonGiaNhap = this.DonGiaNhap,
            };
            chiTiet.TinhThanhTien();

            var chiTietView = new ChiTietPNView
            {
                STT        = DsCTSP.Count + 1,
                MaSP       = this.MaSP,
                TenSP      = this.TenSanPham,
                SoLuong    = this.SoLuong,
                DonGiaNhap = this.DonGiaNhap,
                ThanhTien  = chiTiet.ThanhTien,
                ChiTiet    = chiTiet
            };

            DsCTSP.Add(chiTietView);
            RecalculateTotal();
            ClearForm();
        }

        // Hàm tải thông tin dòng được chọn lên form để chỉnh sửa
        private void LoadDetailToForm(ChiTietPNView item)
        {
            if (item == null) return;

            SelectedCTSP = item;
            MaSP         = item.MaSP;
            TenSanPham   = item.TenSP;
            SoLuong      = item.SoLuong ?? 0;
            DonGiaNhap   = item.DonGiaNhap ?? 0m;

         
            AddDetailCommand = new RelayCommand(p => UpdateDetail());
            OnPropertyChanged("AddDetailCommand");
        }

        // Hàm cập nhật thông tin dòng đang được chọn sau khi chỉnh sửa trên form
        private void UpdateDetail()
        {
            if (SelectedCTSP == null) return;

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

            
            SelectedCTSP.MaSP= this.MaSP;
            SelectedCTSP.TenSP= SelectedSanPham.TenSP;
            SelectedCTSP.SoLuong = this.SoLuong;
            SelectedCTSP.DonGiaNhap = this.DonGiaNhap;
            SelectedCTSP.ChiTiet.MaSP = this.MaSP;
            SelectedCTSP.ChiTiet.SoLuong= this.SoLuong;
            SelectedCTSP.ChiTiet.DonGiaNhap = this.DonGiaNhap;
            SelectedCTSP.ChiTiet.TinhThanhTien();
            SelectedCTSP.ThanhTien = SelectedCTSP.ChiTiet.ThanhTien;

          
            DsCTSP = new ObservableCollection<ChiTietPNView>(DsCTSP);

            RecalculateTotal();

          
            AddDetailCommand = new RelayCommand(p => AddDetail());
            OnPropertyChanged("AddDetailCommand");

            SelectedCTSP = null;
            ClearForm();

            MessageBox.Show("Cập nhật thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Hàm xóa dòng chi tiết được truyền vào qua CommandParameter
        private void DeleteDetail(ChiTietPNView item)
        {
            if (item == null) return;

            var ketQua = MessageBox.Show(
                $"Bạn có chắc muốn xóa sản phẩm [{item.MaSP}] - {item.TenSP}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua == MessageBoxResult.Yes)
            {
                DsCTSP.Remove(item);

              
                for (int i = 0; i < DsCTSP.Count; i++)
                    DsCTSP[i].STT = i + 1;

                RecalculateTotal();
                ClearForm();
            }
        }

        // Hàm tính lại tổng tiền từ toàn bộ các dòng chi tiết
        private void RecalculateTotal()
        {
            TongTien = DsCTSP.Sum(ct => ct.ThanhTien.HasValue ? ct.ThanhTien.Value : 0m);
        }

        // Hàm xóa trắng form nhập liệu
        private void ClearForm()
        {
            SelectedSanPham = null;
            MaSP       = string.Empty;
            TenSanPham = string.Empty;
            SoLuong    = 0;
            DonGiaNhap = 0m;
        }

        // Hàm lưu phiếu nhập và toàn bộ chi tiết vào cơ sở dữ liệu
        private void SaveInvoice()
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
                    MaPN      = this.MaPhieu,
                    MaNCC     = this.MaNCC,
                    NgayNhap  = this.NgayNhap,
                    TongTien  = this.TongTien
                };

                db.PhieuNhaps.Add(phieuNhap);

                foreach (var view in DsCTSP)
                {
                    view.ChiTiet.MaPN = phieuNhap.MaPN;
                    db.ChiTietPNs.Add(view.ChiTiet);
                }

                db.SaveChanges();

                MessageBox.Show("Lưu hóa đơn thành công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DsCTSP   = new ObservableCollection<ChiTietPNView>();
                MaPhieu  = GenerateReceiptCode();
                MaNCC    = string.Empty;
                TongTien = 0m;
                NgayNhap = DateTime.Today;
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}