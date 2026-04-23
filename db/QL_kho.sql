

create database QL_KHO
GO
USE QL_KHO
GO

---Bảng loại sản phẩm
create table LoaiSanPham
(
	MaLoai char (10) primary key not null,
	TenLoai nvarchar (100)
);

---Bảng sản phẩm
create table SanPham
(
	MaSP char (10) not null,
	TenSP nvarchar (100),
	DVT nvarchar (10),
	SLTon int,
	MaLoai char (10),
	DonGia int,
	constraint PK_SanPham primary key (MaSP),
	constraint FK_LoaiSanPham foreign key (MaLoai) references LoaiSanPham(MaLoai)
);

--Bảng phân quyền
CREATE TABLE NhomQuyen
(
    MaQuyen CHAR(10) PRIMARY KEY NOT NULL,
    TenQuyen NVARCHAR(50) NOT NULL
);
---Bảng nhân viên
create table NhanVien
(
	MaNV char (10) primary key not null,
	HoTen nvarchar (100),
	NgaySinh datetime,
	SDT int,
	MaQuyen char (10),
    constraint FK_NhanVien_NhomQuyen foreign key (MaQuyen) references NhomQuyen(MaQuyen)
);

---Bảng nhà cung cấp
create table NhaCungCap
(
	MaNCC char (10) primary key not null,
	TenNCC nvarchar (100),
	SDT int,
	DiaChi nvarchar (100),
	Email nvarchar (100)
);

---Bảng đại lý
create table DaiLy
(
	MaDL char (10) primary key not null,
	TenDL nvarchar (100),
	DiaChi nvarchar (100),
	SDT int
);

---Bảng phiếu nhập
create table PhieuNhap
(
	MaPN char (10) primary key not null,
	NgayNhap datetime,
	TongTien money,
	MaNCC char (10),
	MaNV char (10),
	constraint FK_NhaCungCap foreign key (MaNCC) references NhaCungCap(MaNCC),
	constraint FK_NhanVien_PN foreign key (MaNV) references NhanVien(MaNV)
);

---Bảng phiếu xuất
create table PhieuXuat
(
	MaPX char (10) primary key not null,
	NgayXuat datetime,
	TongTien money,
	MaDL char (10),
	MaNV char (10),
	constraint FK_DaiLy foreign key (MaDL) references DaiLy(MaDL),
	constraint FK_NhanVien_PX foreign key (MaNV) references NhanVien(MaNV)
);

---Bảng chi tiết phiếu nhập
create table ChiTietPN
(
	MaPN char (10) not null ,
	MaSP char (10) not null,
	SoLuong int,
	DonGiaNhap money,
	ThanhTien money,
	constraint PK_CTPN primary key (MaPN, MaSP),
	constraint FK_PhieuNhap foreign key (MaPN) references PhieuNhap(MaPN),
	constraint FK_SanPham_CTPN foreign key (MaSP) references SanPham(MaSP)
);

---Bảng chi tiết phiếu xuất
create table ChiTietPX
(
	MaPX char (10) not null,
	MaSP char (10) not null,
	SoLuong int,
	DonGiaNhap money,
	ThanhTien money,
	constraint PK_CTPX primary key (MaPX, MaSP),
	constraint FK_PhieuXuat foreign key (MaPX) references PhieuXuat(MaPX),
	constraint FK_SanPham_CTPX foreign key (MaSP) references SanPham(MaSP)
)

--Bảng tài khoản
CREATE TABLE TaiKhoan
(
    TenDangNhap VARCHAR(50) PRIMARY KEY NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    MaNV CHAR(10) NOT NULL UNIQUE, -- Ràng buộc UNIQUE để 1 nhân viên chỉ có 1 tài khoản
    TrangThai BIT DEFAULT 1, -- 1: Đang hoạt động, 0: Bị khóa
    CONSTRAINT FK_TaiKhoan_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);
--- Bảng LoaiSanPham
--- Ràng buộc UNIQUE: Mã loại sản phẩm là duy nhất
alter table LoaiSanPham
add constraint UQ_LoaiSanPham_MaLoai unique (MaLoai);

--- Bảng SanPham
--- Ràng buộc UNIQUE: Mã sản phẩm là duy nhất
alter table SanPham
add constraint UQ_SanPham_MaSP unique (MaSP);

