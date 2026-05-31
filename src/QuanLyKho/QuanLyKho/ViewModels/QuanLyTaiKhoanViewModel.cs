using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    public class QuanLyTaiKhoanViewModel : BaseViewModel
    {
        #region Properties
        private string _tenDangNhap;
        public string TenDangNhap { get => _tenDangNhap; set { _tenDangNhap = value; OnPropertyChanged(); } }

        private string _matKhau;
        public string MatKhau { get => _matKhau; set { _matKhau = value; OnPropertyChanged(); } }

        private string _quyenTruyCap;
        public string QuyenTruyCap { get => _quyenTruyCap; set { _quyenTruyCap = value; OnPropertyChanged(); } }

       
        private NhanVien _nhanVienDuocChon;
        public NhanVien NhanVienDuocChon { get => _nhanVienDuocChon; set { _nhanVienDuocChon = value; OnPropertyChanged(); } }

        public ObservableCollection<string> DanhSachVaiTro { get; set; }
        public ObservableCollection<NhanVien> DanhSachNhanVien { get; set; } 
        public ObservableCollection<TaiKhoan> DanhSachTaiKhoan { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        #endregion

        public QuanLyTaiKhoanViewModel()
        {
            DanhSachTaiKhoan = new ObservableCollection<TaiKhoan>();
            DanhSachNhanVien = new ObservableCollection<NhanVien>();

            DanhSachVaiTro = new ObservableCollection<string>
            {
                "Quản Lý",
                "Nhân Viên"
            };
            QuyenTruyCap = DanhSachVaiTro[1];

            LoadData();

            AddCommand = new RelayCommand(ExecuteAdd, CanExecuteAdd);
            DeleteCommand = new RelayCommand(ExecuteDelete);
        }

        private void LoadData()
        {
            using (var db = new QL_KHOEntities3())
            {
             
                var listNV = db.NhanViens.ToList();
                DanhSachNhanVien.Clear();
                foreach (var nv in listNV) DanhSachNhanVien.Add(nv);

              
                var listTK = db.TaiKhoans.Include("NhanVien").Where(x => x.TrangThai == true).ToList();
                DanhSachTaiKhoan.Clear();
                foreach (var tk in listTK) DanhSachTaiKhoan.Add(tk);
            }
        }

        private bool CanExecuteAdd(object obj)
        {
          
            return !string.IsNullOrWhiteSpace(TenDangNhap) &&
                   !string.IsNullOrWhiteSpace(MatKhau) &&
                   !string.IsNullOrWhiteSpace(QuyenTruyCap) &&
                   NhanVienDuocChon != null;
        }

        private void ExecuteAdd(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                if (db.TaiKhoans.Any(x => x.TenDangNhap == TenDangNhap))
                {
                    MessageBox.Show("Tên đăng nhập này đã tồn tại! Vui lòng chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newAccount = new TaiKhoan
                {
                    TenDangNhap = TenDangNhap,
                    MatKhau = MatKhau,
                    QuyenTruyCap = QuyenTruyCap,
                    TrangThai = true,
                    MaNV = NhanVienDuocChon.MaNV 
                };

                db.TaiKhoans.Add(newAccount);
                db.SaveChanges();
                MessageBox.Show("Thêm tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            TenDangNhap = "";
            MatKhau = "";
            NhanVienDuocChon = null; 
            LoadData();
        }

        private void ExecuteDelete(object obj)
        {
            if (obj is TaiKhoan tk && MessageBox.Show($"Bạn có chắc chắn muốn vô hiệu hóa tài khoản [{tk.TenDangNhap}]?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new QL_KHOEntities3())
                {
                    var item = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == tk.TenDangNhap);
                    if (item != null)
                    {
                        item.TrangThai = false;
                        db.SaveChanges();
                        MessageBox.Show("Đã khóa tài khoản thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                LoadData();
            }
        }
    }
}