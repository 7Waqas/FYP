using book.DataAccess.Data;
using book.DataAccess.Repository.IRepository;
using book.Models;
using book.Models.ViewModel;
using book.Utility;
using bookNest.Areas.Seller.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace bookNest.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = SD.Role_Seller)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webhostEnvironment, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _webhostEnvironment = webhostEnvironment;
            _db = dbContext;
        }
        public IActionResult Index()
        {        
            return View();
        }

        public IActionResult sellerProduct()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get all products for the logged-in seller
            var products = (from pro in _db.Products
                            join cat in _db.Categories on pro.CategoryId equals cat.Id where pro.userId == userId
                            select new ProductViewModel
                            {
                                author = pro.Author,
                                categoryname = cat.Name,
                                Id = pro.Id,
                                isbn = pro.ISBN,
                                listPrice = pro.ListPrice,
                                title = pro.Title
                            }).ToList();

            return new JsonResult(new { data = products });
        }

        public IActionResult Create(int? id)
        {


            ProductVM productVM = new()
            {

                Categorylist = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()

            };
            if (id == null || id == 0)
            {
                //create product
                return View(productVM);
            }
            else
            {
                //update product
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM, List<IFormFile> files)
        {

            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                productVM.Product.userId = userId;
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }


                _unitOfWork.Save();

                string wwwRootPath = _webhostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {


                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = Path.Combine("images", "products", "product-" + productVM.Product.Id);
                        string finalPath = Path.Combine(wwwRootPath, productPath);


                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);


                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = "/" + Path.Combine(productPath, fileName).Replace("\\", "/"),
                            ProductId = productVM.Product.Id
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);
                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();


                }


                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.Categorylist = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });


            }
            return View(productVM);
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }


            string productPath = Path.Combine("images", "products", "product-" + id);
            string finalPath = Path.Combine(_webhostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filepaths = Directory.GetFiles(finalPath);
                foreach (string filepath in filepaths)
                {
                    System.IO.File.Delete(filepath);

                }

                Directory.Delete(finalPath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

    }
}