--- Ràng buộc DEFAULT: Số lượng tồn mặc định = 0
alter table SanPham
add constraint DF_SanPham_SLTon default 0 for SLTon;

--- Ràng buộc CHECK: Số lượng tồn không âm
alter table SanPham
add constraint CK_SanPham_SLTon check (SLTon >= 0);

--- Đơn giá phải lớn hơn 0
alter table SanPham
add constraint CK_SanPham_DonGia check (DonGia > 0);

--- Bảng NhanVien
--- Ràng buộc UNIQUE: Mã nhân viên là duy nhất
alter table NhanVien
add constraint UQ_NhanVien_MaNV unique (MaNV)

--- Ràng buộc DEFAULT: Chức vụ mặc định = Nhân viên


--- Bảng NhaCungCap
--- Ràng buộc UNIQUE: Mã nhà cung cấp là duy nhất
alter table NhaCungCap
add constraint UQ_NhaCungCap_MaNCC unique (MaNCC);

--- Bảng DaiLy
--- Ràng buộc UNIQUE: Mã đại lý là duy nhất
alter table DaiLy
add constraint UQ_DaiLy_MaDL unique (MaDL);

--- Bảng PhieuNhap
--- Ràng buộc UNIQUE: Mã phiếu nhập là duy nhất
alter table PhieuNhap
add constraint UQ_PhieuNhap_MaPN unique (MaPN);

--- Ràng buộc DEFAULT: Ngày nhập mặc định là ngày hiện tại
alter table PhieuNhap
add constraint DF_PhieuNhap_NgayNhap default getdate() for NgayNhap;

--- Ràng buộc CHECK: Tổng tiền >= 0
alter table PhieuNhap
add constraint CK_PhieuNhap_TongTien check (TongTien >= 0);

--- Bảng PhieuXuat
--- Ràng buộc UNIQUE: Mã phiếu xuất là duy nhất
alter table PhieuXuat
add constraint UQ_PhieuXuat_MaPX unique (MaPX);

--- Ràng buộc DEFAULT: Ngày xuất mặc định là ngày hiện tại
alter table PhieuXuat
add constraint DF_PhieuXuat_NgayXuat default getdate() for NgayXuat;

--- Ràng buộc CHECK: Tổng tiền >= 0
alter table PhieuXuat
add constraint CK_PhieuXuat_TongTien check (TongTien >= 0);

--- Bảng ChiTietPN
--- Ràng buộc CHECK: Số lượng nhập > 0
alter table ChiTietPN
add constraint CK_CTPN_SoLuong check (SoLuong > 0);

--- Ràng buộc CHECK: Đơn giá nhập > 0
alter table ChiTietPN
add constraint CK_CTPN_DonGiaNhap check (DonGiaNhap > 0);

--- Ràng buộc CHECK: Thành tiền >= 0
alter table ChiTietPN
add constraint CK_CTPN_ThanhTien check (ThanhTien >= 0);

--- Bảng ChiTietPX
--- Ràng buộc CHECK: Số lượng xuất > 0
alter table ChiTietPX
add constraint CK_CTPX_SoLuong check (SoLuong > 0);

--- Ràng buộc CHECK: Đơn giá xuất > 0
alter table ChiTietPX
add constraint CK_CTPX_DonGiaNhap check (DonGiaNhap > 0);

--- Ràng buộc CHECK: Thành tiền >= 0
alter table ChiTietPX
add constraint CK_CTPX_ThanhTien check (ThanhTien >= 0);




insert into LoaiSanPham values
('L01', N'Đồ uống'),
('L02', N'Bánh kẹo'),
('L03', N'Gia vị'),
('L04', N'Đồ hộp'),
('L05', N'Sữa'),
('L06', N'Mỹ phẩm'),
('L07', N'Văn phòng phẩm'),
('L08', N'Đồ gia dụng');

insert into SanPham values
('SP01', N'Nước suối Lavie', N'Chai', 100, 'L01', 5000),
('SP02', N'Coca Cola', N'Lon', 80, 'L01', 10000),
('SP03', N'Bánh Oreo', N'Gói', 60, 'L02', 15000),
('SP04', N'Muối i-ốt', N'Gói', 40, 'L03', 8000),
('SP05', N'Cá hộp 3 cô gái', N'Hộp', 30, 'L04', 25000),
('SP06', N'Sữa Vinamilk', N'Hộp', 90, 'L05', 12000),
('SP07', N'Bút bi Thiên Long', N'Cây', 200, 'L07', 5000),
('SP08', N'Nước rửa chén', N'Chai', 50, 'L08', 30000);

