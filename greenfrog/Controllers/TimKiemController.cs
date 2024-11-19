using System;
using greenfrog.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;

namespace greenfrog.Controllers
{
    public class TimKiemController : Controller
    {
        
        doanwebEntities data = new doanwebEntities();

        [HttpGet]
        public ActionResult TimKiem(int? page, string search)
        {
            if (search == null || search == "" || search == " ")
            {
                TempData["thongbaodetrrong"] = "Không được để trống khi tìm kiếm";
            }

            if (search.Length > 30)
            {
                TempData["thongbaododai"] = "Độ dài vượt quá 30 kí tự";
            }

            if (page == null) page = 1;
            search = RemoveDiacritics(search).ToLower();
            //tìm kiếm không cần viết HOA
            var lstSP = data.SanPhams.AsEnumerable().Where(n => RemoveDiacritics(n.tensp).ToLower().Contains(search)).OrderBy(n => n.tensp);
            int pageSize = 5;
            int pageNum = page ?? 1;

            ViewBag.Search = search;

            return View(lstSP.ToPagedList(pageNum, pageSize));
        }
        //Tìm kiếm không cần giấu
        public string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        //Demo
        public ActionResult KQ(int? page, string search, string searchby)
        {
            if (page == null) page = 1;

            int pageSize = 5;
            int pageNum = page ?? 1;

            if (searchby == "giakm")
            {
                var tblProduct = data.SanPhams.Where(n => n.giakhuyenmai.ToString().Contains(search));
                ViewBag.Search = search;
                ViewBag.Searchby = searchby;
                return View(tblProduct.ToPagedList(pageNum, pageSize));
            }
            else if (searchby == "danhmuc")
            {
                var tblProduct = data.SanPhams.Where(n => n.idDanhmuc.ToString().Contains(search));
                ViewBag.Search = search;
                ViewBag.Searchby = searchby;
                return View(tblProduct.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var tblProduct = data.SanPhams.Where(n => n.tensp.Contains(search));
                ViewBag.Search = search;
                ViewBag.Searchby = searchby;
                return View(tblProduct.ToPagedList(pageNum, pageSize));

            }

        }

    }
}