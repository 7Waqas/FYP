﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace book.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Display (Name="Last Price")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }
        [Display(Name = "Price for 1 to 50")]
        [Range(1, 1000)]
        public double Price { get; set; }
        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
        public string? userId { get; set; }
        

    }
}
