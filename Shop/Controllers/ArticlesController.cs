using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticlesController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArticlesController(MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var myDbContext = _context.Article.Include(a => a.Category);
            return View(myDbContext.ToList());
        }

        // GET: Articles/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = _context.Article
                .Include(a => a.Category)
                .FirstOrDefault(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Nazwa");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Nazwa,Price,CategoryId,PhotoFile")] Article article)
        {
            if (ModelState.IsValid)
            {
                if (article.PhotoFile != null)
                {
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath + "/upload/");
                    string filename = Path.GetFileNameWithoutExtension(article.PhotoFile.FileName);
                    article.Photo = filename + DateTime.Now.ToString("ddmmyyyy") + Path.GetExtension(article.PhotoFile.FileName);
                    var photoStream = new FileStream(uploadFolder + article.Photo, FileMode.Create);
                    article.PhotoFile.CopyTo(photoStream);
                    photoStream.Close();
                }
                else
                {
                    article.Photo = "noimage.jpg";
                }

                _context.Add(article);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Nazwa", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = _context.Article.Find(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Nazwa", article.CategoryId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Nazwa,Price,CategoryId,Photo")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Nazwa", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = _context.Article
                .Include(a => a.Category)
                .FirstOrDefault(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var article = _context.Article.Find(id);
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "upload", article.Photo);
            System.IO.File.Delete(filePath);

            _context.Article.Remove(article);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
    }
}
