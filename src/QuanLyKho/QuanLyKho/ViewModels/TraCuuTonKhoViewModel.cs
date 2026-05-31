using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
 
    public class SanPhamTonKhoUI
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string TenLoai { get; set; }
        public int SLTon { get; set; }
        public decimal DonGia { get; set; }
        public string TinhTrang { get; set; }
    }

    public class TraCuuTonKhoViewModel : BaseViewModel
    {
        private string _maLoaiLoc;
        public string MaLoaiLoc { get => _maLoaiLoc; set { _maLoaiLoc = value; OnPropertyChanged(); } }

        private string _tuKhoa;
        public string TuKhoa { get => _tuKhoa; set { _tuKhoa = value; OnPropertyChanged(); } }

    
        public ObservableCollection<LoaiSanPham> DanhSachLoaiSPLoc { get; set; }

        public ObservableCollection<SanPhamTonKhoUI> DanhSachSanPham { get; set; }

        public ICommand SearchCommand { get; set; }

        public TraCuuTonKhoViewModel()
        {
            
            DanhSachLoaiSPLoc = new ObservableCollection<LoaiSanPham>();
            DanhSachSanPham = new ObservableCollection<SanPhamTonKhoUI>();

            LoadFilters();
            ExecuteSearch(null); 

            SearchCommand = new RelayCommand(ExecuteSearch);
        }

        
        private void LoadFilters()
        {
            using (var db = new QL_KHOEntities3())
            {
                var loaiList = db.LoaiSanPhams.ToList();
                DanhSachLoaiSPLoc.Clear();

           
                DanhSachLoaiSPLoc.Add(new LoaiSanPham { MaLoai = "", TenLoai = "Tất cả" });

                foreach (var loai in loaiList)
                {
                    DanhSachLoaiSPLoc.Add(loai);
                }

             
                MaLoaiLoc = "";
            }
        }

        
        private void ExecuteSearch(object obj)
        {
            using (var db = new QL_KHOEntities3())
            {
             
                var query = db.SanPhams.Include("LoaiSanPham").Where(x => x.IsActive == true).AsQueryable();

                
                if (!string.IsNullOrWhiteSpace(MaLoaiLoc))
                {
                    query = query.Where(x => x.MaLoai == MaLoaiLoc);
                }

               
                if (!string.IsNullOrWhiteSpace(TuKhoa))
                {
                    var keyword = TuKhoa.ToLower();
                    query = query.Where(x => x.MaSP.ToLower().Contains(keyword) || x.TenSP.ToLower().Contains(keyword));
                }

                var list = query.ToList();

               
                DanhSachSanPham.Clear();
                foreach (var sp in list)
                {
                    DanhSachSanPham.Add(new SanPhamTonKhoUI
                    {
                        MaSP = sp.MaSP,
                        TenSP = sp.TenSP,
                        TenLoai = sp.LoaiSanPham != null ? sp.LoaiSanPham.TenLoai : "Chưa phân loại",
                        SLTon = sp.SLTon ?? 0,
                        DonGia = sp.DonGia ?? 0,
                        TinhTrang = (sp.SLTon > 0) ? "Còn hàng" : "Hết hàng"
                    });
                }
            }
        }
    }
}