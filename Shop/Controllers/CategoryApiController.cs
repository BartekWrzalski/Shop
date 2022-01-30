using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Authorization;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [EnableCors]
    [Route("api/category")]
    [ApiController]
    public class CategoryApiController : ControllerBase
    {
        private MyDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryApiController(MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IEnumerable<Category> Get() => _context.Category.ToList();

        [HttpGet("{id}")]
        public Category Get(int id) => _context.Category.FirstOrDefault(c => c.Id == id);

        [HttpPost]
        public Category Post([FromBody] Category cat)
        {
            _context.Add(new Category
            {
                Nazwa = cat.Nazwa,
            });
            _context.SaveChanges();
            return cat;
        }

        [HttpPut]
        public Category Put([FromBody] Category cat)
        {
            _context.Update(cat);
            _context.SaveChanges();
            return cat;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            foreach (var article in _context.Article.Where(a => a.CategoryId == id))
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "upload", article.Photo);
                Response.Cookies.Append($"art{article.Id}", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
                System.IO.File.Delete(filePath);
            }
            var cat = _context.Category.FirstOrDefault(c => c.Id == id);

            _context.Category.Remove(cat);
            _context.SaveChanges();
        }
    }
}
