using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32; 

namespace QuanLyKho.ViewModels
{
    public class ThemSanPhamViewModel : BaseViewModel
    {
        #region Properties
        private string _maSP;
        public string MaSP { get => _maSP; set { _maSP = value; OnPropertyChanged(); } }

        private string _tenSP;
        public string TenSP { get => _tenSP; set { _tenSP = value; OnPropertyChanged(); } }

        private string _maLoai;
        public string MaLoai { get => _maLoai; set { _maLoai = value; OnPropertyChanged(); } }

        private decimal? _donGia;
        public decimal? DonGia { get => _donGia; set { _donGia = value; OnPropertyChanged(); } }

        private string _donViTinh;
        public string DonViTinh { get => _donViTinh; set { _donViTinh = value; OnPropertyChanged(); } }

       
        private string _hinhAnh;
        public string HinhAnh { get => _hinhAnh; set { _hinhAnh = value; OnPropertyChanged(); } }

        private SanPham _selectedSanPham;
        public SanPham SelectedSanPham
        {
            get => _selectedSanPham;
            set
            {
                _selectedSanPham = value;
                if (_selectedSanPham != null)
                {
                    MaSP = _selectedSanPham.MaSP;
                    TenSP = _selectedSanPham.TenSP;
                    MaLoai = _selectedSanPham.MaLoai;
                    DonGia = _selectedSanPham.DonGia;
                    DonViTinh = _selectedSanPham.DVT;
                    HinhAnh = _selectedSanPham.HinhAnh; 
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region Collections
        public ObservableCollection<SanPham> DanhSachSanPham { get; set; }
        public ObservableCollection<LoaiSanPham> DanhSachLoaiSP { get; set; }
        #endregion

        #region Commands
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand UploadImageCommand { get; set; }
        #endregion

        public ThemSanPhamViewModel()
        {
            DanhSachSanPham = new ObservableCollection<SanPham>();
            DanhSachLoaiSP = new ObservableCollection<LoaiSanPham>();

            LoadData();

            AddCommand = new RelayCommand(ExecuteAdd, CanExecute);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecute);
            ClearCommand = new RelayCommand(o => ClearForm());
            DeleteCommand = new RelayCommand(ExecuteDelete);

        
            UploadImageCommand = new RelayCommand(ExecuteUploadImage);
        }

        private void ExecuteUploadImage(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
              
                HinhAnh = openFileDialog.FileName;
            }
        }

        private void LoadData()
        {
            using (var db = new QL_KHOEntities3())
            {
                var loaiList = db.LoaiSanPhams.ToList();
                DanhSachLoaiSP.Clear();
                foreach (var loai in loaiList) DanhSachLoaiSP.Add(loai);

                var spList = db.SanPhams.Include("LoaiSanPham").Where(x => x.IsActive == true).ToList();
                DanhSachSanPham.Clear();
                foreach (var sp in spList) DanhSachSanPham.Add(sp);
            }
        }

        private bool CanExecute(object obj)
        {
            return !string.IsNullOrWhiteSpace(MaSP) &&
                   !string.IsNullOrWhiteSpace(TenSP) &&
                   !string.IsNullOrWhiteSpace(MaLoai);
        }

        private void ExecuteAdd(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                if (db.SanPhams.Any(x => x.MaSP == MaSP))
                {
                    MessageBox.Show("Mã sản phẩm này đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newSP = new SanPham
                {
                    MaSP = MaSP,
                    TenSP = TenSP,
                    MaLoai = MaLoai,
                    DonGia = DonGia ?? 0,
                    DVT = DonViTinh,
                    HinhAnh = HinhAnh, 
                    SLTon = 0,
                    IsActive = true
                };

                db.SanPhams.Add(newSP);
                db.SaveChanges();
                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadData();
            ClearForm();
        }

        private void ExecuteUpdate(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                var sp = db.SanPhams.FirstOrDefault(x => x.MaSP == MaSP);
                if (sp == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm để cập nhật (Không được sửa Mã SP)!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                sp.TenSP = TenSP;
                sp.MaLoai = MaLoai;
                sp.DonGia = DonGia;
                sp.DVT = DonViTinh;

              
                if (!string.IsNullOrEmpty(HinhAnh))
                {
                    sp.HinhAnh = HinhAnh;
                }

                db.SaveChanges();
                MessageBox.Show("Cập nhật thông tin sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadData();
            ClearForm();
        }

        private void ExecuteDelete(object obj)
        {
            if (obj is SanPham sp && MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm {sp.TenSP}?\n(Dữ liệu sẽ được ẩn đi nhưng vẫn giữ lại trong lịch sử Nhập/Xuất)", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new QL_KHOEntities3())
                {
                    var item = db.SanPhams.FirstOrDefault(x => x.MaSP == sp.MaSP);
                    if (item != null)
                    {
                        item.IsActive = false;
                        db.SaveChanges();
                        MessageBox.Show("Đã xóa sản phẩm khỏi hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                LoadData();
            }
        }

        private void ClearForm()
        {
            MaSP = "";
            TenSP = "";
            MaLoai = null;
            DonGia = null;
            DonViTinh = "";
            HinhAnh = null; 
            SelectedSanPham = null;
        }
    }
}