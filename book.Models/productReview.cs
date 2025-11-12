#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace book.Models
{
    public class productReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Range(1, 5)]
        [Required]
        public int Rating { get; set; }  

        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } 

        public int OrderId { get; set; }    
        public bool IsApproved { get; set; } = false;

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