INSERT INTO NhomQuyen VALUES 
('Q01', N'Quản Lý'), 
('Q02', N'Nhân Viên');


insert into NhanVien values
('NV01', N'Nguyễn Văn A', '1999-05-10', 912345678, 'Q01'),
('NV02', N'Trần Thị B', '2000-08-15', 934567890, 'Q02'),
('NV03', N'Lê Văn C', '1998-02-20', 965432187, 'Q02'),
('NV04', N'Phạm Thị D', '2001-11-30', 978654321, 'Q02'),
('NV05', N'Hoàng Văn E', '1997-06-25', 987654321, 'Q02'),
('NV06', N'Đặng Thị F', '1999-09-18', 901234567, 'Q02'),
('NV07', N'Bùi Văn G', '2000-01-05', 923456789, 'Q02'),
('NV08', N'Võ Thị H', '2002-04-12', 956789012, 'Q02');

insert into NhaCungCap values
('NCC01', N'Công ty An Phát', 912345111, N'Hà Nội', 'anphat@gmail.com'),
('NCC02', N'Công ty Minh Long', 912345222, N'TP.HCM', 'minhlong@gmail.com'),
('NCC03', N'Công ty Hòa Bình', 912345333, N'Đà Nẵng', 'hoabinh@gmail.com'),
('NCC04', N'Công ty Thành Công', 912345444, N'Hải Phòng', 'thanhcong@gmail.com'),
('NCC05', N'Công ty Việt Nhật', 912345555, N'Cần Thơ', 'vietnhat@gmail.com'),
('NCC06', N'Công ty Đại Phát', 912345666, N'Bình Dương', 'daiphat@gmail.com'),
('NCC07', N'Công ty Tân Tiến', 912345777, N'Đồng Nai', 'tantien@gmail.com'),
('NCC08', N'Công ty Phú Quý', 912345888, N'Long An', 'phuquy@gmail.com');

insert into DaiLy values
('DL01', N'Đại lý Minh Châu', N'Quận 1', 934111111),
('DL02', N'Đại lý Hồng Phát', N'Quận 3', 934222222),
('DL03', N'Đại lý Tân Lợi', N'Quận 5', 934333333),
('DL04', N'Đại lý Gia Bảo', N'Quận 7', 934444444),
('DL05', N'Đại lý Hoàng Long', N'Tân Bình', 934555555),
('DL06', N'Đại lý Phúc An', N'Gò Vấp', 934666666),
('DL07', N'Đại lý Thịnh Phát', N'Bình Thạnh', 934777777),
('DL08', N'Đại lý Đại Phát', N'Thủ Đức', 934888888);

---Sử dụng giá trị mặc định cho cột NgayNhap
insert into PhieuNhap (MaPN, TongTien, MaNCC, MaNV)
values
('PN01', 500000, 'NCC01', 'NV01'),
('PN02', 420000, 'NCC02', 'NV02'),
('PN03', 380000, 'NCC03', 'NV03'),
('PN04', 600000, 'NCC04', 'NV04'),
('PN05', 250000, 'NCC05', 'NV05'),
('PN06', 700000, 'NCC06', 'NV06'),
('PN07', 450000, 'NCC07', 'NV07'),
('PN08', 520000, 'NCC08', 'NV08');

---Sử dụng giá trị mặc định cho cột NgayXuat
insert into PhieuXuat (MaPX, TongTien, MaDL, MaNV)
values
('PX01', 300000, 'DL01', 'NV01'),
('PX02', 280000, 'DL02', 'NV02'),
('PX03', 350000, 'DL03', 'NV03'),
('PX04', 400000, 'DL04', 'NV04'),
('PX05', 200000, 'DL05', 'NV05'),
('PX06', 450000, 'DL06', 'NV06'),
('PX07', 320000, 'DL07', 'NV07'),
('PX08', 380000, 'DL08', 'NV08');

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


