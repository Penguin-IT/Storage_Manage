using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
// Đảm bảo chỉ dùng duy nhất 1 dòng using này cho Model
using StorageManage.Models;

namespace StorageManage.Models // Chuyển namespace về đây cho khớp với TaiKhoan.cs
{
    public partial class StorageManageEntities : DbContext
    {

        public StorageManageEntities() : base("name=StorageManageEntities")
        {
            Database.SetInitializer<StorageManageEntities>(null);
        }

        // Tên DbSet có chữ 's' để khớp với ViewModel
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<NhomQuyen> NhomQuyens { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<DaiLy> DaiLies { get; set; }
        public virtual DbSet<PhieuNhap> PhieuNhaps { get; set; }
        public virtual DbSet<PhieuXuat> PhieuXuats { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Dòng code "vàng" để ép Entity Framework phải nhận diện TaiKhoan
            modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoan");
        }
    }
}