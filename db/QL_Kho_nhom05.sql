---Thành viên nhóm:
---Nguyễn Hữu Nhật
---Nguyễn Gia Thịnh
---Phạm Lê Đăng Vương
---Phạm Duy Đạt


CREATE DATABASE QL_KHO
GO
USE QL_KHO
GO

-- =========================================================
-- PHẦN 1: TẠO BẢNG
-- =========================================================

---Bảng loại sản phẩm
create table LoaiSanPham
(
	MaLoai char (10) primary key not null,
	TenLoai nvarchar (100),
    GhiChu nvarchar(max) default null
);


---Bảng sản phẩm
create table SanPham
(
	MaSP char (10) not null,
	TenSP nvarchar (100),
	DVT nvarchar (10),
	SLTon int,
	MaLoai char (10),
	DonGia money,
    HinhAnh NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
	constraint PK_SanPham primary key (MaSP)
);



---Bảng nhân viên
create table NhanVien
(
	MaNV char (10) primary key not null,
	HoTen nvarchar (100),
	NgaySinh datetime,
	SDT varchar(15),
	ChucVu nvarchar(20)
);

---Bảng nhà cung cấp
create table NhaCungCap
(
	MaNCC char (10) primary key not null,
	TenNCC nvarchar (100),
	SDT varchar(15),
	DiaChi nvarchar (100),
    IsActive BIT NOT NULL DEFAULT 1,
	Email nvarchar (100)
);

---Bảng đại lý
create table DaiLy
(
	MaDL char (10) primary key not null,
	TenDL nvarchar (100),
	DiaChi nvarchar (100),
    IsActive BIT NOT NULL DEFAULT 1,
	SDT varchar(15)
);


---Bảng phiếu nhập
create table PhieuNhap
(
	MaPN char (10) primary key not null,
	NgayNhap datetime,
	TongTien money default 0,
	MaNCC char (10),
	MaNV char (10)
);

---Bảng phiếu xuất
create table PhieuXuat
(
	MaPX char (10) primary key not null,
	NgayXuat datetime,
	TongTien money default 0,
	MaDL char (10),
	MaNV char (10)
);

---Bảng chi tiết phiếu nhập
create table ChiTietPN
(
	MaPN char (10) not null,
	MaSP char (10) not null,
	SoLuong int,
	DonGiaNhap money,
	ThanhTien money,
	constraint PK_CTPN primary key (MaPN, MaSP)
);

---Bảng chi tiết phiếu xuất
create table ChiTietPX
(
	MaPX char (10) not null,
	MaSP char (10) not null,
	SoLuong int,
	DonGiaXuat money,
	ThanhTien money,
	constraint PK_CTPX primary key (MaPX, MaSP)
);

---Bảng Kho Hàng 
create table KhoHang
(
	MaKho char(10) primary key not null,
	TenKho nvarchar(100),
	DiaChi nvarchar(200)
);

---Bảng Tài Khoản 
create table TaiKhoan
(
	TenDangNhap varchar(50) primary key not null,
	MatKhau varchar(255) not null,
	MaNV char(10) not null,
	QuyenTruyCap nvarchar(50),
	TrangThai bit
);

---Bảng Phiếu Kiểm Kê 
create table PhieuKiemKe
(
	MaPKK char(10) primary key not null,
	NgayKiemKe datetime,
	MaNV char(10) not null,
	GhiChu nvarchar(255)
);

---Bảng Chi Tiết Kiểm Kê
create table ChiTietKiemKe
(
	MaPKK char(10) not null,
	MaSP char(10) not null,
	SLHeThong int,
	SLThucTe int,
	SLLech int, 
	LyDo nvarchar(255),
	constraint PK_CTKK primary key (MaPKK, MaSP)
);


-- =========================================================
-- PHẦN 2: THÊM CÁC RÀNG BUỘC (CONSTRAINTS & FOREIGN KEYS)
-- =========================================================

--- Ràng buộc UNIQUE
alter table LoaiSanPham add constraint UQ_LoaiSanPham_MaLoai unique (MaLoai);
alter table SanPham add constraint UQ_SanPham_MaSP unique (MaSP);
alter table NhanVien add constraint UQ_NhanVien_MaNV unique (MaNV);
alter table NhaCungCap add constraint UQ_NhaCungCap_MaNCC unique (MaNCC);
alter table DaiLy add constraint UQ_DaiLy_MaDL unique (MaDL);
alter table PhieuNhap add constraint UQ_PhieuNhap_MaPN unique (MaPN);
alter table PhieuXuat add constraint UQ_PhieuXuat_MaPX unique (MaPX);
alter table KhoHang add constraint UQ_KhoHang_MaKho unique (MaKho);
alter table TaiKhoan add constraint UQ_TaiKhoan_TenDangNhap unique (TenDangNhap);
alter table PhieuKiemKe add constraint UQ_PhieuKiemKe_MaPKK unique (MaPKK);

--- Ràng buộc DEFAULT
alter table SanPham add constraint DF_SanPham_SLTon default 0 for SLTon;
alter table NhanVien add constraint DF_NhanVien_ChucVu default N'Nhân Viên' for ChucVu;
alter table PhieuNhap add constraint DF_PhieuNhap_NgayNhap default getdate() for NgayNhap;
alter table PhieuXuat add constraint DF_PhieuXuat_NgayXuat default getdate() for NgayXuat;
alter table TaiKhoan add constraint DF_TaiKhoan_QuyenTruyCap default N'Nhân Viên' for QuyenTruyCap;
alter table TaiKhoan add constraint DF_TaiKhoan_TrangThai default 1 for TrangThai;
alter table PhieuKiemKe add constraint DF_PhieuKiemKe_NgayKiemKe default getdate() for NgayKiemKe;

