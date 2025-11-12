using book.DataAccess.Data;
using book.DataAccess.Repository;
using book.DataAccess.Repository.IRepository;
using book.Models.ViewModel;
using book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bookNest.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerOrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public SellerOrderController(ApplicationDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _db = dbContext;
            _unitOfWork = unitOfWork;

        }

        [BindProperty]
        public OrderVm OrderVm { get; set; }

        public IActionResult Index()
        {    

            return View();
        }
        public IActionResult getOrderList(string status)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Here you would typically retrieve orders for the seller using userId
            if (status == "all")
            {
                var obj = (from order in _db.OrderHeaders
                           join orderDetail in _db.OrderDetails on order.Id equals orderDetail.OrderHeaderId
                           join product in _db.Products on orderDetail.ProductId equals product.Id
                           join user in _db.ApplicationUsers on order.ApplicationUserId equals user.Id
                           where product.userId == userId
                           select new
                           {
                               OrderId = order.Id,
                               CustomerName = order.Name,
                               TotalAmount = order.OrderTotal,
                               OrderStatus = order.OrderStatus,
                               Email = user.Email,
                               PhoneNumber = order.PhoneNumber
                           }).ToList();

                return new JsonResult(new { data = obj });
            }
            else
            {
                var obj = (from order in _db.OrderHeaders
                           join orderDetail in _db.OrderDetails on order.Id equals orderDetail.OrderHeaderId
                           join product in _db.Products on orderDetail.ProductId equals product.Id
                           join user in _db.ApplicationUsers on order.ApplicationUserId equals user.Id
                           where product.userId == userId && order.OrderStatus == status
                           select new
                           {
                               OrderId = order.Id,
                               CustomerName = order.Name,
                               TotalAmount = order.OrderTotal,
                               OrderStatus = order.OrderStatus,
                               Email = user.Email,
                               PhoneNumber = order.PhoneNumber
                           }).ToList();

                return new JsonResult(new { data = obj });
            }

        }
        public IActionResult Details(int orderId)
        {
            OrderVm = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "product")
            };

            return View(OrderVm);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_Seller)]
        public IActionResult UpdateOrderDetail(int orderId)
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVm.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVm.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVm.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVm.OrderHeader.City;
            orderHeaderFromDb.State = OrderVm.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVm.OrderHeader.PostalCode;
            orderHeaderFromDb.ShippingDate = OrderVm.OrderHeader.ShippingDate;


            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Order Details Updated Successfully";



            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_Seller)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVm.OrderHeader.Id, SD.StatusInProcess, "Started processing");
            _unitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVm.OrderHeader.Id });

        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_Seller)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVm.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVm.OrderHeader.Carrier;
            orderHeader.ShippingDate = DateTime.Now;
            orderHeader.OrderStatus = SD.StatusShipped;

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVm.OrderHeader.Id });

        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_Seller)]
        public IActionResult CancelOrder()
        {

            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id);
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, "Cancelled");
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVm.OrderHeader.Id });

        }
    }
    }

