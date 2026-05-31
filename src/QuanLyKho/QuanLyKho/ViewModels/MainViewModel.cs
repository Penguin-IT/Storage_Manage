using QuanLyKho.ViewModels.Core;
using System.Windows.Input;

namespace QuanLyKho.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
     
        private bool _isLoggedIn;
        public bool IsLoggedIn { get => _isLoggedIn; set { _isLoggedIn = value; OnPropertyChanged(); } }

      
        private bool _isAdmin;
        public bool IsAdmin { get => _isAdmin; set { _isAdmin = value; OnPropertyChanged(); } }

    
        private string _tenHienThi;
        public string TenHienThi { get => _tenHienThi; set { _tenHienThi = value; OnPropertyChanged(); } }

        private string _vaiTroHienThi;
        public string VaiTroHienThi { get => _vaiTroHienThi; set { _vaiTroHienThi = value; OnPropertyChanged(); } }

        private object _currentView;
        public object CurrentView { get => _currentView; set { _currentView = value; OnPropertyChanged(); } }

    
        public DangNhapViewModel DangNhapVM { get; set; }
        public TrangChuViewModel TrangChuVM { get; set; }
        public ThemDanhMucViewModel ThemDanhMucVM { get; set; }
        public NhaCungCapViewModel NhaCungCapVM { get; set; }
        public DaiLyViewModel DaiLyVM { get; set; }
        public ThemSanPhamViewModel ThemSanPhamVM { get; set; }
        public TaoHoaDonNhapViewModel HoaDonNhapVM { get; set; }
        public TaoHoaDonBanViewModel HoaDonBanVM { get; set; }
        public PhieuKiemKeViewModel KiemKeVM { get; set; }
        public TimKiemChungTuViewModel TimKiemVM { get; set; }
        public TraCuuTonKhoViewModel TonKhoVM { get; set; }
        public ThongKeDoanhThuViewModel DoanhThuVM { get; set; }
        public InBaoCaoViewModel InBaoCaoVM { get; set; }
        public BaoCaoPhanTichViewModel PhanTichVM { get; set; }
        public QuanLyTaiKhoanViewModel TaiKhoanVM { get; set; }
        public DoiMatKhauViewModel DoiMatKhauVM { get; set; }

       
        public ICommand TrangChuCmd { get; set; }
        public ICommand ThemDanhMucCmd { get; set; }
        public ICommand NhaCungCapCmd { get; set; }
        public ICommand DaiLyCmd { get; set; }
        public ICommand ThemSanPhamCmd { get; set; }
        public ICommand TaoHoaDonNhapCmd { get; set; }
        public ICommand TaoHoaDonBanCmd { get; set; }
        public ICommand PhieuKiemKeCmd { get; set; }
        public ICommand TimKiemChungTuCmd { get; set; }
        public ICommand TonKhoCmd { get; set; }
        public ICommand ThongKeDoanhThuCmd { get; set; }
        public ICommand InBaoCaoCmd { get; set; }
        public ICommand BaoCaoPhanTichCmd { get; set; }
        public ICommand QuanLyTaiKhoanCmd { get; set; }
        public ICommand DoiMatKhauCmd { get; set; }
        public ICommand DangXuatCmd { get; set; }

        public MainViewModel()
        {
            IsLoggedIn = false;
            IsAdmin = false; 

            DangNhapVM = new DangNhapViewModel(LoginSuccess);
            TrangChuVM = new TrangChuViewModel();
            ThemDanhMucVM = new ThemDanhMucViewModel();
            NhaCungCapVM = new NhaCungCapViewModel();
            DaiLyVM = new DaiLyViewModel();
            ThemSanPhamVM = new ThemSanPhamViewModel();
            HoaDonNhapVM = new TaoHoaDonNhapViewModel();
            HoaDonBanVM = new TaoHoaDonBanViewModel();
            KiemKeVM = new PhieuKiemKeViewModel();
            TimKiemVM = new TimKiemChungTuViewModel();
            TonKhoVM = new TraCuuTonKhoViewModel();
            DoanhThuVM = new ThongKeDoanhThuViewModel();
            InBaoCaoVM = new InBaoCaoViewModel();
            PhanTichVM = new BaoCaoPhanTichViewModel();
            TaiKhoanVM = new QuanLyTaiKhoanViewModel();
            DoiMatKhauVM = new DoiMatKhauViewModel();

            CurrentView = DangNhapVM;

            TrangChuCmd = new RelayCommand(o => CurrentView = TrangChuVM);
            ThemDanhMucCmd = new RelayCommand(o => CurrentView = ThemDanhMucVM);
            NhaCungCapCmd = new RelayCommand(o => CurrentView = NhaCungCapVM);
            DaiLyCmd = new RelayCommand(o => CurrentView = DaiLyVM);
            ThemSanPhamCmd = new RelayCommand(o => CurrentView = ThemSanPhamVM);
            TaoHoaDonNhapCmd = new RelayCommand(o => CurrentView = HoaDonNhapVM);
            TaoHoaDonBanCmd = new RelayCommand(o => CurrentView = HoaDonBanVM);
            PhieuKiemKeCmd = new RelayCommand(o => CurrentView = KiemKeVM);
            TimKiemChungTuCmd = new RelayCommand(o => CurrentView = TimKiemVM);
            TonKhoCmd = new RelayCommand(o => CurrentView = TonKhoVM);
            ThongKeDoanhThuCmd = new RelayCommand(o => CurrentView = DoanhThuVM);
            InBaoCaoCmd = new RelayCommand(o => CurrentView = InBaoCaoVM);
            BaoCaoPhanTichCmd = new RelayCommand(o => CurrentView = PhanTichVM);
            QuanLyTaiKhoanCmd = new RelayCommand(o => CurrentView = TaiKhoanVM);
            DoiMatKhauCmd = new RelayCommand(o => CurrentView = DoiMatKhauVM);

            DangXuatCmd = new RelayCommand(o =>
            {
                IsLoggedIn = false;
                IsAdmin = false;
                DangNhapVM.TenDangNhap = "";
                CurrentView = DangNhapVM;
            });
        }

        private void LoginSuccess()
        {
            IsLoggedIn = true;

          
            var user = DangNhapViewModel.CurrentUser;
            if (user != null)
            {
                TenHienThi = user.TenDangNhap; 
                VaiTroHienThi = user.QuyenTruyCap;   

               
                IsAdmin = (user.QuyenTruyCap == "Quản Lý");
            }

          
            CurrentView = TrangChuVM;
        }
    }
}