--- Ràng buộc CHECK
alter table SanPham add constraint CK_SanPham_SLTon check (SLTon >= 0);
alter table SanPham add constraint CK_SanPham_DonGia check (DonGia > 0);
alter table PhieuNhap add constraint CK_PhieuNhap_TongTien check (TongTien >= 0);
alter table PhieuXuat add constraint CK_PhieuXuat_TongTien check (TongTien >= 0);
alter table ChiTietPN add constraint CK_CTPN_SoLuong check (SoLuong > 0);
alter table ChiTietPN add constraint CK_CTPN_DonGiaNhap check (DonGiaNhap > 0);
alter table ChiTietPN add constraint CK_CTPN_ThanhTien check (ThanhTien >= 0);
alter table ChiTietPX add constraint CK_CTPX_SoLuong check (SoLuong > 0);
alter table ChiTietPX add constraint CK_CTPX_DonGiaXuat check (DonGiaXuat > 0); 
alter table ChiTietPX add constraint CK_CTPX_ThanhTien check (ThanhTien >= 0);

--- Ràng buộc KHÓA NGOẠI (FOREIGN KEY)
alter table SanPham add constraint FK_LoaiSanPham foreign key (MaLoai) references LoaiSanPham(MaLoai);

alter table PhieuNhap add constraint FK_NhaCungCap foreign key (MaNCC) references NhaCungCap(MaNCC);
alter table PhieuNhap add constraint FK_NhanVien_PN foreign key (MaNV) references NhanVien(MaNV);

alter table PhieuXuat add constraint FK_DaiLy foreign key (MaDL) references DaiLy(MaDL);
alter table PhieuXuat add constraint FK_NhanVien_PX foreign key (MaNV) references NhanVien(MaNV);

alter table ChiTietPN add constraint FK_PhieuNhap foreign key (MaPN) references PhieuNhap(MaPN);
alter table ChiTietPN add constraint FK_SanPham_CTPN foreign key (MaSP) references SanPham(MaSP);

alter table ChiTietPX add constraint FK_PhieuXuat foreign key (MaPX) references PhieuXuat(MaPX);
alter table ChiTietPX add constraint FK_SanPham_CTPX foreign key (MaSP) references SanPham(MaSP);

alter table TaiKhoan add constraint FK_TaiKhoan_NhanVien foreign key (MaNV) references NhanVien(MaNV);

alter table PhieuKiemKe add constraint FK_KiemKe_NhanVien foreign key (MaNV) references NhanVien(MaNV);

alter table ChiTietKiemKe add constraint FK_CTKK_PhieuKiemKe foreign key (MaPKK) references PhieuKiemKe(MaPKK);
alter table ChiTietKiemKe add constraint FK_CTKK_SanPham foreign key (MaSP) references SanPham(MaSP);

-- =========================================================
-- PHẦN 3: TRIGGER THỰC THI NGHIỆP VỤ LOGIC 
-- =========================================================

GO
--- 1. Trigger tự động cộng Tồn Kho, tính Thành Tiền và Tổng Tiền khi NHẬP HÀNG
CREATE  TRIGGER trg_CapNhatPhieuNhap
ON ChiTietPN
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        UPDATE ct
        SET ct.ThanhTien = ct.SoLuong * ct.DonGiaNhap
        FROM ChiTietPN ct
        JOIN inserted i ON ct.MaPN = i.MaPN AND ct.MaSP = i.MaSP;
    END

   
    UPDATE sp
    SET sp.SLTon = sp.SLTon - ISNULL(d.SoLuong, 0) + ISNULL(i.SoLuong, 0)
    FROM SanPham sp
    LEFT JOIN deleted d ON sp.MaSP = d.MaSP
    LEFT JOIN inserted i ON sp.MaSP = i.MaSP;


    DECLARE @DanhSachMaPN TABLE (MaPN CHAR(10));
    INSERT INTO @DanhSachMaPN SELECT MaPN FROM inserted UNION SELECT MaPN FROM deleted;

    UPDATE pn
    SET pn.TongTien = (SELECT ISNULL(SUM(ThanhTien), 0) FROM ChiTietPN WHERE MaPN = pn.MaPN)
    FROM PhieuNhap pn
    WHERE pn.MaPN IN (SELECT MaPN FROM @DanhSachMaPN);
END
GO

--- 2. Trigger tự động trừ Tồn Kho, tính Thành Tiền và Tổng Tiền khi XUẤT HÀNG
CREATE  TRIGGER trg_CapNhatPhieuXuat
ON ChiTietPX
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;


    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        UPDATE ct
        SET ct.ThanhTien = ct.SoLuong * ct.DonGiaXuat
        FROM ChiTietPX ct
        JOIN inserted i ON ct.MaPX = i.MaPX AND ct.MaSP = i.MaSP;
    END


    UPDATE sp
    SET sp.SLTon = sp.SLTon + ISNULL(d.SoLuong, 0) - ISNULL(i.SoLuong, 0)
    FROM SanPham sp
    LEFT JOIN deleted d ON sp.MaSP = d.MaSP
    LEFT JOIN inserted i ON sp.MaSP = i.MaSP;

 
    DECLARE @DanhSachMaPX TABLE (MaPX CHAR(10));
    INSERT INTO @DanhSachMaPX SELECT MaPX FROM inserted UNION SELECT MaPX FROM deleted;

    UPDATE px
    SET px.TongTien = (SELECT ISNULL(SUM(ThanhTien), 0) FROM ChiTietPX WHERE MaPX = px.MaPX)
    FROM PhieuXuat px
    WHERE px.MaPX IN (SELECT MaPX FROM @DanhSachMaPX);
