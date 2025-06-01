
using book.DataAccess.Data;
using book.DataAccess.Repository;
using book.DataAccess.Repository.IRepository;
using book.Models;
using book.Models.ViewModel;
using book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bookNest.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webhostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webhostEnvironment = webhostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
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
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id,includeProperties:"ProductImages");
                return View(productVM);
            }

            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            
            if (ModelState.IsValid)
            {
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
                if(files != null)
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

        public IActionResult DeleteImage(int imageId) {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if(!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webhostEnvironment.WebRootPath,
                        imageToBeDeleted.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                }
               _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();
                TempData["success"] = "Image deleted successfully";
            }
            return RedirectToAction(nameof(Upsert), new { id = productId });

        }
       
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted= _unitOfWork.Product.Get(u => u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }


            string productPath = Path.Combine("images", "products", "product-" + id);
            string finalPath = Path.Combine(_webhostEnvironment.WebRootPath , productPath);

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
        #endregion
    }
}
