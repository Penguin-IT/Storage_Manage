using StorageManage.ViewModels;
using StorageManage.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StorageManage.ViewModels
{
    public class ChiTietPXView
    {
        public int STT { get; set; }
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGiaXuat { get; set; }
        public decimal? ThanhTien { get; set; }
        public ChiTietPX ChiTiet { get; set; }
    }

    public class XuatHoaDon : BaseViewModel
    {
        private QLKEntities db = new QLKEntities();

        
        private ObservableCollection<SanPham> _danhSachSP;
        public ObservableCollection<SanPham> DanhSachSP
        {
            get { return _danhSachSP; }
            set { _danhSachSP = value; OnPropertyChanged("DanhSachSP"); }
        }

        // Sản phẩm đang được chọn trên ComboBox
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
                    DonGiaXuat = value.DonGia.HasValue ? (decimal)value.DonGia.Value : 0m;
                }
                else
                {
                    MaSP = string.Empty;
                    DonGiaXuat = 0m;
                }
            }
        }

        private ObservableCollection<ChiTietPXView> _dsCTSP;
        public ObservableCollection<ChiTietPXView> DSCTSP
        {
            get { return _dsCTSP; }
            set { _dsCTSP = value; OnPropertyChanged("DSCTSP"); }
        }

        private string _maPhieu;
        public string MaPhieu
        {
            get { return _maPhieu; }
            set { _maPhieu = value; OnPropertyChanged("MaPhieu"); }
        }

        private string _maDL;
        public string MaDL
        {
            get { return _maDL; }
            set { _maDL = value; OnPropertyChanged("MaDL"); }
        }

        private DateTime _ngayXuat;
        public DateTime NgayXuat
        {
            get { return _ngayXuat; }
            set { _ngayXuat = value; OnPropertyChanged("NgayXuat"); }
        }

        
        private string _maSP;
        public string MaSP
        {
            get { return _maSP; }
            set { _maSP = value; OnPropertyChanged("MaSP");
                if (!string.IsNullOrWhiteSpace(value)) {
                    var sp = db.SanPhams.FirstOrDefault(x => x.MaSP == value);
                    if (sp != null)
                    {
                        TenSanPham = sp.TenSP;
                        DonGiaXuat = sp.DonGia.HasValue?sp.DonGia.Value:0m;
                        if (_selectedSanPham == null || _selectedSanPham.MaSP != sp.MaSP)
                        {
                            SelectedSanPham = sp;
                        }
                    }
                    else
                    {
                        TenSanPham = "Không tìm thấy sản phẩm";
                        DonGiaXuat = 0m;
                    }
                }
                else
                {
                    TenSanPham = string.Empty;
                    DonGiaXuat = 0m;
                    SelectedSanPham = null;
                }
            }
        }

        private string _tenSanPham;
        public string TenSanPham
        {
            get { return _tenSanPham; }
            set { _tenSanPham = value; OnPropertyChanged("TenSanPham"); }
        }

        private decimal _donGiaXuat;
        public decimal DonGiaXuat
        {
            get { return _donGiaXuat; }
            set { _donGiaXuat = value; OnPropertyChanged("DonGiaXuat"); }
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

        private ChiTietPXView _selectedCTSP;
        public ChiTietPXView SelectedCTSP
        {
            get { return _selectedCTSP; }
            set { _selectedCTSP = value; OnPropertyChanged("SelectedCTSP"); }
        }

        public ICommand AddDetailCommand { get; set; }
        public ICommand SaveInvoiceCommand { get; set; }
        public ICommand DeleteDetailCommand { get; set; }
        public ICommand EditDetailCommand { get; set; }

        // Hàm khởi tạo: thiết lập giá trị mặc định và đăng ký các lệnh
        public XuatHoaDon()
        {
            NgayXuat = DateTime.Today;
            DSCTSP = new ObservableCollection<ChiTietPXView>();
            MaPhieu = GenerateReceiptCode();

           
            DanhSachSP = new ObservableCollection<SanPham>(db.SanPhams.ToList());

            AddDetailCommand = new RelayCommand(p => AddDetail());
            SaveInvoiceCommand = new RelayCommand(p => SaveInvoice());
            DeleteDetailCommand = new RelayCommand(p => DeleteDetail(p as ChiTietPXView));
            EditDetailCommand = new RelayCommand(p => LoadDetailToForm(p as ChiTietPXView));
        }

        // Hàm tự động tạo mã phiếu xuất theo định dạng PXxx
        private string GenerateReceiptCode()
        {
            var danhSachMa = db.PhieuXuats
                .Where(p => p.MaPX.StartsWith("PX"))
                .Select(p => p.MaPX)
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
            return $"PX{(soLonNhat + 1):D2}";
        }

        // Hàm thêm chi tiết sản phẩm vào danh sách hóa đơn
        private void AddDetail()
        {
            if (string.IsNullOrWhiteSpace(MaDL))
            {
                MessageBox.Show("Vui lòng nhập mã Đại Lý!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NgayXuat == default(DateTime) || NgayXuat > DateTime.Now)
            {
                MessageBox.Show("Ngày xuất không hợp lệ!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedSanPham == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SoLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DonGiaXuat < 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var chiTiet = new ChiTietPX
            {
                MaSP = this.MaSP,
                SoLuong = this.SoLuong,
                DonGiaNhap = this.DonGiaXuat
            };
            chiTiet.TinhThanhTien();

            var chiTietView = new ChiTietPXView
            {
                STT = DSCTSP.Count + 1,
                MaSP = this.MaSP,
                TenSP = SelectedSanPham.TenSP,
                SoLuong = this.SoLuong,
                DonGiaXuat = this.DonGiaXuat,
                ThanhTien = chiTiet.ThanhTien,
                ChiTiet = chiTiet
            };

            DSCTSP.Add(chiTietView);
            RecalculateTotal();
            ClearForm();
        }

        // Hàm tải thông tin dòng được chọn lên form để chỉnh sửa
        private void LoadDetailToForm(ChiTietPXView item)
        {
            if (item == null) return;

            SelectedCTSP = item;

         
            SelectedSanPham = DanhSachSP.FirstOrDefault(sp => sp.MaSP == item.MaSP);
            SoLuong = item.SoLuong ?? 0;
            DonGiaXuat = item.DonGiaXuat ?? 0m;

           
            AddDetailCommand = new RelayCommand(p => UpdateDetail());
            OnPropertyChanged("AddDetailCommand");
        }

        // Hàm cập nhật thông tin dòng đang được chọn sau khi chỉnh sửa trên form
        private void UpdateDetail()
        {
            if (SelectedCTSP == null) return;

            if (SelectedSanPham == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SoLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DonGiaXuat < 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

          
            SelectedCTSP.MaSP = this.MaSP;
            SelectedCTSP.TenSP = SelectedSanPham.TenSP;
            SelectedCTSP.SoLuong = this.SoLuong;
            SelectedCTSP.DonGiaXuat = this.DonGiaXuat;
            SelectedCTSP.ChiTiet.MaSP = this.MaSP;
            SelectedCTSP.ChiTiet.SoLuong = this.SoLuong;
            SelectedCTSP.ChiTiet.DonGiaNhap = this.DonGiaXuat;
            SelectedCTSP.ChiTiet.TinhThanhTien();
            SelectedCTSP.ThanhTien = SelectedCTSP.ChiTiet.ThanhTien;

            
            DSCTSP = new ObservableCollection<ChiTietPXView>(DSCTSP);
            RecalculateTotal();

            
            AddDetailCommand = new RelayCommand(p => AddDetail());
            OnPropertyChanged("AddDetailCommand");

            SelectedCTSP = null;
            ClearForm();

            MessageBox.Show("Cập nhật thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Hàm xóa dòng chi tiết được truyền vào qua CommandParameter
        private void DeleteDetail(ChiTietPXView item)
        {
            if (item == null) return;

            var ketQua = MessageBox.Show(
                $"Bạn có chắc muốn xóa sản phẩm [{item.MaSP}] - {item.TenSP}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua == MessageBoxResult.Yes)
            {
                DSCTSP.Remove(item);


                for (int i = 0; i < DSCTSP.Count; i++)
                    DSCTSP[i].STT = i + 1;

                RecalculateTotal();
                ClearForm();
            }
        }

        // Hàm tính lại tổng tiền từ toàn bộ các dòng chi tiết
        private void RecalculateTotal()
        {
            TongTien = DSCTSP.Sum(ct => ct.ThanhTien.HasValue ? ct.ThanhTien.Value : 0m);
        }

        // Hàm xóa trắng form nhập liệu
        private void ClearForm()
        {
            SelectedSanPham = null;
            MaSP = string.Empty;
            TenSanPham = string.Empty;
            SoLuong = 0;
            DonGiaXuat = 0m;
        }

        // Hàm lưu phiếu xuất và toàn bộ chi tiết vào cơ sở dữ liệu
        private void SaveInvoice()
        {
            if (DSCTSP == null || DSCTSP.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm nào trong hóa đơn!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var phieuXuat = new PhieuXuat
                {
                    MaPX = this.MaPhieu,
                    MaDL = this.MaDL,
                    NgayXuat = this.NgayXuat,
                    TongTien = this.TongTien
                };

                db.PhieuXuats.Add(phieuXuat);

                foreach (var view in DSCTSP)
                {
                    view.ChiTiet.MaPX = phieuXuat.MaPX;
                    db.ChiTietPXes.Add(view.ChiTiet);
                }

                db.SaveChanges();

                MessageBox.Show("Lưu hóa đơn thành công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DSCTSP = new ObservableCollection<ChiTietPXView>();
                MaPhieu = GenerateReceiptCode();
                MaDL = string.Empty;
                TongTien = 0m;
                NgayXuat = DateTime.Today;
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