END
GO

--- 3. Trigger tự động tính Số Lượng Lệch và Cập nhật Tồn Kho khi KIỂM KÊ
CREATE  TRIGGER trg_CapNhatKiemKe
ON ChiTietKiemKe
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

   
    UPDATE ct
    SET ct.SLLech = i.SLThucTe - i.SLHeThong
    FROM ChiTietKiemKe ct
    JOIN inserted i ON ct.MaPKK = i.MaPKK AND ct.MaSP = i.MaSP;


    UPDATE sp
    SET sp.SLTon = i.SLThucTe
    FROM SanPham sp
    JOIN inserted i ON sp.MaSP = i.MaSP;
END
GO
-- =========================================================
-- PHẦN 4: THÊM DỮ LIỆU MẪU
-- =========================================================


insert into LoaiSanPham (MaLoai, TenLoai) values
('L01', N'Đồ uống'), ('L02', N'Bánh kẹo'), ('L03', N'Gia vị'), ('L04', N'Đồ hộp'),
('L05', N'Sữa'), ('L06', N'Mỹ phẩm'), ('L07', N'Văn phòng phẩm'), ('L08', N'Đồ gia dụng');


insert into SanPham (MaSP, TenSP, DVT, SLTon, MaLoai, DonGia) values
('SP01', N'Nước suối Lavie', N'Chai', 100, 'L01', 5000),
('SP02', N'Coca Cola', N'Lon', 80, 'L01', 10000),
('SP03', N'Bánh Oreo', N'Gói', 60, 'L02', 15000),
('SP04', N'Muối i-ốt', N'Gói', 40, 'L03', 8000),
('SP05', N'Cá hộp 3 cô gái', N'Hộp', 30, 'L04', 25000),
('SP06', N'Sữa Vinamilk', N'Hộp', 90, 'L05', 12000),
('SP07', N'Bút bi Thiên Long', N'Cây', 200, 'L07', 5000),
('SP08', N'Nước rửa chén', N'Chai', 50, 'L08', 30000);

insert into NhanVien values
('NV01', N'Nguyễn Văn A', '1999-05-10', '0912345678', N'Quản Lý'),
('NV02', N'Trần Thị B', '2000-08-15', '0934567890', N'Nhân Viên'),
('NV03', N'Lê Văn C', '1998-02-20', '0965432187', N'Nhân Viên'),
('NV04', N'Phạm Thị D', '2001-11-30', '0978654321', N'Nhân Viên'),
('NV05', N'Hoàng Văn E', '1997-06-25', '0987654321', N'Nhân Viên'),
('NV06', N'Đặng Thị F', '1999-09-18', '0901234567', N'Nhân Viên'),
('NV07', N'Bùi Văn G', '2000-01-05', '0923456789', N'Nhân Viên'),
('NV08', N'Võ Thị H', '2002-04-12', '0956789012', N'Nhân Viên');


insert into NhaCungCap (MaNCC, TenNCC, SDT, DiaChi, Email) values
('NCC01', N'Công ty An Phát', '0912345111', N'Hà Nội', 'anphat@gmail.com'),
('NCC02', N'Công ty Minh Long', '0912345222', N'TP.HCM', 'minhlong@gmail.com'),
('NCC03', N'Công ty Hòa Bình', '0912345333', N'Đà Nẵng', 'hoabinh@gmail.com'),
('NCC04', N'Công ty Thành Công', '0912345444', N'Hải Phòng', 'thanhcong@gmail.com'),
('NCC05', N'Công ty Việt Nhật', '0912345555', N'Cần Thơ', 'vietnhat@gmail.com'),
('NCC06', N'Công ty Đại Phát', '0912345666', N'Bình Dương', 'daiphat@gmail.com'),
('NCC07', N'Công ty Tân Tiến', '0912345777', N'Đồng Nai', 'tantien@gmail.com'),
('NCC08', N'Công ty Phú Quý', '0912345888', N'Long An', 'phuquy@gmail.com');


insert into DaiLy (MaDL, TenDL, DiaChi, SDT) values
('DL01', N'Đại lý Minh Châu', N'Quận 1', '0934111111'),
('DL02', N'Đại lý Hồng Phát', N'Quận 3', '0934222222'),
('DL03', N'Đại lý Tân Lợi', N'Quận 5', '0934333333'),
('DL04', N'Đại lý Gia Bảo', N'Quận 7', '0934444444'),
('DL05', N'Đại lý Hoàng Long', N'Tân Bình', '0934555555'),
('DL06', N'Đại lý Phúc An', N'Gò Vấp', '0934666666'),
('DL07', N'Đại lý Thịnh Phát', N'Bình Thạnh', '0934777777'),
('DL08', N'Đại lý Đại Phát', N'Thủ Đức', '0934888888');

