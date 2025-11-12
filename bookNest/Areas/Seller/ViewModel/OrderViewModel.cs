#nullable disable
namespace bookNest.Areas.Seller.ViewModel
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public double TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}
