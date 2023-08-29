using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Books.Models.ViewModels
{
    public class UserRoleVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CompanyList { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}