--- Dữ liệu bảng KhoHang
insert into KhoHang values
('K01', N'Kho Tổng', N'Quận Tân Phú, TP.HCM'),
('K02', N'Kho Trung Chuyển', N'Quận 9, TP.HCM');

--- Dữ liệu bảng TaiKhoan
insert into TaiKhoan values
('admin', '123456', 'NV01', N'Quản Lý', 1),
('nv_kho1', '123456', 'NV02', N'Nhân Viên', 1),
('nv_kho2', '123456', 'NV03', N'Nhân Viên', 1);

insert into PhieuNhap (MaPN, TongTien, MaNCC, MaNV) values
('PN01', 0, 'NCC01', 'NV01'), ('PN02', 0, 'NCC02', 'NV02'),
('PN03', 0, 'NCC03', 'NV03'), ('PN04', 0, 'NCC04', 'NV04'),
('PN05', 0, 'NCC05', 'NV05'), ('PN06', 0, 'NCC06', 'NV06'),
('PN07', 0, 'NCC07', 'NV07'), ('PN08', 0, 'NCC08', 'NV08');

insert into PhieuXuat (MaPX, TongTien, MaDL, MaNV) values
('PX01', 0, 'DL01', 'NV01'), ('PX02', 0, 'DL02', 'NV02'),
('PX03', 0, 'DL03', 'NV03'), ('PX04', 0, 'DL04', 'NV04'),
('PX05', 0, 'DL05', 'NV05'), ('PX06', 0, 'DL06', 'NV06'),
('PX07', 0, 'DL07', 'NV07'), ('PX08', 0, 'DL08', 'NV08');

insert into ChiTietPN values
('PN01', 'SP01', 20, 5000, 100000),
('PN02', 'SP02', 30, 9000, 270000),
('PN03', 'SP03', 15, 14000, 210000),
('PN04', 'SP04', 25, 7000, 175000),
('PN05', 'SP05', 10, 24000, 240000),
('PN06', 'SP06', 40, 11000, 440000),
('PN07', 'SP07', 50, 4500, 225000),
('PN08', 'SP08', 12, 28000, 336000);

insert into ChiTietPX values
('PX01', 'SP01', 10, 6000, 60000),
('PX02', 'SP02', 15, 11000, 165000),
('PX03', 'SP03', 8, 16000, 128000),
('PX04', 'SP04', 12, 9000, 108000),
('PX05', 'SP05', 5, 30000, 150000),
('PX06', 'SP06', 20, 15000, 300000),
('PX07', 'SP07', 30, 6000, 180000),
('PX08', 'SP08', 7, 35000, 245000);

--- Dữ liệu bảng PhieuKiemKe
insert into PhieuKiemKe values
('PKK01', '2026-05-01', 'NV01', N'Kiểm kê đầu tháng'),
('PKK02', '2026-05-15', 'NV01', N'Kiểm kê ngẫu nhiên');

--- Dữ liệu bảng ChiTietKiemKe
insert into ChiTietKiemKe values
('PKK01', 'SP01', 110, 108, -2, N'Hư hỏng nhãn dán'),
('PKK01', 'SP02', 95, 95, 0, N'Bình thường'),
('PKK02', 'SP03', 67, 65, -2, N'Mất nắp chai');
-- =========================================================
-- PHẦN 5: VIEWS, PROCEDURES, FUNCTIONS, CURSORS
-- =========================================================
----------------------------VIEW----------------------------------
GO
CREATE VIEW vw_ThongTinSanPham AS
SELECT sp.MaSP, sp.TenSP, lsp.TenLoai, sp.DVT, sp.SLTon, sp.DonGia, (sp.SLTon * sp.DonGia) AS TongGiaTriTien
FROM SanPham sp JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai;
GO

CREATE VIEW vw_BaoCaoXuatNhapTon AS
SELECT sp.MaSP, sp.TenSP, lsp.TenLoai,
    ISNULL(Nhap.TongSLNhap, 0) AS TongSoLuongNhap,
    ISNULL(Xuat.TongSLXuat, 0) AS TongSoLuongXuat,
    sp.SLTon AS SoLuongTonKhoThucTe
FROM SanPham sp
JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai
LEFT JOIN (SELECT MaSP, SUM(SoLuong) AS TongSLNhap FROM ChiTietPN GROUP BY MaSP) AS Nhap ON sp.MaSP = Nhap.MaSP
LEFT JOIN (SELECT MaSP, SUM(SoLuong) AS TongSLXuat FROM ChiTietPX GROUP BY MaSP) AS Xuat ON sp.MaSP = Xuat.MaSP;
GO

CREATE VIEW vw_ChiTietPhieuNhap AS
SELECT pn.MaPN, pn.NgayNhap, nv.HoTen AS TenNhanVien, ncc.TenNCC AS TenNhaCungCap, sp.TenSP, ctpn.SoLuong, ctpn.DonGiaNhap, ctpn.ThanhTien
FROM PhieuNhap pn
JOIN NhanVien nv ON pn.MaNV = nv.MaNV
JOIN NhaCungCap ncc ON pn.MaNCC = ncc.MaNCC
JOIN ChiTietPN ctpn ON pn.MaPN = ctpn.MaPN
JOIN SanPham sp ON ctpn.MaSP = sp.MaSP;
GO

CREATE VIEW vw_ChiTietPhieuXuat AS
SELECT px.MaPX, px.NgayXuat, nv.HoTen AS TenNhanVien, dl.TenDL AS TenDaiLy, dl.DiaChi AS DiaChiDaiLy, sp.TenSP, ctpx.SoLuong, 
       ctpx.DonGiaXuat,
	   ctpx.ThanhTien
