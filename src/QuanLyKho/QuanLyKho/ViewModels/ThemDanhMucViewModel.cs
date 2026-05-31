using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    public class ThemDanhMucViewModel : BaseViewModel
    {
        private string _maLoai;
        public string MaLoai { get => _maLoai; set { _maLoai = value; OnPropertyChanged(); } }

        private string _tenLoai;
        public string TenLoai { get => _tenLoai; set { _tenLoai = value; OnPropertyChanged(); } }

        private string _ghiChu;
        public string GhiChu { get => _ghiChu; set { _ghiChu = value; OnPropertyChanged(); } }

       
        private LoaiSanPham _selectedLoai;
        public LoaiSanPham SelectedLoai
        {
            get => _selectedLoai;
            set
            {
                _selectedLoai = value;
               
                if (_selectedLoai != null)
                {
                    MaLoai = _selectedLoai.MaLoai;
                    TenLoai = _selectedLoai.TenLoai;
                    GhiChu = _selectedLoai.GhiChu;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LoaiSanPham> DanhSachLoaiSP { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public ThemDanhMucViewModel()
        {
            DanhSachLoaiSP = new ObservableCollection<LoaiSanPham>();
            LoadData();

            AddCommand = new RelayCommand(ExecuteAdd, CanExecute);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecute);
            ClearCommand = new RelayCommand(o => ClearForm());
        }

        private void LoadData()
        {
            using (var db = new QL_KHOEntities3())
            {
                var list = db.LoaiSanPhams.ToList();
                DanhSachLoaiSP.Clear();
                foreach (var item in list) DanhSachLoaiSP.Add(item);
            }
        }

        
        private bool CanExecute(object obj) => !string.IsNullOrWhiteSpace(MaLoai) && !string.IsNullOrWhiteSpace(TenLoai);

        private void ExecuteAdd(object obj)
        {
         
            if (!Regex.IsMatch(MaLoai, @"^L\d{2}$"))
            {
                MessageBox.Show("Mã danh mục phải có định dạng LXX (VD: L01, L15).", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new QL_KHOEntities3())
            {
            
                var exists = db.LoaiSanPhams.Any(x => x.MaLoai == MaLoai);
                if (exists)
                {
                    MessageBox.Show("Mã danh mục này đã tồn tại trong hệ thống. Không thể thêm mới!", "Trùng dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            
                db.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = MaLoai, TenLoai = TenLoai, GhiChu = GhiChu });
                db.SaveChanges();
                MessageBox.Show("Thêm mới danh mục thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LoadData();
            ClearForm();
        }

        private void ExecuteUpdate(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
              
                var loaiSP = db.LoaiSanPhams.FirstOrDefault(x => x.MaLoai == MaLoai);
                if (loaiSP == null)
                {
                    MessageBox.Show("Không tìm thấy danh mục này để cập nhật (Mã danh mục không được sửa).", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                
                loaiSP.TenLoai = TenLoai;
                loaiSP.GhiChu = GhiChu;
                db.SaveChanges();
                MessageBox.Show("Cập nhật danh mục thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LoadData();
            ClearForm();
        }

        private void ClearForm()
        {
            MaLoai = "";
            TenLoai = "";
            GhiChu = "";
            SelectedLoai = null;
        }
    }
}