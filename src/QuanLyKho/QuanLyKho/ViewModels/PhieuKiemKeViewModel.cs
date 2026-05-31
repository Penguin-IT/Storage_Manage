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
    public class ChiTietKiemKeUI : BaseViewModel
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public int SLHeThong { get; set; }

        private int _slThucTe;
        public int SLThucTe { get => _slThucTe; set { _slThucTe = value; SLLech = _slThucTe - SLHeThong; OnPropertyChanged(); } }

        private int _slLech;
        public int SLLech { get => _slLech; set { _slLech = value; OnPropertyChanged(); } }

        public string LyDo { get; set; }
    }

    public class PhieuKiemKeViewModel : BaseViewModel
    {
        private string _maPKK;
        public string MaPKK { get => _maPKK; set { _maPKK = value; OnPropertyChanged(); } }

        private DateTime _ngayLap;
        public DateTime NgayLap { get => _ngayLap; set { _ngayLap = value; OnPropertyChanged(); } }

        private string _ghiChu;
        public string GhiChu { get => _ghiChu; set { _ghiChu = value; OnPropertyChanged(); } }

        private SanPham _selectedSP;
        public SanPham SelectedSP
        {
            get => _selectedSP;
            set
            {
                _selectedSP = value;
                if (_selectedSP != null)
                {
                    SLHeThong = _selectedSP.SLTon ?? 0;
                    SLThucTe = SLHeThong;
                }
                OnPropertyChanged();
            }
        }

        private int _slHeThong;
        public int SLHeThong { get => _slHeThong; set { _slHeThong = value; TinhLech(); OnPropertyChanged(); } }

        private int _slThucTe;
        public int SLThucTe { get => _slThucTe; set { _slThucTe = value; TinhLech(); OnPropertyChanged(); } }

        private int _slLech;
        public int SLLech { get => _slLech; set { _slLech = value; OnPropertyChanged(); } }

        private string _lyDo;
        public string LyDo { get => _lyDo; set { _lyDo = value; OnPropertyChanged(); } }

        private void TinhLech() => SLLech = SLThucTe - SLHeThong;

        public ObservableCollection<SanPham> DanhSachSP { get; set; }
        public ObservableCollection<ChiTietKiemKeUI> DanhSachChiTiet { get; set; }

        public ICommand AddDetailCommand { get; set; }
        public ICommand RemoveDetailCommand { get; set; }
        public ICommand EditDetailCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public PhieuKiemKeViewModel()
        {
            NgayLap = DateTime.Now;
            DanhSachSP = new ObservableCollection<SanPham>();
            DanhSachChiTiet = new ObservableCollection<ChiTietKiemKeUI>();

            LoadData();

            AddDetailCommand = new RelayCommand(ExecuteAddDetail, CanAddDetail);
            RemoveDetailCommand = new RelayCommand(ExecuteRemoveDetail);
            EditDetailCommand = new RelayCommand(ExecuteEditDetail);
            SaveCommand = new RelayCommand(ExecuteSave, CanSave);
        }

        private void LoadData()
        {
            using (var db = new QL_KHOEntities3())
            {
                MaPKK = TaoMaTuDong("PK", db);

                var sps = db.SanPhams.Where(x => x.IsActive == true).ToList();
                DanhSachSP.Clear();
                foreach (var s in sps) DanhSachSP.Add(s);
            }
        }

        private string TaoMaTuDong(string prefix, QL_KHOEntities3 db)
        {
            string maCuoi = db.PhieuKiemKes.OrderByDescending(x => x.MaPKK).Select(x => x.MaPKK).FirstOrDefault();
            if (string.IsNullOrEmpty(maCuoi)) return prefix + "00000001";

            string phanSo = maCuoi.Substring(2).Trim();
            if (int.TryParse(phanSo, out int soMoi)) return prefix + (soMoi + 1).ToString("D8");
            return prefix + "00000001";
        }

        private bool CanAddDetail(object obj) => SelectedSP != null;

        private void ExecuteAddDetail(object obj)
        {
            var exist = DanhSachChiTiet.FirstOrDefault(x => x.MaSP == SelectedSP.MaSP);
            if (exist == null)
            {
                DanhSachChiTiet.Add(new ChiTietKiemKeUI
                {
                    MaSP = SelectedSP.MaSP,
                    TenSP = SelectedSP.TenSP,
                    SLHeThong = SLHeThong,
                    SLThucTe = SLThucTe,
                    LyDo = LyDo
                });
            }
            SelectedSP = null; SLHeThong = 0; SLThucTe = 0; LyDo = "";
        }

        private void ExecuteEditDetail(object obj)
        {
            if (obj is ChiTietKiemKeUI item)
            {
                SelectedSP = DanhSachSP.FirstOrDefault(x => x.MaSP == item.MaSP);
                SLThucTe = item.SLThucTe;
                LyDo = item.LyDo;
                DanhSachChiTiet.Remove(item);
            }
        }

        private void ExecuteRemoveDetail(object obj)
        {
            if (obj is ChiTietKiemKeUI item) DanhSachChiTiet.Remove(item);
        }

        private bool CanSave(object obj) => DanhSachChiTiet.Count > 0;

        private void ExecuteSave(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                try
                {
                    string maMoi = TaoMaTuDong("PK", db);

                    var pk = new PhieuKiemKe
                    {
                        MaPKK = maMoi,
                        NgayKiemKe = NgayLap,
                        MaNV = DangNhapViewModel.CurrentUser != null ? DangNhapViewModel.CurrentUser.MaNV : "NV01",
                        GhiChu = GhiChu
                    };
                    db.PhieuKiemKes.Add(pk);

                    foreach (var item in DanhSachChiTiet)
                    {
                        db.ChiTietKiemKes.Add(new ChiTietKiemKe
                        {
                            MaPKK = maMoi,
                            MaSP = item.MaSP,
                            SLHeThong = item.SLHeThong,
                            SLThucTe = item.SLThucTe,
                            SLLech = item.SLLech,
                            LyDo = item.LyDo
                        });
                    }

                    db.SaveChanges();
                    MessageBox.Show("Tạo phiếu kiểm kê thành công! Mã phiếu: " + maMoi, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    DanhSachChiTiet.Clear(); GhiChu = "";
                    LoadData();
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