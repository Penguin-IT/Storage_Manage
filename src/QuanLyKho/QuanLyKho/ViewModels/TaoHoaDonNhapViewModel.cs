using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    public class ChiTietNhapUI : BaseViewModel
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        private int _soLuong;
        public int SoLuong { get => _soLuong; set { _soLuong = value; ThanhTien = _soLuong * DonGia; OnPropertyChanged(); } }
        public decimal DonGia { get; set; }
        private decimal _thanhTien;
        public decimal ThanhTien { get => _thanhTien; set { _thanhTien = value; OnPropertyChanged(); } }
    }

    public class TaoHoaDonNhapViewModel : BaseViewModel
    {
        private string _maPN;
        public string MaPN { get => _maPN; set { _maPN = value; OnPropertyChanged(); } }

        private DateTime _ngayLap;
        public DateTime NgayLap { get => _ngayLap; set { _ngayLap = value; OnPropertyChanged(); } }

        private NhaCungCap _selectedNCC;
        public NhaCungCap SelectedNCC { get => _selectedNCC; set { _selectedNCC = value; OnPropertyChanged(); } }

        private SanPham _selectedSP;
        public SanPham SelectedSP
        {
            get => _selectedSP;
            set
            {
                _selectedSP = value;
                if (_selectedSP != null)
                {
                    DonGiaNhap = _selectedSP.DonGia ?? 0;
                    SoLuong = 1;
                }
                OnPropertyChanged();
            }
        }

        private int _soLuong;
        public int SoLuong { get => _soLuong; set { _soLuong = value; TinhThanhTien(); OnPropertyChanged(); } }

        private decimal _donGiaNhap;
        public decimal DonGiaNhap { get => _donGiaNhap; set { _donGiaNhap = value; TinhThanhTien(); OnPropertyChanged(); } }

        private decimal _thanhTien;
        public decimal ThanhTien { get => _thanhTien; set { _thanhTien = value; OnPropertyChanged(); } }
        private decimal _tongTienPhieu;
        public decimal TongTienPhieu { get => _tongTienPhieu; set { _tongTienPhieu = value; OnPropertyChanged(); } }

        private void TinhThanhTien() => ThanhTien = SoLuong * DonGiaNhap;

        public ObservableCollection<NhaCungCap> DanhSachNCC { get; set; }
        public ObservableCollection<SanPham> DanhSachSP { get; set; }
        public ObservableCollection<ChiTietNhapUI> DanhSachChiTiet { get; set; }

        public ICommand AddDetailCommand { get; set; }
        public ICommand RemoveDetailCommand { get; set; }
        public ICommand EditDetailCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public TaoHoaDonNhapViewModel()
        {
            NgayLap = DateTime.Now;
            DanhSachNCC = new ObservableCollection<NhaCungCap>();
            DanhSachSP = new ObservableCollection<SanPham>();
            DanhSachChiTiet = new ObservableCollection<ChiTietNhapUI>();

            LoadData();

            AddDetailCommand = new RelayCommand(ExecuteAddDetail, CanAddDetail);
            RemoveDetailCommand = new RelayCommand(ExecuteRemoveDetail);
            EditDetailCommand = new RelayCommand(ExecuteEditDetail);
            SaveCommand = new RelayCommand(ExecuteSave, CanSave);
        }
        private void TinhTongTienPhieu()
        {
            TongTienPhieu = DanhSachChiTiet.Sum(x => x.ThanhTien);
        }
        private void LoadData()
        {
            using (var db = new QL_KHOEntities3())
            {
                MaPN = TaoMaTuDong("PN", db);

                var nccs = db.NhaCungCaps.Where(x => x.IsActive == true).ToList();
                DanhSachNCC.Clear();
                foreach (var n in nccs) DanhSachNCC.Add(n);

                var sps = db.SanPhams.Where(x => x.IsActive == true).ToList();
                DanhSachSP.Clear();
                foreach (var s in sps) DanhSachSP.Add(s);
            }
        }

        private string TaoMaTuDong(string prefix, QL_KHOEntities3 db)
        {
            string maCuoi = db.PhieuNhaps.OrderByDescending(x => x.MaPN).Select(x => x.MaPN).FirstOrDefault();
            if (string.IsNullOrEmpty(maCuoi)) return prefix + "00000001";

            string phanSo = maCuoi.Substring(2).Trim();
            if (int.TryParse(phanSo, out int soMoi)) return prefix + (soMoi + 1).ToString("D8");
            return prefix + "00000001";
        }

        private bool CanAddDetail(object obj) => SelectedSP != null && SoLuong > 0 && DonGiaNhap >= 0;

        private void ExecuteAddDetail(object obj)
        {
            var exist = DanhSachChiTiet.FirstOrDefault(x => x.MaSP == SelectedSP.MaSP);
            if (exist != null) exist.SoLuong += SoLuong;
            else
            {
                DanhSachChiTiet.Add(new ChiTietNhapUI
                {
                    MaSP = SelectedSP.MaSP,
                    TenSP = SelectedSP.TenSP,
                    SoLuong = SoLuong,
                    DonGia = DonGiaNhap,
                    ThanhTien = ThanhTien
                });
            }
            SelectedSP = null; SoLuong = 0; DonGiaNhap = 0; ThanhTien = 0;
            TinhTongTienPhieu();
        }

        private void ExecuteEditDetail(object obj)
        {
            if (obj is ChiTietNhapUI item)
            {
            
                SelectedSP = DanhSachSP.FirstOrDefault(x => x.MaSP == item.MaSP);
                SoLuong = item.SoLuong;
                DonGiaNhap = item.DonGia;

               
                DanhSachChiTiet.Remove(item);
                TinhTongTienPhieu();
            }
        }

        private void ExecuteRemoveDetail(object obj)
        {
            if (obj is ChiTietNhapUI item)
            {
                DanhSachChiTiet.Remove(item);
                TinhTongTienPhieu();
            }
        }

        private bool CanSave(object obj) => SelectedNCC != null && DanhSachChiTiet.Count > 0;

        private void ExecuteSave(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                try
                {
                    string maMoi = TaoMaTuDong("PN", db);

                    var pn = new PhieuNhap
                    {
                        MaPN = maMoi,
                        NgayNhap = NgayLap,
                        MaNCC = SelectedNCC.MaNCC,
                       
                        MaNV = DangNhapViewModel.CurrentUser != null ? DangNhapViewModel.CurrentUser.MaNV : "NV01",
                        TongTien = 0
                    };
                    db.PhieuNhaps.Add(pn);

                    foreach (var item in DanhSachChiTiet)
                    {
                        db.ChiTietPNs.Add(new ChiTietPN
                        {
                            MaPN = maMoi,
                            MaSP = item.MaSP,
                            SoLuong = item.SoLuong,
                            DonGiaNhap = item.DonGia,
                            ThanhTien = item.ThanhTien
                        });
                    }

                    db.SaveChanges();
                    MessageBox.Show("Tạo phiếu nhập thành công! Mã phiếu: " + maMoi, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    DanhSachChiTiet.Clear(); SelectedNCC = null;
                    DanhSachChiTiet.Clear();
                    SelectedNCC = null; 
                    LoadData();

                    
                    TongTienPhieu = 0;
                }
                catch (DbEntityValidationException ex)
                {
                    string errorMsgs = "Dữ liệu chưa hợp lệ:\n";
                    foreach (var validationErrors in ex.EntityValidationErrors)
                        foreach (var validationError in validationErrors.ValidationErrors)
                            errorMsgs += $"- Cột [{validationError.PropertyName}]: {validationError.ErrorMessage}\n";
                    MessageBox.Show(errorMsgs, "Lỗi Validation", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}