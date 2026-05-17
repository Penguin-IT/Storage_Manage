using StorageManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StorageManage.ViewModels
{
    public class TaoMoiDanhMuc : BaseViewModel
    { 
        private string _maLoai;
        public string MaLoai
        {
            get => _maLoai;
            set
            {
                _maLoai = value;
                OnPropertyChanged(nameof(MaLoai));
            }
        }

        private string _tenDanhMuc;
        public string TenDanhMuc
        {
            get => _tenDanhMuc;
            set
            {
                _tenDanhMuc = value;
                OnPropertyChanged(nameof(TenDanhMuc)); 
            }
        }

        public ICommand SaveCommand { get; set; }

        public TaoMoiDanhMuc()
        {
            SaveCommand = new RelayCommand(_ => Save());
        }

        void Save()
        {
            if (string.IsNullOrWhiteSpace(MaLoai) ||
                string.IsNullOrWhiteSpace(TenDanhMuc))
            {
                MessageBox.Show("Không được để trống Mã loại hoặc Tên danh mục!");
                return;
            }

            using (var db = new QLKEntities())
            {
                bool isExist = db.LoaiSanPhams.Any(x => x.MaLoai == MaLoai);
                if (isExist)
                {
                    MessageBox.Show("Mã loại đã tồn tại!");
                    return;
                }
                db.LoaiSanPhams.Add(new LoaiSanPham
                {
                    MaLoai = MaLoai,
                    TenLoai = TenDanhMuc,
                    
                });

                db.SaveChanges();
            }

            MessageBox.Show("Thêm thành công!");
            MaLoai = "";
            TenDanhMuc = "";
        }
    }
}
