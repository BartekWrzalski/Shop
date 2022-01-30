using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Shop.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "To short name")]
        [MaxLength(50, ErrorMessage = "To long name, do not exceed {1}")]
        public string Nazwa { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string Photo { get; set; }

        [NotMapped]
        [Display(Name = "Photo")]
        public IFormFile PhotoFile { get; set; }

    }
}