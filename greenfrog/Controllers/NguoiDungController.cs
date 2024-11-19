using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using greenfrog.Models;
using System.Security.Cryptography;
using System.Text;
using greenfrog.Common;
using System.Net.Mail;
using System.Net;

namespace greenfrog.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        doanwebEntities data = new doanwebEntities();

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KhachHang kh)
        {
            var hoten = collection["hoten"];
            var tendangnhap = collection["tendangnhap"];
            var matkhau1 = collection["matkhau"];
            var matkhau = Encryptor.MD5Hash(matkhau1);

            doanwebEntities data = new doanwebEntities();
            var dblist = data.KhachHangs.ToList();

            // Kiểm tra nhập liệu cơ bản
            if (string.IsNullOrEmpty(hoten) || string.IsNullOrEmpty(tendangnhap) || string.IsNullOrEmpty(matkhau1))
            {
                ViewData["Error"] = "Vui lòng điền đầy đủ thông tin bắt buộc.";
                return View();
            }

            // Kiểm tra độ dài mật khẩu
            if (matkhau1.Length < 6 || matkhau1.Length > 18)
            {
                ViewData["Length>6"] = "Mật khẩu phải dài từ 6 đến 18 ký tự";
            }
            // Kiểm tra độ dài họ tên
            else if (hoten.Length < 4 || hoten.Length > 50)
            {
                ViewData["hoten>3"] = "Họ tên phải dài từ 4 đến 50 ký tự";
            }
            // Kiểm tra độ dài tên đăng nhập
            else if (tendangnhap.Length < 4 || tendangnhap.Length > 30)
            {
                ViewData["tendangnhap>3"] = "Tên đăng nhập phải dài từ 4 đến 30 ký tự";
            }
            else
            {
                var MatKhauXacNhan1 = collection["MatKhauXacNhan"];
                var MatKhauXacNhan = Encryptor.MD5Hash(MatKhauXacNhan1);

                var email = collection["email"];
                var diachi = collection["diachi"];
                var dienthoai = collection["dienthoai"];
                var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["ngaysinh"]);

                // Kiểm tra nhập mật khẩu xác nhận
                if (String.IsNullOrEmpty(MatKhauXacNhan1))
                {
                    ViewData["NhapMKXN"] = "Phải nhập mật khẩu xác nhận";
                }
                else
                {
                    bool canRegister = true;

                    // Kiểm tra tồn tại tên đăng nhập hoặc email trong cơ sở dữ liệu
                    foreach (var item in dblist)
                    {
                        if (item.tendangnhap == tendangnhap)
                        {
                            ViewData["existtk"] = "Tên tài khoản đã tồn tại";
                            canRegister = false;
                            break;
                        }

                        if (item.email == email)
                        {
                            ViewData["existemail"] = "Email đã được sử dụng cho tài khoản khác";
                            canRegister = false;
                            break;
                        }
                    }

                    // Kiểm tra mật khẩu và mật khẩu xác nhận có giống nhau không
                    if (!matkhau.Equals(MatKhauXacNhan))
                    {
                        ViewData["MatKhauGiongNhau"] = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
                        canRegister = false;
                    }

                    // Kiểm tra ngày sinh hợp lệ
                    DateTime parsedNgaySinh;
                    if (!DateTime.TryParse(ngaysinh, out parsedNgaySinh))
                    {
                        ViewData["calcns"] = "Ngày sinh không hợp lệ";
                        canRegister = false;
                    }
                    else if ((DateTime.Now.Year - parsedNgaySinh.Year) < 18)
                    {
                        ViewData["calcns"] = "Khách hàng phải đảm bảo đủ 18 tuổi";
                        canRegister = false;
                    }

                    // Bổ sung: Kiểm tra định dạng email
                    if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"\A[^@\s]+@[^@\s]+\.[^@\s]+\z"))
                    {
                        ViewData["InvalidEmail"] = "Định dạng email không hợp lệ";
                        canRegister = false;
                    }

                    // Bổ sung: Kiểm tra số điện thoại
                    if (!System.Text.RegularExpressions.Regex.IsMatch(dienthoai, @"^\d{10,15}$"))
                    {
                        ViewData["InvalidPhone"] = "Số điện thoại phải là số và có độ dài từ 10 đến 15 số";
                        canRegister = false;
                    }

                    // Bổ sung: Kiểm tra địa chỉ
                    if (String.IsNullOrEmpty(diachi) || diachi.Length < 5 || diachi.Length > 100)
                    {
                        ViewData["InvalidAddress"] = "Địa chỉ phải dài từ 5 đến 100 ký tự";
                        canRegister = false;
                    }

                    if (canRegister)
                    {
                        // Lưu thông tin khách hàng mới vào cơ sở dữ liệu
                        kh.hoten = hoten;
                        kh.tendangnhap = tendangnhap;
                        kh.matkhau = matkhau;
                        kh.email = email;
                        kh.diachi = diachi;
                        kh.dienthoai = dienthoai;
                        kh.ngaysinh = parsedNgaySinh;
                        kh.RoleID = 2;
                        kh.status = 1;

                        data.KhachHangs.Add(kh);
                        data.SaveChanges();

                        return RedirectToAction("DangNhap");
                    }
                }
            }

            return View();
        }


        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();

        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendangnhap = collection["tendangnhap"];
            var matkhau1 = collection["matkhau"];

            if (string.IsNullOrEmpty(tendangnhap) || string.IsNullOrEmpty(matkhau1))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return RedirectToAction("DangNhap");
            }

            var matkhau = Encryptor.EncryptMD5(matkhau1);

            KhachHang kh = data.KhachHangs.FirstOrDefault(n => n.tendangnhap == tendangnhap && n.matkhau == matkhau);

            if (kh == null)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return RedirectToAction("DangNhap");
            }

            if (kh.status == 2)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn đã bị khóa do có hành vi bất thường.";
                return RedirectToAction("DangNhap");
            }

            Session["TaiKhoan"] = kh;
            Session["Username"] = tendangnhap;

            switch (kh.RoleID)
            {
                case 1:
                    Session["Admin"] = kh;
                    return RedirectToAction("Index", "Admin");
                case 3:
                    Session["Staff"] = kh;
                    return RedirectToAction("Index", "Admin");
                default:
                    return RedirectToAction("ListSanPham", "SanPham");
            }
        }

        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            Session.Clear();
            return RedirectToAction("ListSanPham", "SanPham");
        }


        [HttpPost]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/NguoiDung/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("kjitvaf4@gmail.com", "greenfrog");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "123456789Abc";

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Tai khoan da duoc tao thanh cong";
                body = "chua lam";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset password";
                body = "Bạn vừa gửi link xác thực tài khoản, Hãy click vào link bên dưới để lấy lại mật khẩu<br>" +
                    "<a href=" + link + ">Reset password</a>";
            }

            /*var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);*/
            /*smtp - mail.outlook.com*/

            MailMessage mc = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), emailID);
            mc.Subject = subject;
            mc.Body = body;
            mc.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            //smtp.Timeout = 1000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mc);

        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {

            if (IsValidEmail(EmailID))
            {
                string message = "";
                doanwebEntities data = new doanwebEntities();
                var account = data.KhachHangs.Where(a => a.email == EmailID).FirstOrDefault();

                if (account != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.email, resetCode, "ResetPassword");
                    account.resetpasswordcode = resetCode;

                    data.SaveChanges();

                    message = "Reset password link đã được gửi đến email của bạn";
                }


                ViewBag.Message = message;
                return View();
            }
            else
            {
                TempData["mgss"] = "Email không hợp lệ";
                return View();
            }

        }

        public ActionResult ResetPassword(string id)
        {
            doanwebEntities data = new doanwebEntities();
            var user = data.KhachHangs.Where(a => a.resetpasswordcode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                doanwebEntities data = new doanwebEntities();
                var user = data.KhachHangs.Where(a => a.resetpasswordcode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.matkhau = Encryptor.MD5Hash(model.NewPassword);
                    user.resetpasswordcode = "";

                    data.SaveChanges();
                    message = "Cập nhập mật khẩu thành công";
                }
            }
            else
            {
                message = "Cập nhập mật khẩu thất bại";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}
