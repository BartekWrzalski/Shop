using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "To short name")]
        [MaxLength(20, ErrorMessage = "To long name, do not exceed {1}")]
        public string Nazwa { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}