#nullable disable
namespace bookNest.Areas.Seller.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string title { get; set; }   
        public string isbn { get; set; }    
        public double listPrice { get; set; }   
        public string author { get; set; }  
        public string categoryname { get; set; }    
    }
}