FROM PhieuXuat px
JOIN NhanVien nv ON px.MaNV = nv.MaNV
JOIN DaiLy dl ON px.MaDL = dl.MaDL
JOIN ChiTietPX ctpx ON px.MaPX = ctpx.MaPX
JOIN SanPham sp ON ctpx.MaSP = sp.MaSP;
GO

CREATE VIEW vw_NhaCungCapTheoSanPham AS
SELECT DISTINCT
    sp.MaSP AS [Mã Sản Phẩm],
    sp.TenSP AS [Tên Sản Phẩm],
    sp.SLTon AS [Số Lượng Tồn],
    ncc.TenNCC AS [Tên Nhà Cung Cấp],
    ncc.SDT AS [Số Điện Thoại NCC],
    ncc.DiaChi AS [Địa Chỉ]
FROM SanPham sp
JOIN ChiTietPN ctpn ON sp.MaSP = ctpn.MaSP
JOIN PhieuNhap pn ON ctpn.MaPN = pn.MaPN
JOIN NhaCungCap ncc ON pn.MaNCC = ncc.MaNCC;
GO
----------------------------PROCEDURE----------------------------------
CREATE PROCEDURE sp_TinhTienThuong
    @LuongCoBan MONEY, @HeSo FLOAT, @TienThuong MONEY OUTPUT 
AS BEGIN SET @TienThuong = @LuongCoBan * @HeSo; END
GO

CREATE PROCEDURE sp_LayTongTienPhieuNhap
    @manv VARCHAR(10), @mancc VARCHAR(10), @mapn VARCHAR(10), @tongtien MONEY OUTPUT
AS BEGIN
    SELECT @tongtien = TongTien FROM PhieuNhap WHERE MaPN = @mapn AND MaNV = @manv AND MaNCC = @mancc;
END
GO

CREATE PROCEDURE sp_TongSoLuongSanPhamTheoPhieuXuat
    @mapx VARCHAR(10)
AS BEGIN
    SELECT sp.MaSP, sp.TenSP, SUM(ctpx.SoLuong) AS TongSoLuong
    FROM ChiTietPX ctpx JOIN SanPham sp ON ctpx.MaSP = sp.MaSP
    WHERE ctpx.MaPX = @mapx GROUP BY sp.MaSP, sp.TenSP;
END
GO

CREATE PROCEDURE sp_TongTonKhoTheoLoai
    @MaLoai CHAR(10), @TongSoLuong INT OUTPUT  
AS BEGIN
    IF NOT EXISTS (SELECT 1 FROM LoaiSanPham WHERE MaLoai = @MaLoai)
    BEGIN SET @TongSoLuong = 0; RETURN; END
    SELECT @TongSoLuong = SUM(SLTon) FROM SanPham WHERE MaLoai = @MaLoai;
    IF @TongSoLuong IS NULL SET @TongSoLuong = 0;
END
GO

GO
CREATE PROCEDURE sp_InsertChiTietPX
    @MaPX char(10),
    @MaSP char(10),
    @SoLuong int,
    @DonGiaXuat money
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SLTonHienTai INT;
    SELECT @SLTonHienTai = SLTon FROM SanPham WHERE MaSP = @MaSP;

    IF @SLTonHienTai < @SoLuong
    BEGIN
        RAISERROR(N'Lỗi: Kho không đủ hàng! Hiện chỉ còn %d sản phẩm.', 16, 1, @SLTonHienTai);
        RETURN;
    END

    DECLARE @ThanhTien money = @SoLuong * @DonGiaXuat;
    INSERT INTO ChiTietPX (MaPX, MaSP, SoLuong, DonGiaXuat, ThanhTien)
    VALUES (@MaPX, @MaSP, @SoLuong, @DonGiaXuat, @ThanhTien);

    PRINT N'Thêm chi tiết phiếu xuất thành công!';
END
GO

GO
CREATE PROCEDURE sp_UpdateNhaCungCap
    @MaNCC   CHAR(10),
    @TenNCC  NVARCHAR(100) = NULL,
    @SDT     VARCHAR(15)   = NULL,
    @DiaChi  NVARCHAR(100) = NULL,
    @Email   NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;


    IF NOT EXISTS (SELECT 1 FROM NhaCungCap WHERE MaNCC = @MaNCC)
    BEGIN
        RAISERROR(N'Loi: Ma nha cung cap %s khong ton tai.', 16, 1, @MaNCC);
        RETURN;
    END


    UPDATE NhaCungCap
    SET
        TenNCC = ISNULL(@TenNCC, TenNCC),
        SDT    = ISNULL(@SDT,    SDT),
        DiaChi = ISNULL(@DiaChi, DiaChi),
        Email  = ISNULL(@Email,  Email)
    WHERE MaNCC = @MaNCC;

    PRINT N'Cap nhat nha cung cap ' + @MaNCC + N' thanh cong.';