INSERT INTO TaiKhoan (TenDangNhap, MatKhau, MaNV) VALUES 
('admin', '1', 'NV01'),      
('nhanvien1', '1', 'NV02'), 
('nhanvien2', '1', 'NV03');

select*from TaiKhoan
--- Xem toàn bộ sản phẩm
SELECT * 
FROM SanPham;

---Tìm các sản phẩm sắp hết hàng (số lượng < 10)
SELECT *
FROM SanPham
WHERE SLTon < 10;

---Tính tổng số lượng sản phẩm trong kho
SELECT SUM(SLTon) AS TongSoLuong
FROM SanPham;

---Tính tổng giá trị kho
SELECT 
    SUM(SLTon * DonGia) AS TongGiaTriKho
FROM SanPham;

---Danh sách phiếu nhập + nhân viên lập
SELECT 
    pn.MaPN,
    pn.NgayNhap,
    nv.HoTen,
    pn.TongTien
FROM PhieuNhap pn
JOIN NhanVien nv ON pn.MaNV = nv.MaNV;

---Thống kê số sản phẩm theo từng loại
SELECT 
    lsp.TenLoai,
    COUNT(sp.MaSP) AS SoLuongSP
FROM LoaiSanPham lsp
LEFT JOIN SanPham sp ON lsp.MaLoai = sp.MaLoai
GROUP BY lsp.TenLoai;

---Thông tin chi tiết sản phẩm
CREATE VIEW vw_ThongTinSanPham 
AS
SELECT 
    sp.MaSP, 
    sp.TenSP, 
    lsp.TenLoai, 
    sp.DVT, 
    sp.SLTon, 
    sp.DonGia,
    (sp.SLTon * sp.DonGia) AS TongGiaTriTien
FROM SanPham sp
JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai;
---Báo cáo Thống kê	Xuất-Nhập-Tồn Kho
CREATE VIEW vw_BaoCaoXuatNhapTon 
AS
SELECT 
    sp.MaSP,
    sp.TenSP,
    lsp.TenLoai,
    ISNULL(Nhap.TongSLNhap, 0) AS TongSoLuongNhap,
    ISNULL(Xuat.TongSLXuat, 0) AS TongSoLuongXuat,
    sp.SLTon AS SoLuongTonKhoThucTe
FROM SanPham sp
JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai
LEFT JOIN (
    -- Subquery tính tổng số lượng nhập của từng sản phẩm
    SELECT MaSP, SUM(SoLuong) AS TongSLNhap
    FROM ChiTietPN
    GROUP BY MaSP
) AS Nhap ON sp.MaSP = Nhap.MaSP
LEFT JOIN (
    -- Subquery tính tổng số lượng xuất của từng sản phẩm
    SELECT MaSP, SUM(SoLuong) AS TongSLXuat
    FROM ChiTietPX
    GROUP BY MaSP
) AS Xuat ON sp.MaSP = Xuat.MaSP;

-- View xuất chi tiết phiếu nhập
CREATE VIEW vw_ChiTietPhieuNhap
AS
SELECT
    pn.MaPN,
    pn.NgayNhap,
    nv.HoTen        AS TenNhanVien,
    ncc.TenNCC      AS TenNhaCungCap,
    sp.TenSP,
    ctpn.SoLuong,
    ctpn.DonGiaNhap,
    ctpn.ThanhTien
FROM PhieuNhap pn
JOIN NhanVien    nv   ON pn.MaNV   = nv.MaNV
JOIN NhaCungCap  ncc  ON pn.MaNCC  = ncc.MaNCC
JOIN ChiTietPN   ctpn ON pn.MaPN   = ctpn.MaPN
JOIN SanPham     sp   ON ctpn.MaSP = sp.MaSP;

-- View xuất chi tiết phiếu xuất
CREATE VIEW vw_ChiTietPhieuXuat
AS
SELECT
    px.MaPX,
    px.NgayXuat,
    nv.HoTen    AS TenNhanVien,
    dl.TenDL    AS TenDaiLy,
    dl.DiaChi   AS DiaChiDaiLy,
    sp.TenSP,
    ctpx.SoLuong,
    ctpx.DonGiaNhap AS DonGiaXuat,
    ctpx.ThanhTien
