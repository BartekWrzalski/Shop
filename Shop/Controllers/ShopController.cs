using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    public class ShopController : Controller
    {
        private readonly MyDbContext _context;
        private readonly List<Category> categories;

        public ShopController(MyDbContext context)
        {
            _context = context;
            categories = _context.Category.ToList();
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var myDbContext = _context.Article.Include(a => a.Category);
            ViewData["CategoryId"] = categories;
            RememberCategory();
            return View(myDbContext.ToList().Take(4));
        }

        public IActionResult Select(int? Id)
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (Id == null)
            {
                return NotFound();
            }

            var category = _context.Category.FirstOrDefault(c => c.Id == Id);
            if (category == null)
            {
                return NotFound();
            }

            RememberCategory(Id);

            var myDbContext = _context.Article.Where(a => a.CategoryId == Id);
            ViewData["CategoryId"] = categories;
            return View("Index", myDbContext.ToList().Take(4));
        }

        public IActionResult AddToCart(int? Id)
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

            var count = Convert.ToInt32(HttpContext.Request.Cookies[$"art{Id}"]);

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(7);

            Response.Cookies.Append($"art{Id}", Convert.ToString(count + 1), option);

            if (Request.Cookies[nameof(Category)] == "-1")
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Select), new { Id = Request.Cookies[nameof(Category)] });
            }
        }

        public void RememberCategory(int? Id = null)
        {
            CookieOptions option = new CookieOptions();
            Response.Cookies.Append(nameof(Category), Convert.ToString(Id == null ? -1 : Id), option);
        }
    }
}
