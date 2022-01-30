using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [EnableCors]
    [Route("api/article")]
    [ApiController]
    public class ArticleApiController : ControllerBase
    {
        private MyDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArticleApiController(MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IEnumerable<Article> Get() => _context.Article.ToList();

        [HttpGet("{id}")]
        public Article Get(int id) => _context.Article.FirstOrDefault(c => c.Id == id);

        [HttpPost]
        public Article Post([FromBody] Article art)
        {
            _context.Add(new Article
            {
                Nazwa = art.Nazwa,
                Price = art.Price,
                CategoryId = art.CategoryId,
                Photo = "noimage.jpg"
            });
            _context.SaveChanges();
            return art;
        }

        [HttpPut]
        public Article Put([FromBody] Article art)
        {
            art.Photo = "noimage.jpg";
            _context.Update(art);
            _context.SaveChanges();
            return art;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var art = _context.Article.FirstOrDefault(c => c.Id == id);
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "upload", art.Photo);
            System.IO.File.Delete(filePath);

            _context.Article.Remove(art);
            _context.SaveChanges();
        }

        [HttpGet("{id}/next")]
        public Article Next(int id)
        {
            return _context.Article.Where(c => c.Id > id).OrderBy(c => c.Id).FirstOrDefault();
        }

        [HttpGet("{id}/nextFromCategory/{cat}")]
        public Article NextFromArticle(int id, int cat)
        {
            return _context.Article.Where(a => a.CategoryId == cat).Where(c => c.Id > id).OrderBy(c => c.Id).FirstOrDefault();
        }
    }
}
