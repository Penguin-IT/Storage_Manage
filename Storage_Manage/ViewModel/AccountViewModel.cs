using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Storage_Manage.Model; // Giữ nguyên để sử dụng BaseViewModel và RelayCommand của bạn
using StorageManage.Models;

namespace Storage_Manage.ViewModels
{
    // Class phụ phục vụ hiển thị lên DataGrid
    public class UserTemp
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }

    public class AccountViewModel : BaseViewModel
    {
        private ObservableCollection<UserTemp> _listUser;
        public ObservableCollection<UserTemp> ListUser { get => _listUser; set { _listUser = value; OnPropertyChanged(); } }

        private UserTemp _selectedUser;
        public UserTemp SelectedUser { get => _selectedUser; set { _selectedUser = value; OnPropertyChanged(); } }

        private string _newUsername;
        public string NewUsername { get => _newUsername; set { _newUsername = value; OnPropertyChanged(); } }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(); } }

        private string _selectedRole;
        public string SelectedRole { get => _selectedRole; set { _selectedRole = value; OnPropertyChanged(); } }

        public ICommand AddAccountCommand { get; set; }
        public ICommand DeleteAccountCommand { get; set; }

        public AccountViewModel()
        {
            LoadData();
            // Khởi tạo command liên kết với các nút bấm trên giao diện
            AddAccountCommand = new RelayCommand<object>((p) => true, (p) => ExecuteAddAccount());
            DeleteAccountCommand = new RelayCommand<object>((p) => true, (p) => ExecuteDeleteAccount());
        }

        // SỬA ĐIỀU 1: Đảm bảo hiển thị đầy đủ mọi tài khoản (Kể cả Admin chưa phân quyền)
        void LoadData()
        {
            try
            {
                using (var db = new StorageManageEntities())
                {
                    // Tải toàn bộ danh sách tài khoản về bộ nhớ trước để tránh lỗi Inner Join lọc mất dữ liệu
                    var accountList = db.TaiKhoans.ToList();

                    var data = accountList.Select(tk => new UserTemp
                    {
                        Username = tk.TenDangNhap,
                        // Kiểm tra null an toàn: nếu không có nhân viên/quyền thì hiển thị "Chưa phân quyền" thay vì ẩn đi
                        Role = (tk.NhanVien != null && tk.NhanVien.NhomQuyen != null)
                               ? tk.NhanVien.NhomQuyen.TenQuyen
                               : "Chưa phân quyền"
                    }).ToList();

                    ListUser = new ObservableCollection<UserTemp>(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách: " + ex.Message);
            }
        }

        // SỬA ĐIỀU 2: Sửa chức năng Thêm tài khoản chạy ổn định
        void ExecuteAddAccount()
        {
            // 1. Kiểm tra nhập liệu cơ bản
            if (string.IsNullOrEmpty(NewUsername) || string.IsNullOrEmpty(NewPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SelectedRole))
            {
                MessageBox.Show("Vui lòng chọn vai trò (Quản lý / Nhân viên)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new StorageManageEntities())
                {
                    // 2. Kiểm tra xem tên đăng nhập đã tồn tại chưa
                    var existingAcc = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == NewUsername);
                    if (existingAcc != null)
                    {
                        MessageBox.Show("Tên đăng nhập này đã tồn tại trong hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var quyen = db.NhomQuyens.FirstOrDefault(q => q.TenQuyen == SelectedRole);

                    // 3. Tạo Mã Nhân Viên (Cắt ngắn tối đa 10 ký tự để tránh lỗi tràn bộ nhớ SQL)
                    string maNVMoi = "NV_" + NewUsername;
                    if (maNVMoi.Length > 10)
                    {
                        maNVMoi = maNVMoi.Substring(0, 10);
                    }

                    // 4. Xử lý Nhân viên thông minh (Tái sử dụng nhân viên cũ nếu trùng mã)
                    var existingEmp = db.NhanViens.FirstOrDefault(x => x.MaNV == maNVMoi);
                    NhanVien empToLink;

                    if (existingEmp != null)
                    {
                        // Nếu đã có nhân viên này, sử dụng lại và cập nhật quyền mới
                        empToLink = existingEmp;
                        if (quyen != null) empToLink.MaQuyen = quyen.MaQuyen;
                    }
                    else
                    {
                        // Nếu chưa có, tạo mới hoàn toàn
                        empToLink = new NhanVien()
                        {
                            MaNV = maNVMoi,
                            HoTen = NewUsername,
                            MaQuyen = quyen?.MaQuyen
                        };
                        db.NhanViens.Add(empToLink);
                    }

                    // 5. Tạo Tài khoản liên kết
                    var newAcc = new TaiKhoan()
                    {
                        TenDangNhap = NewUsername,
                        MatKhau = NewPassword,
                        MaNV = empToLink.MaNV,
                        TrangThai = true
                    };
                    db.TaiKhoans.Add(newAcc);

                    // Lưu xuống Database
                    db.SaveChanges();

                    // 6. Làm sạch giao diện sau khi thêm xong
                    NewUsername = "";
                    NewPassword = "";

                    LoadData();
                    MessageBox.Show("Thêm tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            // BỘ RADAR DÒ LỖI EF: Chỉ đích danh cột nào đang bị sai định dạng trong CSDL
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                string errorMsg = "Database từ chối dữ liệu vì:\n";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMsg += $"- Cột '{validationError.PropertyName}': {validationError.ErrorMessage}\n";
                    }
                }
                MessageBox.Show(errorMsg, "Lỗi Ràng Buộc Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Bắt các lỗi hệ thống/SQL thông thường (ví dụ: mất kết nối, lỗi khóa ngoại...)
            catch (Exception ex)
            {
                string detail = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show("Lỗi hệ thống: " + detail, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // SỬA ĐIỀU 3: Chức năng Xóa an toàn, không lo bị crash do ràng buộc dữ liệu
        void ExecuteDeleteAccount()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản từ danh sách để xóa!");
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản {SelectedUser.Username}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                using (var db = new StorageManageEntities())
                {
                    // Tìm tài khoản cần xóa
                    var acc = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == SelectedUser.Username);
                    if (acc != null)
                    {
                        string maNV = acc.MaNV;

                        // Bước 1: Xóa tài khoản trước để cắt đứt quyền đăng nhập
                        db.TaiKhoans.Remove(acc);

                        // Bước 2: Kiểm tra xử lý bảng Nhân Viên
                        if (!string.IsNullOrEmpty(maNV))
                        {
                            var emp = db.NhanViens.FirstOrDefault(x => x.MaNV == maNV);
                            if (emp != null)
                            {
                                // Kiểm tra xem nhân viên này đã từng lập phiếu nhập hoặc phiếu xuất nào chưa
                                bool hasHistory = db.PhieuNhaps.Any(p => p.MaNV == maNV) || db.PhieuXuats.Any(p => p.MaNV == maNV);

                                if (!hasHistory)
                                {
                                    // Nếu chưa từng phát sinh dữ liệu, tiến hành xóa sạch thông tin nhân viên
                                    db.NhanViens.Remove(emp);
                                }
                                // Nếu đã có lịch sử đơn hàng, ta chỉ xóa tài khoản đăng nhập để bảo toàn lịch sử hệ thống
                            }
                        }

                        db.SaveChanges();
                        LoadData(); // Cập nhật lại danh sách trên màn hình
                        MessageBox.Show("Xóa tài khoản thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa tài khoản: " + ex.Message);
            }
        }
    }
}