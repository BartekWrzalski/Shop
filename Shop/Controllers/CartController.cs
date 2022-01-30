using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly MyDbContext _context;

        public CartController(MyDbContext context)
        {
            _context = context;
        }

        // GET: CartController1
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            List<Article> articles = _context.Article.Include(a => a.Category).ToList();
            List<Article> articlesInCart = new List<Article>();
            int suma = 0;

            foreach (var item in articles)
            {
                var cookie = Request.Cookies[$"art{item.Id}"];
                if (cookie != null)
                {
                    articlesInCart.Add(item);
                    ViewData[$"countArt{item.Id}"] = cookie;
                    suma += item.Price * Convert.ToInt32(cookie);
                }
            }
            ViewData["suma"] = suma;

            return View(articlesInCart);
        }

        // GET: CartController1/Delete/5
        public IActionResult Delete(int? Id)
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (Id == null)
            {
                return NotFound();
            }

            var article = _context.Article.FirstOrDefault(a => a.Id == Id);
            if (article == null)
            {
                return NotFound();
            }
            if (Request.Cookies[$"art{Id}"] == null)
            {
                return NotFound();
            }

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Append($"art{Id}", "", option);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult MinusOne(int? Id)
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (Id == null)
            {
                return NotFound();
            }

            var article = _context.Article.FirstOrDefault(a => a.Id == Id);
            if (article == null)
            {
                return NotFound();
            }

            if (Request.Cookies[$"art{Id}"] == null)
            {
                return NotFound();
            }

            CookieOptions option = new CookieOptions();
            int count = Convert.ToInt32(Request.Cookies[$"art{Id}"]);
            if (count - 1 == 0)
            {
                option.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Append($"art{Id}", "", option);

                return RedirectToAction(nameof(Index));
            }

            option.Expires = DateTime.Now.AddDays(7);
            Response.Cookies.Append($"art{Id}", Convert.ToString(count - 1), option);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult PlusOne(int? Id)
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (Id == null)
            {
                return NotFound();
            }

            var article = _context.Article.FirstOrDefault(a => a.Id == Id);
            if (article == null)
            {
                return NotFound();
            }

            if (Request.Cookies[$"art{Id}"] == null)
            {
                return NotFound();
            }

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(7);
            int count = Convert.ToInt32(Request.Cookies[$"art{Id}"]);
            Response.Cookies.Append($"art{Id}", Convert.ToString(count + 1), option);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Client")]
        public IActionResult Order()
        {
            List<Article> articles = _context.Article.Include(a => a.Category).ToList();
            List<Article> articlesInCart = new List<Article>();
            int suma = 0;

            foreach (var item in articles)
            {
                var cookie = Request.Cookies[$"art{item.Id}"];
                if (cookie != null)
                {
                    articlesInCart.Add(item);
                    ViewData[$"countArt{item.Id}"] = cookie;
                    suma += item.Price * Convert.ToInt32(cookie);
                }
            }
            ViewData["suma"] = suma;

            return View(articlesInCart);
        }

        [Authorize(Roles = "Client")]
        public IActionResult Confirm(string imie, string nazwisko, int telefon, string adres, string kod, string miasto, string platnosc)
        {
            ViewBag.imie = imie;
            ViewBag.nazwisko = nazwisko;
            ViewBag.telefon = telefon;
            ViewBag.adres = adres;
            ViewBag.kod = kod;
            ViewBag.miasto = miasto;
            switch (platnosc)
            {
                case "BLIK":
                    ViewBag.platnosc = "BLIK";
                    break;
                case "ODBIOR":
                    ViewBag.platnosc = "Przy odbiorze";
                    break;
                default:
                    ViewBag.platnosc = "Przelew";
                    break;
            }

            List<Article> articles = _context.Article.Include(a => a.Category).ToList();
            foreach (var item in articles)
            {
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Append($"art{item.Id}", "", option);
            }
            return View();
        }
    }
}
