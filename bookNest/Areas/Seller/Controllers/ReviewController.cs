using book.DataAccess.Data;
using book.Models;
using book.Models.ViewModel;
using book.Utility;
using bookNest.Areas.Seller.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bookNest.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = SD.Role_Seller + "," + SD.Role_Admin + "," + SD.Role_Customer)]

    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReviewController(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<IActionResult> ReviewList()
        {
            var reviews = await _db.productReviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .ToListAsync();

            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var review = await _db.productReviews.FindAsync(id);
            if (review == null)
                return NotFound();

            review.IsApproved = true;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Review approved successfully.";
            return RedirectToAction("ReviewList", "Review", new { area = "Seller" });

        }
        public async Task<IActionResult> Review(int orderId)
        {
            var orderDetails = await _db.OrderDetails
            .Where(od => od.OrderHeaderId == orderId)
            .Include(od => od.product)
            .ToListAsync();


            var model = new OrderReviewViewModel
            {
                OrderId = orderId,
                ProductReviews = orderDetails.Select(od => new ProductReviewViewModel
                {
                    ProductId = od.ProductId,
                    ProductName = od.product.Title,
                    OrderId = orderId
                }).ToList()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                foreach (var item in model.ProductReviews)
                {
                    bool alreadyReviewed = await _db.productReviews.AnyAsync(r =>
                        r.ProductId == item.ProductId && r.OrderId == model.OrderId &&
                        r.UserId == userId);

                    if (!alreadyReviewed)
                    {
                        _db.productReviews.Add(new productReview
                        {
                            ProductId = item.ProductId,
                            Rating = item.Rating,
                            Comment = item.Comment,
                            UserId = userId,
                            CreatedAt = DateTime.Now,
                            IsApproved = false,
                            OrderId = model.OrderId
                        });
                    }
                    else
                    {
                        TempData["Error"] = "You have already reviewed this product.";
                        return RedirectToAction("Review", "Review", new { area = "Seller", orderId = 3 });
                    }
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Order", new { area = "Admin" });
            }

            return RedirectToAction("Review", "Review", new { area = "Seller", orderId = 3 });
        }
    }
}