END
GO
----------------------------FUNCTION----------------------------------
CREATE FUNCTION fn_PhieuNhapTheoKhoangThoiGian (@NgayBatDau DATETIME, @NgayKetThuc DATETIME)
RETURNS TABLE AS RETURN
(
    SELECT pn.MaPN, pn.NgayNhap, nv.HoTen AS TenNhanVien, ncc.TenNCC AS TenNhaCungCap, sp.TenSP, ctpn.SoLuong, ctpn.DonGiaNhap, ctpn.ThanhTien
    FROM PhieuNhap pn
    JOIN NhanVien nv ON pn.MaNV = nv.MaNV JOIN NhaCungCap ncc ON pn.MaNCC = ncc.MaNCC
    JOIN ChiTietPN ctpn ON pn.MaPN = ctpn.MaPN JOIN SanPham sp ON ctpn.MaSP = sp.MaSP
    WHERE pn.NgayNhap BETWEEN @NgayBatDau AND @NgayKetThuc
);
GO

CREATE FUNCTION fn_ThongKeSanPhamTheoLoai (@MaLoai CHAR(10))
RETURNS TABLE AS RETURN
(
    SELECT sp.MaSP, sp.TenSP, sp.DVT, sp.DonGia, ISNULL(Nhap.TongSLNhap, 0) AS TongSLNhap, ISNULL(Xuat.TongSLXuat, 0) AS TongSLXuat, sp.SLTon AS SLTonThucTe
    FROM SanPham sp
    LEFT JOIN (SELECT MaSP, SUM(SoLuong) AS TongSLNhap FROM ChiTietPN GROUP BY MaSP) AS Nhap ON sp.MaSP = Nhap.MaSP
    LEFT JOIN (SELECT MaSP, SUM(SoLuong) AS TongSLXuat FROM ChiTietPX GROUP BY MaSP) AS Xuat ON sp.MaSP = Xuat.MaSP
    WHERE sp.MaLoai = @MaLoai
);
GO

CREATE FUNCTION fn_LichSuGiaoDichNhanVien (@MaNV CHAR(10))
RETURNS @KetQua TABLE (LoaiPhieu NVARCHAR(10), MaPhieu CHAR(10), NgayLap DATETIME, DoiTac NVARCHAR(100), TongTien MONEY)
AS BEGIN
    IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV) RETURN;
    INSERT INTO @KetQua (LoaiPhieu, MaPhieu, NgayLap, DoiTac, TongTien)
    SELECT N'Nhập', pn.MaPN, pn.NgayNhap, ncc.TenNCC, pn.TongTien FROM PhieuNhap pn JOIN NhaCungCap ncc ON pn.MaNCC = ncc.MaNCC WHERE pn.MaNV = @MaNV;
    
    INSERT INTO @KetQua (LoaiPhieu, MaPhieu, NgayLap, DoiTac, TongTien)
    SELECT N'Xuất', px.MaPX, px.NgayXuat, dl.TenDL, px.TongTien FROM PhieuXuat px JOIN DaiLy dl ON px.MaDL = dl.MaDL WHERE px.MaNV = @MaNV;
    RETURN;
END;
GO


----------------------------CURSOR----------------------------------
CREATE PROCEDURE sp_BaoCaoGiaTriTonKhoTheoLoai_Cursor
AS
BEGIN
    DECLARE @MALOAI char(10), @TENLOAI nvarchar(100), @TONGGIATRI decimal(18)
    DECLARE cs_BaoCaoGiaTriTonKhoTheoLoai CURSOR FOR
        SELECT MaLoai, TenLoai FROM LoaiSanPham

    OPEN cs_BaoCaoGiaTriTonKhoTheoLoai
    FETCH NEXT FROM cs_BaoCaoGiaTriTonKhoTheoLoai INTO @MALOAI, @TENLOAI

    WHILE(@@FETCH_STATUS = 0)
    BEGIN
        SELECT @TONGGIATRI = SUM(SLTon * DonGia) FROM SanPham WHERE MaLoai = @MALOAI
        IF @TONGGIATRI IS NULL SET @TONGGIATRI = 0

        IF @TONGGIATRI = 0
            PRINT N'Loại sản phẩm: ' + @TENLOAI + N' - Không có hàng tồn kho.'
        ELSE
            PRINT N'Loại sản phẩm: ' + @TENLOAI + N' - Tổng giá trị tồn: ' + CAST(@TONGGIATRI AS NVARCHAR(50)) + N' VNĐ'

        FETCH NEXT FROM cs_BaoCaoGiaTriTonKhoTheoLoai INTO @MALOAI, @TENLOAI
    END

    CLOSE cs_BaoCaoGiaTriTonKhoTheoLoai
    DEALLOCATE cs_BaoCaoGiaTriTonKhoTheoLoai
END
GO

CREATE PROCEDURE sp_XetThuongNhanVienXuatKho_Cursor
AS
BEGIN
    DECLARE @MANV char(10), @HOTEN nvarchar(100), @TONGTIEN decimal(18,2),@TIENTHUONG decimal(18,2)
    DECLARE cs_XetThuongNhanVienXuatKho CURSOR FOR
        SELECT MaNV,HoTen FROM NhanVien

    OPEN cs_XetThuongNhanVienXuatKho
    FETCH NEXT FROM cs_XetThuongNhanVienXuatKho INTO @MANV,@HOTEN
    WHILE(@@FETCH_STATUS=0)
    BEGIN
        SELECT @TONGTIEN = SUM(TongTien)
        FROM PhieuXuat
        WHERE MaNV=@MANV

        IF @TONGTIEN IS NULL SET @TONGTIEN=0
        IF @TONGTIEN >=1000000
        BEGIN
            SET @TIENTHUONG=@TONGTIEN*0.1
        END
        ELSE IF @TONGTIEN <500000
        BEGIN
            SET @TIENTHUONG =0
        END
        ELSE
            SET @TIENTHUONG=@TONGTIEN*0.05

        PRINT N'Nhân viên: '+@HOTEN+ N' - Doanh số xuất : '+CAST(@TONGTIEN AS Nvarchar(50))+ N'VNĐ'+ N' - Tiền thưởng :'+ CAST(@TIENTHUONG AS Nvarchar(50))+ N'VNĐ'
        FETCH NEXT FROM cs_XetThuongNhanVienXuatKho INTO @MANV,@HOTEN
    END
    CLOSE cs_XetThuongNhanVienXuatKho
    DEALLOCATE cs_XetThuongNhanVienXuatKho
