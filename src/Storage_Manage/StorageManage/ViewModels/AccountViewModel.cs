using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using StorageManage.Models;

namespace StorageManage.ViewModels
{
    public class UserTemp
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }

    public class AccountViewModel : BaseViewModel
    {
        private ObservableCollection<UserTemp> _listUser;
        public ObservableCollection<UserTemp> ListUser { get => _listUser; set { _listUser = value; OnPropertyChanged(nameof(ListUser)); } }

        private UserTemp _selectedUser;
        public UserTemp SelectedUser { get => _selectedUser; set { _selectedUser = value; OnPropertyChanged(nameof(SelectedUser)); } }

        private string _newUsername;
        public string NewUsername { get => _newUsername; set { _newUsername = value; OnPropertyChanged(nameof(NewUsername)); } }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(nameof(NewPassword)); } }

        private string _selectedRole;
        public string SelectedRole { get => _selectedRole; set { _selectedRole = value; OnPropertyChanged(nameof(SelectedRole)); } }

        public ICommand AddAccountCommand { get; set; }
        public ICommand DeleteAccountCommand { get; set; }

        public AccountViewModel()
        {
            LoadData();
            AddAccountCommand = new RelayCommand((p) => ExecuteAddAccount());
            DeleteAccountCommand = new RelayCommand((p) => ExecuteDeleteAccount());
        }

        void LoadData()
        {
            try
            {
                using (var db = new QLKEntities())
                {
                    var accountList = db.TaiKhoans.ToList();

                    var data = accountList.Select(tk => new UserTemp
                    {
                        Username = tk.TenDangNhap,
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

        void ExecuteAddAccount()
        {
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
                using (var db = new QLKEntities())
                {
                    var existingAcc = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == NewUsername);
                    if (existingAcc != null)
                    {
                        MessageBox.Show("Tên đăng nhập này đã tồn tại trong hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var quyen = db.NhomQuyens.FirstOrDefault(q => q.TenQuyen == SelectedRole);

                    string maNVMoi = "NV_" + NewUsername;
                    if (maNVMoi.Length > 10)
                    {
                        maNVMoi = maNVMoi.Substring(0, 10);
                    }

                    var existingEmp = db.NhanViens.FirstOrDefault(x => x.MaNV == maNVMoi);
                    NhanVien empToLink;

                    if (existingEmp != null)
                    {
                        empToLink = existingEmp;
                        if (quyen != null) empToLink.MaQuyen = quyen.MaQuyen;
                    }
                    else
                    {
                        empToLink = new NhanVien()
                        {
                            MaNV = maNVMoi,
                            HoTen = NewUsername,
                            MaQuyen = quyen?.MaQuyen
                        };
                        db.NhanViens.Add(empToLink);
                    }

                    var newAcc = new TaiKhoan()
                    {
                        TenDangNhap = NewUsername,
                        MatKhau = NewPassword,
                        MaNV = empToLink.MaNV,
                        TrangThai = true
                    };
                    db.TaiKhoans.Add(newAcc);

                    db.SaveChanges();

                    NewUsername = "";
                    NewPassword = "";

                    LoadData();
                    MessageBox.Show("Thêm tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
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
            catch (Exception ex)
            {
                string detail = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show("Lỗi hệ thống: " + detail, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
                using (var db = new QLKEntities())
                {
                    var acc = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == SelectedUser.Username);
                    if (acc != null)
                    {
                        string maNV = acc.MaNV;

                        db.TaiKhoans.Remove(acc);

                        if (!string.IsNullOrEmpty(maNV))
                        {
                            var emp = db.NhanViens.FirstOrDefault(x => x.MaNV == maNV);
                            if (emp != null)
                            {
                                bool hasHistory = db.PhieuNhaps.Any(p => p.MaNV == maNV) || db.PhieuXuats.Any(p => p.MaNV == maNV);

                                if (!hasHistory)
                                {
                                    db.NhanViens.Remove(emp);
                                }
                            }
                        }

                        db.SaveChanges();
                        LoadData();
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