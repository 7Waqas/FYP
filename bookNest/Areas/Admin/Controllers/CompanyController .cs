
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
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            
            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id)
        {
                  

           
            
            if (id == null || id == 0)
            {
                //create company
                return View(new Company());
            }
            else
            {
                //update company
                Company companyobj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyobj);
            }

            
        }
        [HttpPost]
        public IActionResult Upsert(Company Companyobj, IFormFile? file)
        {
            
            if (ModelState.IsValid)
            {
                
                if (Companyobj.Id == 0)
                {
                    _unitOfWork.Company.Add(Companyobj);
                }
                else
                {
                    _unitOfWork.Company.Update(Companyobj);
                }

               
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {

                return View(Companyobj);
            }
           
        }
       
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted= _unitOfWork.Company.Get(u => u.Id == id);
            if(companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