END

----------------------------SAO LUU----------------------------------
 
ALTER DATABASE QL_KHO SET RECOVERY SIMPLE;
GO
 
BACKUP DATABASE QL_KHO
TO DISK = N'D:\BT Nhóm đại học\HQT - CSDL\QL_KHO_Simple_Full.bak'
WITH
    NAME        = N'QL_KHO - Simple Full Backup',
    DESCRIPTION = N'Sao luu toan bo - che do Simple',
    FORMAT,
    INIT,
    STATS = 10;
GO
 
BACKUP DATABASE QL_KHO
TO DISK = N'D:\BT Nhóm đại học\HQT - CSDL\QL_KHO_Simple_Diff.bak'
WITH
    NAME        = N'QL_KHO - Simple Differential Backup',
    DESCRIPTION = N'Sao luu vi sai - che do Simple',
    DIFFERENTIAL,
    NOINIT,
    STATS = 10;
GO


ALTER DATABASE QL_KHO SET RECOVERY FULL;
GO
 
BACKUP DATABASE QL_KHO
TO DISK = N'D:\BT Nhóm đại học\HQT - CSDL\QL_KHO_Full_Full.bak'
WITH
    NAME        = N'QL_KHO - Full Recovery Full Backup',
    DESCRIPTION = N'Sao luu toan bo - che do Full Recovery',
    FORMAT,
    INIT,
    STATS = 10;
GO
 

BACKUP DATABASE QL_KHO
TO DISK = N'D:\BT Nhóm đại học\HQT - CSDL\QL_KHO_Full_Diff.bak'
WITH
    NAME        = N'QL_KHO - Full Recovery Differential Backup',
    DESCRIPTION = N'Sao luu vi sai - che do Full Recovery',
    DIFFERENTIAL,
    NOINIT,
    STATS = 10;
GO
 

BACKUP LOG QL_KHO
TO DISK = N'D:\BT Nhóm đại học\HQT - CSDL\QL_KHO_Full_Log.bak'
WITH
    NAME        = N'QL_KHO - Full Recovery Log Backup',
    DESCRIPTION = N'Sao luu nhat ky giao dich - che do Full Recovery',
    NOINIT,
    STATS = 10;
GO

-- =========================================================
-- PHẦN 6: THỰC THI CÁC HÀM, THỦ TỤC, VIEW
-- =========================================================

SELECT * FROM vw_ThongTinSanPham;
GO

SELECT * FROM vw_BaoCaoXuatNhapTon;
GO

SELECT * FROM vw_ChiTietPhieuNhap;
GO

SELECT * FROM vw_ChiTietPhieuXuat;
GO

DECLARE @TongTienThuong MONEY;
EXEC sp_TinhTienThuong @LuongCoBan = 15000000, @HeSo = 1.5, @TienThuong = @TongTienThuong OUTPUT;
SELECT @TongTienThuong AS TongTienThuong;
GO

DECLARE @TongTienPhieuNhap MONEY;
EXEC sp_LayTongTienPhieuNhap @manv = 'NV01', @mancc = 'NCC01', @mapn = 'PN01', @tongtien = @TongTienPhieuNhap OUTPUT;
SELECT @TongTienPhieuNhap AS TongTienPhieuNhap;
GO

EXEC sp_TongSoLuongSanPhamTheoPhieuXuat @mapx = 'PX01';
GO

DECLARE @TongTon INT;
EXEC sp_TongTonKhoTheoLoai @MaLoai = 'L01', @TongSoLuong = @TongTon OUTPUT;
SELECT @TongTon AS TongTonKhoTheoLoai;
GO

SELECT * FROM fn_PhieuNhapTheoKhoangThoiGian('2024-01-01', '2026-12-31');
GO

SELECT * FROM fn_ThongKeSanPhamTheoLoai('L01');
GO

SELECT * FROM fn_LichSuGiaoDichNhanVien('NV01');
GO

INSERT INTO ChiTietPN (MaPN, MaSP, SoLuong, DonGiaNhap, ThanhTien) 
VALUES ('PN02', 'SP02', 100, 9000, 900000);
GO

SELECT MaSP, TenSP, SLTon FROM SanPham WHERE MaSP = 'SP02';
GO

SELECT MaPN, TongTien FROM PhieuNhap WHERE MaPN = 'PN02';
GO

EXEC sp_BaoCaoGiaTriTonKhoTheoLoai_Cursor;
GO

EXEC sp_XetThuongNhanVienXuatKho_Cursor;
GO

-- Gọi thử với mã không tồn tại → trả về bảng rỗng
SELECT *
FROM fn_LichSuGiaoDichNhanVien('NV99');
GO



