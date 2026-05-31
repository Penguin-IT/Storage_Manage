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
    public class ChiTietXuatUI : BaseViewModel
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        private int _soLuong;
        public int SoLuong { get => _soLuong; set { _soLuong = value; ThanhTien = _soLuong * DonGia; OnPropertyChanged(); } }
        public decimal DonGia { get; set; }
        private decimal _thanhTien;
        public decimal ThanhTien { get => _thanhTien; set { _thanhTien = value; OnPropertyChanged(); } }
        
    }

    public class TaoHoaDonBanViewModel : BaseViewModel
    {
        private string _maPX;
        public string MaPX { get => _maPX; set { _maPX = value; OnPropertyChanged(); } }

        private DateTime _ngayLap;
        public DateTime NgayLap { get => _ngayLap; set { _ngayLap = value; OnPropertyChanged(); } }

        private DaiLy _selectedDL;
        public DaiLy SelectedDL { get => _selectedDL; set { _selectedDL = value; OnPropertyChanged(); } }

        private SanPham _selectedSP;
        public SanPham SelectedSP
        {
            get => _selectedSP;
            set
            {
                _selectedSP = value;
                if (_selectedSP != null)
                {
                    DonGiaXuat = _selectedSP.DonGia ?? 0;
                    SoLuong = 1;
                }
                OnPropertyChanged();
            }
        }

        private int _soLuong;
        public int SoLuong { get => _soLuong; set { _soLuong = value; TinhThanhTien(); OnPropertyChanged(); } }

        private decimal _donGiaXuat;
        public decimal DonGiaXuat { get => _donGiaXuat; set { _donGiaXuat = value; TinhThanhTien(); OnPropertyChanged(); } }

        private decimal _thanhTien;
        public decimal ThanhTien { get => _thanhTien; set { _thanhTien = value; OnPropertyChanged(); } }
        private decimal _tongTienPhieu;
        public decimal TongTienPhieu { get => _tongTienPhieu; set { _tongTienPhieu = value; OnPropertyChanged(); } }

        private void TinhThanhTien() => ThanhTien = SoLuong * DonGiaXuat;

        public ObservableCollection<DaiLy> DanhSachDL { get; set; }
        public ObservableCollection<SanPham> DanhSachSP { get; set; }
        public ObservableCollection<ChiTietXuatUI> DanhSachChiTiet { get; set; }

        public ICommand AddDetailCommand { get; set; }
        public ICommand RemoveDetailCommand { get; set; }
        public ICommand EditDetailCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public TaoHoaDonBanViewModel()
        {
            NgayLap = DateTime.Now;
            DanhSachDL = new ObservableCollection<DaiLy>();
            DanhSachSP = new ObservableCollection<SanPham>();
            DanhSachChiTiet = new ObservableCollection<ChiTietXuatUI>();

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
                MaPX = TaoMaTuDong("PX", db);

                var dls = db.DaiLies.Where(x => x.IsActive == true).ToList();
                DanhSachDL.Clear();
                foreach (var d in dls) DanhSachDL.Add(d);

                var sps = db.SanPhams.Where(x => x.IsActive == true).ToList();
                DanhSachSP.Clear();
                foreach (var s in sps) DanhSachSP.Add(s);
            }
        }

        private string TaoMaTuDong(string prefix, QL_KHOEntities3 db)
        {
            string maCuoi = db.PhieuXuats.OrderByDescending(x => x.MaPX).Select(x => x.MaPX).FirstOrDefault();
            if (string.IsNullOrEmpty(maCuoi)) return prefix + "00000001";

            string phanSo = maCuoi.Substring(2).Trim();
            if (int.TryParse(phanSo, out int soMoi)) return prefix + (soMoi + 1).ToString("D8");
            return prefix + "00000001";
        }

        private bool CanAddDetail(object obj) => SelectedSP != null && SoLuong > 0 && DonGiaXuat >= 0;

        private void ExecuteAddDetail(object obj)
        {
            var exist = DanhSachChiTiet.FirstOrDefault(x => x.MaSP == SelectedSP.MaSP);
            int tongSoLuong = SoLuong + (exist != null ? exist.SoLuong : 0);

            if (tongSoLuong > SelectedSP.SLTon)
            {
                MessageBox.Show($"Kho không đủ hàng! Tồn kho hiện tại: {SelectedSP.SLTon}", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (exist != null) exist.SoLuong += SoLuong;
            else
            {
                DanhSachChiTiet.Add(new ChiTietXuatUI
                {
                    MaSP = SelectedSP.MaSP,
                    TenSP = SelectedSP.TenSP,
                    SoLuong = SoLuong,
                    DonGia = DonGiaXuat,
                    ThanhTien = ThanhTien
                });
            }
            SelectedSP = null; SoLuong = 0; DonGiaXuat = 0; ThanhTien = 0;
            TinhTongTienPhieu();
        }

        private void ExecuteEditDetail(object obj)
        {
            if (obj is ChiTietXuatUI item)
            {
                SelectedSP = DanhSachSP.FirstOrDefault(x => x.MaSP == item.MaSP);
                SoLuong = item.SoLuong;
                DonGiaXuat = item.DonGia;
                DanhSachChiTiet.Remove(item);
                TinhTongTienPhieu();
            }
        }

        private void ExecuteRemoveDetail(object obj)
        {
            if (obj is ChiTietXuatUI item)
            {
                DanhSachChiTiet.Remove(item);
                TinhTongTienPhieu();
            }
        }

        private bool CanSave(object obj) => SelectedDL != null && DanhSachChiTiet.Count > 0;

        private void ExecuteSave(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                try
                {
                    string maMoi = TaoMaTuDong("PX", db);

                    var px = new PhieuXuat
                    {
                        MaPX = maMoi,
                        NgayXuat = NgayLap,
                        MaDL = SelectedDL.MaDL,
                        
                        MaNV = DangNhapViewModel.CurrentUser != null ? DangNhapViewModel.CurrentUser.MaNV : "NV01",
                        TongTien = 0
                    };
                    db.PhieuXuats.Add(px);

                    foreach (var item in DanhSachChiTiet)
                    {
                        db.ChiTietPXes.Add(new ChiTietPX
                        {
                            MaPX = maMoi,
                            MaSP = item.MaSP,
                            SoLuong = item.SoLuong,
                            DonGiaXuat = item.DonGia,
                            ThanhTien = item.ThanhTien
                        });
                    }

                    db.SaveChanges();
                    MessageBox.Show("Tạo phiếu xuất thành công! Mã phiếu: " + maMoi, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    DanhSachChiTiet.Clear();
                    SelectedDL = null; 
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
            }
        }
    }
}