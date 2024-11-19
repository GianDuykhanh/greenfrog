create database doanweb
go
use doanweb
go

create table DanhMuc
(
idDanhmuc int identity primary key,
tendanhmuc nvarchar(30),
ParentID nvarchar(100) default NULL
)
go

create table ThuVienAnh
(
idthuvien int identity primary key,
img1 nvarchar(255),
img2 nvarchar(255),
img3 nvarchar(255)
)
go

create table SanPham
(
masp int identity(1,1) primary key,
idDanhmuc int references
DanhMuc(idDanhmuc),
idthuvien int references
ThuVienAnh(idthuvien),
tensp nvarchar(100) not null,
hinh nvarchar(255),
giaban decimal(18,0),
ngaycapnhat smalldatetime,
soluongton int,
mota nvarchar(MAX),
giamgia int,
giakhuyenmai decimal(18,0)
)
go

create table KhachHangRoles(
RoleID int,
RoleName varchar(30)
primary key(RoleID)
)
go

create table KhachHang(
makh int identity(1,1) primary key,
hoten nvarchar(50),
tendangnhap varchar(20),

matkhau nvarchar(255),
email varchar(50),
diachi nvarchar(100),
dienthoai varchar(15),
ngaysinh date,
RoleID int references KhachHangRoles(RoleID),
status int,
resetpasswordcode nvarchar(255)
)
go


create table DichVu(
	iddichvu int identity(1,1) primary key,
	hoten nvarchar(30),
	email nvarchar(30),
	sdt nvarchar(30),
	diachi nvarchar(30),
	trangthai nvarchar(30) NULL,
	tendichvu nvarchar(30),
	ngaydat datetime,
	makh int references
	KhachHang(makh)
)
go

create table DonHang(
madon int identity(1,1) primary key,
thanhtoan nvarchar(50),
giaohang nvarchar(255),
ngaydat date,
ngaygiao date,
makh int references KhachHang(makh)
)
go

create table ChiTietDonHang(
madon int references DonHang(madon),
masp int references SanPham(masp),
soluong int,
gia decimal(18,0),
tongsoluong int,
tonggia decimal(18,0),
status int,
primary key(madon,masp)
)
go

create table DanhGia(
	Id int identity(1,1) primary key,
	[Content] nvarchar(MAX),
	Rating float,
	Ngaycapnhap datetime,
	trangthai int default 0,
	id_sp int references SanPham(masp),
	id_kh int references KhachHang(makh)
)
go

create table LiveStream
(
idLiveStream int identity primary key,
noidunglive nvarchar(255),
hinh nvarchar(255),
link nvarchar(255)
)
go


INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Sản phẩm mới nhất', NULL);

INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Sản phẩm giảm giá', NULL);

INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Tô Gỗ', N'Sản phẩm mới nhất');
INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Túi Giấy', N'Sản phẩm mới nhất');
INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Ống Hút', N'Sản phẩm mới nhất');


INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Tô Gỗ 1', N'Sản phẩm giảm giá');
INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Túi Giấy 1', N'Sản phẩm giảm giá');
INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Ống Hút 1', N'Sản phẩm giảm giá');

INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Sản phẩm khác', NULL);

INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Ly Thủy Tinh', N'Sản phẩm khác');

INSERT INTO DanhMuc (tendanhmuc, ParentID)
VALUES (N'Ly Sứ', N'Sản phẩm khác');

go


INSERT INTO KhachHangRoles(RoleID, RoleName)
VALUES (1, 'Admin');
go
INSERT INTO KhachHangRoles(RoleID, RoleName)
VALUES (2, 'User');
go
INSERT INTO KhachHangRoles(RoleID, RoleName)
VALUES (3, 'Staff');
go



INSERT INTO [dbo].[KhachHang]
           ([hoten]
           ,[tendangnhap]
           ,[matkhau]
           ,[email]
           ,[diachi]
           ,[dienthoai]
           ,[ngaysinh]
           ,[RoleID]
		   ,[status])
     VALUES
           ('Vu Ngoc Lam'
           ,'lam'
           ,'202cb962ac59075b964b07152d234b70'
           ,'thanhphohochiminh5@gmail.com'
           ,'HCM'
           ,123456789
           ,'15/10/2004'
           ,2,
		   1)
GO
INSERT INTO [dbo].[KhachHang]
           ([hoten]
           ,[tendangnhap]
           ,[matkhau]
           ,[email]
           ,[diachi]
           ,[dienthoai]
           ,[ngaysinh]
           ,[RoleID],
		   [status])
     VALUES
           ('Admin'
           ,'admin'
           ,'202cb962ac59075b964b07152d234b70'
           ,null
           ,''
           ,null
           ,null
           ,1,
		   1)
GO
INSERT INTO [dbo].[KhachHang]
           ([hoten]
           ,[tendangnhap]
           ,[matkhau]
           ,[email]
           ,[diachi]
           ,[dienthoai]
           ,[ngaysinh]
           ,[RoleID],
		   [status])
     VALUES
           ('Staff'
           ,'Staff'
           ,'202cb962ac59075b964b07152d234b70'
           ,null
           ,null
           ,null
           ,null
           ,3,
		   1)
GO


create TRIGGER trg_HuyDatHang ON ChiTietDonHang FOR DELETE AS 
BEGIN
	UPDATE SanPham
	SET SoLuongTon = SoLuongTon + (SELECT soluong FROM deleted WHERE masp = SanPham.masp)
	FROM SanPham 
	JOIN deleted ON SanPham.masp = deleted.masp
	
END

