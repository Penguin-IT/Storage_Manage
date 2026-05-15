using Microsoft.Win32;
using StorageManage.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StorageManage.ViewModels
{
    public class ThemMoiSanPham : BaseViewModel
    {
        public string TenSanPham { get; set; }
        public double Gia { get; set; }
        public DateTime NgayNhap { get; set; } = DateTime.Now;

        public string ImagePath { get; set; }

        private ObservableCollection<TaoMoiDanhMuc> _danhMucs;
        public ObservableCollection<TaoMoiDanhMuc> DanhMucs
        {
            get => _danhMucs;
            set
            {
                _danhMucs = value;
                OnPropertyChanged(nameof(DanhMucs));
            }
        }
        private TaoMoiDanhMuc _selectedDanhMuc;
        public TaoMoiDanhMuc SelectedDanhMuc
        {
            get => _selectedDanhMuc;
            set
            {
                _selectedDanhMuc = value;
                OnPropertyChanged(nameof(SelectedDanhMuc));
            }
        }

        public ICommand SaveCommand { get; set; }
        public ICommand ChonAnhCommand { get; set; }

        public ThemMoiSanPham()
        {
            LoadDanhMuc();

            SaveCommand = new RelayCommand(_ => Save());
            ChonAnhCommand = new RelayCommand(_ => ChonAnh());
        }

        void LoadDanhMuc()
        {
            using (var db = new MyDbContext())
            {
                DanhMucs = new ObservableCollection<TaoMoiDanhMuc>(db.DanhMucs.ToList());
            }
        }

        void ChonAnh()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image|*.jpg;*.png";

            if (dlg.ShowDialog() == true)
            {
                ImagePath = dlg.FileName;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        void Save()
        {
            if (string.IsNullOrWhiteSpace(TenSanPham))
            {
                MessageBox.Show("Tên sản phẩm không được trống!");
                return;
            }

            if (SelectedDanhMuc == null)
            {
                MessageBox.Show("Phải chọn danh mục!");
                return;
            }

            using (var db = new MyDbContext())
            {
                db.SanPhams.Add(new SanPham
                {
                    TenSanPham = TenSanPham,
                    Gia = Gia,
                    NgayNhap = NgayNhap,
                    ImagePath = ImagePath,
                    DanhMucId = SelectedDanhMuc.Id
                });

                db.SaveChanges();
            }

            MessageBox.Show("Thêm sản phẩm thành công!");
        }
    }
}
