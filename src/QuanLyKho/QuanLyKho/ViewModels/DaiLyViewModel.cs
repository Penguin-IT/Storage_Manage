using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    public class DaiLyViewModel : BaseViewModel
    {
        private string _maDL;
        public string MaDL { get => _maDL; set { _maDL = value; ClearErrors(nameof(MaDL)); string err = DataValidator.KiemTraRong(value, "Mã Đại Lý") ?? DataValidator.KiemTraChieuDai(value, "Mã Đại Lý", 20); if (err != null) AddError(nameof(MaDL), err); OnPropertyChanged(); } }

        private string _tenDL;
        public string TenDL { get => _tenDL; set { _tenDL = value; ClearErrors(nameof(TenDL)); string err = DataValidator.KiemTraRong(value, "Tên Đại Lý"); if (err != null) AddError(nameof(TenDL), err); OnPropertyChanged(); } }

        private string _sdt;
        public string SDT { get => _sdt; set { _sdt = value; ClearErrors(nameof(SDT)); string err = DataValidator.KiemTraSoDienThoai(value); if (err != null) AddError(nameof(SDT), err); OnPropertyChanged(); } }

        private string _diaChi;
        public string DiaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(); } }

       
        private DaiLy _selectedDaiLy;
        public DaiLy SelectedDaiLy
        {
            get => _selectedDaiLy;
            set
            {
                _selectedDaiLy = value;
                if (_selectedDaiLy != null)
                {
                    MaDL = _selectedDaiLy.MaDL;
                    TenDL = _selectedDaiLy.TenDL;
                    SDT = _selectedDaiLy.SDT;
                    DiaChi = _selectedDaiLy.DiaChi;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DaiLy> DanhSachDaiLy { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public DaiLyViewModel()
        {
            DanhSachDaiLy = new ObservableCollection<DaiLy>();
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
              
                var dls = db.DaiLies.Where(x => x.IsActive == true).ToList();
                DanhSachDaiLy.Clear();
                foreach (var item in dls) DanhSachDaiLy.Add(item);
            }
        }

        private bool CanExecute(object obj) => !HasErrors && !string.IsNullOrWhiteSpace(MaDL) && !string.IsNullOrWhiteSpace(TenDL);

        private void ExecuteAdd(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                if (db.DaiLies.Any(x => x.MaDL == MaDL))
                {
                    MessageBox.Show("Mã Đại lý này đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                db.DaiLies.Add(new DaiLy { MaDL = MaDL, TenDL = TenDL, SDT = SDT, DiaChi = DiaChi, IsActive = true });
                db.SaveChanges();
                MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadData(); ClearForm();
        }

        private void ExecuteUpdate(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
                var dl = db.DaiLies.FirstOrDefault(x => x.MaDL == MaDL);
                if (dl == null)
                {
                    MessageBox.Show("Không tìm thấy đại lý để cập nhật!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                dl.TenDL = TenDL; dl.SDT = SDT; dl.DiaChi = DiaChi;
                db.SaveChanges();
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadData(); ClearForm();
        }

        private void ExecuteDelete(object obj)
        {
            if (obj is DaiLy dl && MessageBox.Show("Xóa đại lý này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new QL_KHOEntities3())
                {
                    var item = db.DaiLies.FirstOrDefault(x => x.MaDL == dl.MaDL);
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
            MaDL = ""; TenDL = ""; SDT = ""; DiaChi = ""; SelectedDaiLy = null;
            ClearErrors(nameof(MaDL)); ClearErrors(nameof(TenDL)); ClearErrors(nameof(SDT));
        }
    }
}