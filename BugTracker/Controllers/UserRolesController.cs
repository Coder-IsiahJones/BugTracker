using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IRolesService _rolesService;
        private readonly ICompanyInfoService _companyInfoService;

        public UserRolesController(IRolesService rolesService, ICompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();

            int companyId = User.Identity.GetCompanyId().Value;

            List<User> users = await _companyInfoService.GetAllMembersAsync(companyId);

            foreach (User user in users)
            {
                ManageUserRolesViewModel viewModel = new();

                viewModel.User = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            User user = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(x => x.Id == member.User.Id);

            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(user);

            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                if (await _rolesService.RemoveUserFromRolesAsync(user, roles))
                {
                    await _rolesService.AddUserToRoleAsync(user, userRole);
                }
            }

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}