GO
DELETE FROM ChiTietPX WHERE MaPX = 'PX09' AND MaSP = 'SP03';
EXEC sp_InsertChiTietPX @MaPX = 'PX09', @MaSP = 'SP03', @SoLuong = 5, @DonGiaXuat = 16000;
GO


-- Cap nhat toan bo thong tin
EXEC sp_UpdateNhaCungCap
    @MaNCC  = 'NCC01',
    @TenNCC = N'Công ty An Phát Mới',
    @SDT    = '0912999111',
    @DiaChi = N'Hà Nội - Quận Cầu Giấy',
    @Email  = 'anphatmoi@gmail.com';
GO

-- Chi cap nhat SDT va Email, giu nguyen phan con lai
EXEC sp_UpdateNhaCungCap
    @MaNCC = 'NCC02',
    @SDT   = '0999888777',
    @Email = 'minhlong_new@gmail.com';
GO

-- Kiem tra ket qua
SELECT * FROM NhaCungCap WHERE MaNCC IN ('NCC01', 'NCC02');
GO

-- =========================================================
-- PHẦN 7: TẠO TÀI KHOẢN HỆ THỐNG VÀ PHÂN QUYỀN (SECURITY)
-- =========================================================
USE QL_KHO
GO


----------------------------TẠO LOGIN---------------------------------

CREATE LOGIN Login_QuanLy WITH PASSWORD = 'Password123';
GO


CREATE LOGIN Login_NhanVien WITH PASSWORD = 'Password456';
GO


----------------------------TẠO USER---------------------------------

CREATE USER User_QuanLy FOR LOGIN Login_QuanLy;
GO

CREATE USER User_NhanVien FOR LOGIN Login_NhanVien;
GO


----------------------------PHÂN QUYỀN-------------------------------

ALTER ROLE db_owner ADD MEMBER User_QuanLy;
GO

GRANT SELECT ON SanPham TO User_NhanVien;
GRANT SELECT ON LoaiSanPham TO User_NhanVien;
GRANT SELECT ON KhoHang TO User_NhanVien;

GRANT SELECT, INSERT, UPDATE ON PhieuNhap TO User_NhanVien;
GRANT SELECT, INSERT, UPDATE ON ChiTietPN TO User_NhanVien;
GRANT SELECT, INSERT, UPDATE ON PhieuXuat TO User_NhanVien;
GRANT SELECT, INSERT, UPDATE ON ChiTietPX TO User_NhanVien;

GRANT SELECT, INSERT, UPDATE ON PhieuKiemKe TO User_NhanVien;
GRANT SELECT, INSERT, UPDATE ON ChiTietKiemKe TO User_NhanVien;

GRANT SELECT ON vw_ThongTinSanPham TO User_NhanVien;
GRANT SELECT ON vw_BaoCaoXuatNhapTon TO User_NhanVien;
GO

-- =========================================================
-- PHẦN 8: GIAO TÁC VÀ MỨC ĐỘ CÔ LẬP 
-- =========================================================
GO


CREATE PROCEDURE sp_GiaoTac_XoaPhieuXuat
    @MaPX CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; 
    
    BEGIN TRY
        BEGIN TRAN;

        DELETE FROM ChiTietPX WHERE MaPX = @MaPX;
        DELETE FROM PhieuXuat WHERE MaPX = @MaPX;

        COMMIT TRAN;
        PRINT N'Đã xóa Phiếu xuất an toàn (Không bị Phantom Read).';
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (N'Lỗi khi xóa Phiếu Xuất: %s', 16, 1, @ErrorMessage);
    END CATCH
END
GO


CREATE  PROCEDURE sp_GiaoTac_XoaPhieuNhap
    @MaPN CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    
    BEGIN TRY
        BEGIN TRAN; 

        DELETE FROM ChiTietPN WHERE MaPN = @MaPN;
        DELETE FROM PhieuNhap WHERE MaPN = @MaPN;

        COMMIT TRAN; 
        PRINT N'Đã xóa Phiếu nhập an toàn (Không bị Phantom Read).';
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN; 
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (N'Lỗi khi xóa Phiếu Nhập: %s', 16, 1, @ErrorMessage);
    END CATCH
END
GO


CREATE PROCEDURE sp_GiaoTac_ThemChiTietPhieuXuat
    @MaPX CHAR(10),
    @MaSP CHAR(10),
    @SoLuong INT,
    @DonGiaXuat MONEY
AS
BEGIN
    SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
    
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @SLTonHienTai INT;

        SELECT @SLTonHienTai = SLTon FROM SanPham WHERE MaSP = @MaSP; 

        IF @SLTonHienTai < @SoLuong
        BEGIN
            RAISERROR(N'Kho không đủ hàng! Tồn kho hiện tại: %d.', 16, 1, @SLTonHienTai);
        END

        INSERT INTO ChiTietPX (MaPX, MaSP, SoLuong, DonGiaXuat)
        VALUES (@MaPX, @MaSP, @SoLuong, @DonGiaXuat);

        COMMIT TRAN;
        PRINT N'Xuất kho thành công, dữ liệu an toàn khỏi Lost Update!';
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMsg, 16, 1);
    END CATCH
END
GO


CREATE  PROCEDURE sp_GiaoTac_XemTonKhoAnToan
    @MaSP CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 

    BEGIN TRY
        BEGIN TRAN;
        
  
        SELECT MaSP, TenSP, SLTon 
        FROM SanPham 
        WHERE MaSP = @MaSP;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
    END CATCH
END
GO