FROM PhieuXuat px
JOIN NhanVien   nv   ON px.MaNV   = nv.MaNV
JOIN DaiLy      dl   ON px.MaDL   = dl.MaDL
JOIN ChiTietPX  ctpx ON px.MaPX   = ctpx.MaPX
JOIN SanPham    sp   ON ctpx.MaSP = sp.MaSP;

--Tính tổng tiền thưởng nhân viên
CREATE PROCEDURE sp_TinhTienThuong
    @LuongCoBan MONEY,       
    @HeSo FLOAT,            
    @TienThuong MONEY OUTPUT 
AS
BEGIN  
    SET @TienThuong = @LuongCoBan * @HeSo;
END
GO
DECLARE @luong MONEY;
DECLARE @heso FLOAT;
DECLARE @tong_tien_thuong MONEY;
SET @luong = 15000000;
SET @heso = 1.5;
EXEC sp_TinhTienThuong 
    @LuongCoBan = @luong,
    @HeSo = @heso,
    @TienThuong = @tong_tien_thuong OUTPUT;
SELECT @tong_tien_thuong AS N'Tổng Tiền Thưởng Nhân Viên';

--Lấy tổng tiền phiếu nhập
CREATE PROCEDURE sp_LayTongTienPhieuNhap
    @manv     VARCHAR(10),
    @mancc    VARCHAR(10),
    @mapn     VARCHAR(10),
    @tongtien MONEY OUTPUT
AS
BEGIN
    SELECT @tongtien = TongTien
    FROM PhieuNhap
    WHERE MaPN  = @mapn
      AND MaNV  = @manv
      AND MaNCC = @mancc;
END
GO

declare @_tongtien money
declare @_manv varchar(10) ='NV01'
declare @_mancc varchar(10) = 'NCC01'
declare @_mapn varchar(10) ='PN01'
exec sp_laytongtienphieunhap
    @manv= @_manv,
    @mancc = @_mancc,
    @mapn = @_mapn,
    @tongtien = @_tongtien output

select @_tongtien as N'Tổng Tiền Phiếu Nhập'

--Tính tổng số lượng sản phẩm theo phiếu xuất
CREATE PROCEDURE sp_TongSoLuongSanPhamTheoPhieuXuat
    @mapx VARCHAR(10)
AS
BEGIN
    SELECT 
        sp.MaSP,
        sp.TenSP,
        SUM(ctpx.SoLuong) AS TongSoLuong
    FROM ChiTietPX ctpx
    JOIN SanPham sp ON ctpx.MaSP = sp.MaSP
    WHERE ctpx.MaPX = @mapx
    GROUP BY sp.MaSP, sp.TenSP;
END
GO

-- Gọi thử với phiếu xuất PX01
EXEC sp_TongSoLuongSanPhamTheoPhieuXuat @mapx = 'PX01'


-- Kiểm tra số lượng tồn
CREATE PROCEDURE sp_TongTonKhoTheoLoai
    @MaLoai CHAR(10),              
    @TongSoLuong INT OUTPUT      
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM LoaiSanPham WHERE MaLoai = @MaLoai)
    BEGIN
        SET @TongSoLuong = 0;
        PRINT N'Mã loại không tồn tại!';
        RETURN;
    END

    SELECT @TongSoLuong = SUM(SLTon)
    FROM SanPham
    WHERE MaLoai = @MaLoai;

    IF @TongSoLuong IS NULL
        SET @TongSoLuong = 0;
END

DECLARE @KetQuaTong INT;
DECLARE @MaLoaiCanCheck CHAR(10) = 'L01';

EXEC sp_TongTonKhoTheoLoai
    @MaLoai = @MaLoaiCanCheck,
    @TongSoLuong = @KetQuaTong OUTPUT;

SELECT @MaLoaiCanCheck AS MaLoai, 
       @KetQuaTong AS TongSoLuongTonTrongKho;



