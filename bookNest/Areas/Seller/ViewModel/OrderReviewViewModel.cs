namespace bookNest.Areas.Seller.ViewModel
{
    public class OrderReviewViewModel
    {
        public int OrderId { get; set; }
        public List<ProductReviewViewModel> ProductReviews { get; set; }
    }
}
