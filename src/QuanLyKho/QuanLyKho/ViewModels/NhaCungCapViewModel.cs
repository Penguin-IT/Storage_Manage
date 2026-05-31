using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    public class NhaCungCapViewModel : BaseViewModel
    {
        private string _maNCC;
        public string MaNCC { get => _maNCC; set { _maNCC = value; ClearErrors(nameof(MaNCC)); var err = DataValidator.KiemTraRong(value, "Mã NCC"); if (err != null) AddError(nameof(MaNCC), err); OnPropertyChanged(); } }

        private string _tenNCC;
        public string TenNCC { get => _tenNCC; set { _tenNCC = value; ClearErrors(nameof(TenNCC)); var err = DataValidator.KiemTraRong(value, "Tên NCC"); if (err != null) AddError(nameof(TenNCC), err); OnPropertyChanged(); } }

        private string _sdt;
        public string SDT { get => _sdt; set { _sdt = value; ClearErrors(nameof(SDT)); var err = DataValidator.KiemTraSoDienThoai(value); if (err != null) AddError(nameof(SDT), err); OnPropertyChanged(); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string _diaChi;
        public string DiaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(); } }

      
        private NhaCungCap _selectedNCC;
        public NhaCungCap SelectedNCC
        {
            get => _selectedNCC;
            set
            {
                _selectedNCC = value;
                if (_selectedNCC != null)
                {
                    MaNCC = _selectedNCC.MaNCC;
                    TenNCC = _selectedNCC.TenNCC;
                    SDT = _selectedNCC.SDT;
                    Email = _selectedNCC.Email;
                    DiaChi = _selectedNCC.DiaChi;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NhaCungCap> DanhSachNCC { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public NhaCungCapViewModel()
        {
            DanhSachNCC = new ObservableCollection<NhaCungCap>();
            LoadData();
            AddCommand = new RelayCommand(ExecuteAdd, CanExecute);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecute);
            ClearCommand = new RelayCommand(o => ClearForm());
            DeleteCommand = new RelayCommand(ExecuteDelete);
        }

        private void LoadData()
        {
            using (var db = new QL_KHOEntities3()) 
            {
              
                var nccs = db.NhaCungCaps.Where(x => x.IsActive == true).ToList();
                DanhSachNCC.Clear();
                foreach (var item in nccs) DanhSachNCC.Add(item);
            }
        }

        private bool CanExecute(object obj) => !HasErrors && !string.IsNullOrWhiteSpace(MaNCC) && !string.IsNullOrWhiteSpace(TenNCC);

        private void ExecuteAdd(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                if (db.NhaCungCaps.Any(x => x.MaNCC == MaNCC))
                {
                    MessageBox.Show("Mã Nhà cung cấp này đã tồn tại!", "Trùng mã", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                db.NhaCungCaps.Add(new NhaCungCap { MaNCC = MaNCC, TenNCC = TenNCC, SDT = SDT, Email = Email, DiaChi = DiaChi, IsActive = true });
                db.SaveChanges();
                MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadData(); ClearForm();
        }

        private void ExecuteUpdate(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                var ncc = db.NhaCungCaps.FirstOrDefault(x => x.MaNCC == MaNCC);
                if (ncc == null)
                {
                    MessageBox.Show("Không tìm thấy mã này để cập nhật!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ncc.TenNCC = TenNCC; ncc.SDT = SDT; ncc.Email = Email; ncc.DiaChi = DiaChi;
                db.SaveChanges();
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadData(); ClearForm();
        }

        private void ExecuteDelete(object obj)
        {
            if (obj is NhaCungCap ncc && MessageBox.Show($"Xóa Nhà cung cấp {ncc.TenNCC}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new QL_KHOEntities3())
                {
                    var item = db.NhaCungCaps.FirstOrDefault(x => x.MaNCC == ncc.MaNCC);
                    if (item != null)
                    {
                      
                        item.IsActive = false;
                        db.SaveChanges();
                    }
                }
                LoadData();
            }
        }

        private void ClearForm()
        {
            MaNCC = ""; TenNCC = ""; SDT = ""; Email = ""; DiaChi = ""; SelectedNCC = null;
            ClearErrors(nameof(MaNCC)); ClearErrors(nameof(TenNCC)); ClearErrors(nameof(SDT));
        }
    }
}