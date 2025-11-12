#nullable disable
using System.ComponentModel.DataAnnotations;

namespace bookNest.Areas.Seller.ViewModel
{
    public class ProductReviewViewModel
    {
        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Please give a star rating between 1 and 5.")]
        public int Rating { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }
    }
}
