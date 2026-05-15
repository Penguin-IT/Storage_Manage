using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace StorageManage.ViewModels
{
    public class TaoMoiDanhMuc : BaseViewModel
    {
        public string TenDanhMuc { get; set; }
        public string MoTa { get; set; }

        public ICommand SaveCommand { get; set; }

        public TaoMoiDanhMuc()
        {
            SaveCommand = new RelayCommand(_ => Save());
        }

        void Save()
        {
            if (string.IsNullOrWhiteSpace(TenDanhMuc))
            {
                MessageBox.Show("Không được để trống!");
                return;
            }

            using (var db = new QLKEntities()) 
            {
                db.LoaiSPs.Add(new LoaiSanPham
                {
                    TenLoai = TenDanhMuc
                });

                db.SaveChanges();
            }

            MessageBox.Show("Thêm thành công!");
        }
    }
}
