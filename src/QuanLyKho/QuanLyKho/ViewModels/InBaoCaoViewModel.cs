using QuanLyKho.Models;
using QuanLyKho.ViewModels.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
 
    public class BaoCaoXNT_UI
    {
        public int STT { get; set; }
        public string TenSP { get; set; }
        public int TonDauKy { get; set; }
        public int NhapTrongKy { get; set; }
        public int XuatTrongKy { get; set; }
        public int TonCuoiKy { get; set; }
    }

    public class InBaoCaoViewModel : BaseViewModel
    {
        public string TieuDeBaoCao { get; set; } = "BÁO CÁO TỔNG HỢP XUẤT NHẬP TỒN";

        public ObservableCollection<BaoCaoXNT_UI> DanhSachBaoCao { get; set; }

        public ICommand PrintPdfCommand { get; set; }

        public InBaoCaoViewModel()
        {
            DanhSachBaoCao = new ObservableCollection<BaoCaoXNT_UI>();
            PrintPdfCommand = new RelayCommand(ExecutePrintPdf);

         
            LoadBaoCaoXNT();
        }

        private void LoadBaoCaoXNT()
        {
            using (var db = new QL_KHOEntities3())
            {
                DanhSachBaoCao.Clear();

              
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = DateTime.Now;

               
                var sanPhams = db.SanPhams.Where(x => x.IsActive == true).ToList();
                var chiTietNhaps = db.ChiTietPNs.Include("PhieuNhap").ToList();
                var chiTietXuats = db.ChiTietPXes.Include("PhieuXuat").ToList();

                int stt = 1;
                foreach (var sp in sanPhams)
                {
                    
                    int nhapDauKy = chiTietNhaps.Where(x => x.MaSP == sp.MaSP && x.PhieuNhap != null && x.PhieuNhap.NgayNhap < startDate).Sum(x => x.SoLuong) ?? 0;
                    int xuatDauKy = chiTietXuats.Where(x => x.MaSP == sp.MaSP && x.PhieuXuat != null && x.PhieuXuat.NgayXuat < startDate).Sum(x => x.SoLuong) ?? 0;
                    int tonDauKy = nhapDauKy - xuatDauKy;

                  
                    int nhapTrongKy = chiTietNhaps.Where(x => x.MaSP == sp.MaSP && x.PhieuNhap != null && x.PhieuNhap.NgayNhap >= startDate && x.PhieuNhap.NgayNhap <= endDate).Sum(x => x.SoLuong) ?? 0;
                    int xuatTrongKy = chiTietXuats.Where(x => x.MaSP == sp.MaSP && x.PhieuXuat != null && x.PhieuXuat.NgayXuat >= startDate && x.PhieuXuat.NgayXuat <= endDate).Sum(x => x.SoLuong) ?? 0;

                 
                    int tonCuoiKy = tonDauKy + nhapTrongKy - xuatTrongKy;

               
                    DanhSachBaoCao.Add(new BaoCaoXNT_UI
                    {
                        STT = stt++,
                        TenSP = sp.TenSP,
                        TonDauKy = tonDauKy,
                        NhapTrongKy = nhapTrongKy,
                        XuatTrongKy = xuatTrongKy,
                        TonCuoiKy = tonCuoiKy
                    });
                }
            }
        }

        private void ExecutePrintPdf(object obj)
        {
            MessageBox.Show("Hệ thống đang trích xuất Báo cáo Xuất Nhập Tồn ra định dạng PDF...\n(Vui lòng chờ trong giây lát)",
                            "Thông báo In PDF",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}