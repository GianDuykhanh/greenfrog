//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace greenfrog.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class KhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhachHang()
        {
            this.DanhGias = new HashSet<DanhGia>();
            this.DichVus = new HashSet<DichVu>();
            this.DonHangs = new HashSet<DonHang>();
        }
    
        public int makh { get; set; }
        public string hoten { get; set; }
        public string tendangnhap { get; set; }
        public string matkhau { get; set; }
        public string email { get; set; }
        public string diachi { get; set; }
        public string dienthoai { get; set; }
        public Nullable<System.DateTime> ngaysinh { get; set; }
        public Nullable<int> RoleID { get; set; }
        public Nullable<int> status { get; set; }
        public string resetpasswordcode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhGia> DanhGias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DichVu> DichVus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHang> DonHangs { get; set; }
        public virtual KhachHangRole KhachHangRole { get; set; }
    }
}