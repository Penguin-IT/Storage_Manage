using Microsoft.Win32;
using StorageManage.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StorageManage.ViewModels
{
    public class ThemMoiSanPham : BaseViewModel
    {

        private string _tenSanPham;
        public string TenSanPham
        {
            get => _tenSanPham;
            set
            {
                _tenSanPham = value;
                OnPropertyChanged(nameof(TenSanPham));
            }
        }

        private double _gia;
        public double Gia
        {
            get => _gia;
            set
            {
                _gia = value;
                OnPropertyChanged(nameof(Gia));
            }
        }

        private ObservableCollection<LoaiSanPham> _danhMucs;
        public ObservableCollection<LoaiSanPham> DanhMucs
        {
            get => _danhMucs;
            set
            {
                _danhMucs = value;
                OnPropertyChanged(nameof(DanhMucs));
            }
        }

        private LoaiSanPham _selectedDanhMuc;
        public LoaiSanPham SelectedDanhMuc
        {
            get => _selectedDanhMuc;
            set
            {
                _selectedDanhMuc = value;
                OnPropertyChanged(nameof(SelectedDanhMuc));
            }
        }

        public ICommand SaveCommand { get; set; }

        public ThemMoiSanPham()
        {
            SaveCommand = new RelayCommand(_ => Save());

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadDanhMuc();
            }
        }

        void LoadDanhMuc()
        {
            using (var db = new QLKEntities())
            {
                DanhMucs = new ObservableCollection<LoaiSanPham>(db.LoaiSanPhams.ToList());
            }
        }

        void Save()
        {
            if (string.IsNullOrWhiteSpace(TenSanPham))
            {
                MessageBox.Show("Tên sản phẩm không được trống!");
                return;
            }

            if (Gia <= 0)
            {
                MessageBox.Show("Giá phải lớn hơn 0!");
                return;
            }

            if (SelectedDanhMuc == null)
            {
                MessageBox.Show("Phải chọn danh mục!");
                return;
            }

            using (var db = new QLKEntities())
            {
                db.SanPhams.Add(new SanPham
                {
                    TenSP = TenSanPham,
                    DonGia = (int)Gia,
                    MaLoai = SelectedDanhMuc.MaLoai
                });

                db.SaveChanges();
            }

            MessageBox.Show("Thêm sản phẩm thành công!");

            TenSanPham = "";
            Gia = 0;
            SelectedDanhMuc = null;
        }
    }
}