-- =============================================
-- CÂU 1 - DẠNG 1: fn_PhieuNhapTheoKhoangThoiGian
-- =============================================
CREATE FUNCTION fn_PhieuNhapTheoKhoangThoiGian
(
    @NgayBatDau DATETIME,
    @NgayKetThuc DATETIME
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        pn.MaPN,
        pn.NgayNhap,
        nv.HoTen        AS TenNhanVien,
        ncc.TenNCC      AS TenNhaCungCap,
        sp.TenSP,
        ctpn.SoLuong,
        ctpn.DonGiaNhap,
        ctpn.ThanhTien
    FROM PhieuNhap pn
    JOIN NhanVien   nv   ON pn.MaNV  = nv.MaNV
    JOIN NhaCungCap ncc  ON pn.MaNCC = ncc.MaNCC
    JOIN ChiTietPN  ctpn ON pn.MaPN  = ctpn.MaPN
    JOIN SanPham    sp   ON ctpn.MaSP = sp.MaSP
    WHERE pn.NgayNhap BETWEEN @NgayBatDau AND @NgayKetThuc
);


-- Gọi thử
SELECT *
FROM fn_PhieuNhapTheoKhoangThoiGian('2024-01-01', '2026-12-31');
GO


-- =============================================
-- CÂU 2 - DẠNG 1: fn_ThongKeSanPhamTheoLoai
-- =============================================
CREATE FUNCTION fn_ThongKeSanPhamTheoLoai
(
    @MaLoai CHAR(10)
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        sp.MaSP,
        sp.TenSP,
        sp.DVT,
        sp.DonGia,
        ISNULL(Nhap.TongSLNhap, 0)  AS TongSLNhap,
        ISNULL(Xuat.TongSLXuat, 0)  AS TongSLXuat,
        sp.SLTon                    AS SLTonThucTe
    FROM SanPham sp
    LEFT JOIN (
        SELECT MaSP, SUM(SoLuong) AS TongSLNhap
        FROM ChiTietPN
        GROUP BY MaSP
    ) AS Nhap ON sp.MaSP = Nhap.MaSP
    LEFT JOIN (
        SELECT MaSP, SUM(SoLuong) AS TongSLXuat
        FROM ChiTietPX
        GROUP BY MaSP
    ) AS Xuat ON sp.MaSP = Xuat.MaSP
    WHERE sp.MaLoai = @MaLoai
);

-- Gọi thử với loại L01 (Đồ uống)
SELECT *
FROM fn_ThongKeSanPhamTheoLoai('L01');
GO


-- =============================================
-- CÂU 3 - DẠNG 2: fn_LichSuGiaoDichNhanVien
-- =============================================
CREATE FUNCTION fn_LichSuGiaoDichNhanVien
(
    @MaNV CHAR(10)
)
RETURNS @KetQua TABLE
(
    LoaiPhieu   NVARCHAR(10),
    MaPhieu     CHAR(10),
    NgayLap     DATETIME,
    DoiTac      NVARCHAR(100),
    TongTien    MONEY
)
AS
BEGIN
    -- Nếu nhân viên không tồn tại thì trả về bảng rỗng luôn
    IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = @MaNV)
        RETURN;

    -- Insert phiếu nhập
    INSERT INTO @KetQua (LoaiPhieu, MaPhieu, NgayLap, DoiTac, TongTien)
    SELECT
        N'Nhập'         AS LoaiPhieu,
        pn.MaPN         AS MaPhieu,
        pn.NgayNhap     AS NgayLap,
        ncc.TenNCC      AS DoiTac,
        pn.TongTien
    FROM PhieuNhap pn
    JOIN NhaCungCap ncc ON pn.MaNCC = ncc.MaNCC
    WHERE pn.MaNV = @MaNV;

    -- Insert phiếu xuất
    INSERT INTO @KetQua (LoaiPhieu, MaPhieu, NgayLap, DoiTac, TongTien)
    SELECT
        N'Xuất'         AS LoaiPhieu,
        px.MaPX         AS MaPhieu,
        px.NgayXuat     AS NgayLap,
        dl.TenDL        AS DoiTac,
        px.TongTien
    FROM PhieuXuat px
    JOIN DaiLy dl ON px.MaDL = dl.MaDL
    WHERE px.MaNV = @MaNV;

    RETURN;
END;
GO

-- Gọi thử với nhân viên NV01
SELECT *
FROM fn_LichSuGiaoDichNhanVien('NV01')
ORDER BY NgayLap, LoaiPhieu;
GO

-- Gọi thử với mã không tồn tại → trả về bảng rỗng
SELECT *
FROM fn_LichSuGiaoDichNhanVien('NV99');
GO