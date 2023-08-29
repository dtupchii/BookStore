using Books.DataAccess.Data;
using Books.DataAccess.Repository.IRepository;
using Books.Models;
using Books.Models.ViewModels;
using Books.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDBContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            string roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
            ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == userId).Include(u => u.Company).FirstOrDefault();

            IEnumerable<SelectListItem> roles = _db.Roles.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Name
            });
            var companyList = _db.Companies.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });


            UserRoleVM UserRoleVM = new ()
            {
                ApplicationUser = userFromDb,
                RolesList = roles,
                CompanyList = companyList
            };

            UserRoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;

            return View(UserRoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(UserRoleVM userRoleVM)
        {
            string roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userRoleVM.ApplicationUser.Id).RoleId;
            ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == userRoleVM.ApplicationUser.Id).Include(u => u.Company).FirstOrDefault();

            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;

            if (userRoleVM.ApplicationUser.Role != oldRole)
            {
                //a role was updated
                if (userRoleVM.ApplicationUser.Role == SD.Role_Company)
                {
                    userFromDb.CompanyId = userRoleVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    userFromDb.CompanyId = null;
                }

                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(userFromDb, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(userFromDb, userRoleVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }

            return RedirectToAction("Index");
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> applicationUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in applicationUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new { data = applicationUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = true, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();

            return Json(new { success = true, message = "Action completed successfully" });
        }
        #endregion

    }
}
