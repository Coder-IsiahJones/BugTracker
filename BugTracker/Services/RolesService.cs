using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public class RolesService : IRolesService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<bool> AddUserToRoleAsync(User user, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;

            return result;
        }

        public async Task<string> GetRoleNameByIdAsync(string roleId)
        {
            IdentityRole role = _context.Roles.Find(roleId);
            string result = await _roleManager.GetRoleNameAsync(role);

            return result;
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                List<IdentityRole> result = new();

                result = await _context.Roles.ToListAsync();

                return result;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(User user)
        {
            IEnumerable<string> result = await _userManager.GetRolesAsync(user);

            return result;
        }

        public async Task<List<User>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            List<User> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            List<User> result = users.Where(x => x.CompanyId == companyId).ToList();

            return result;
        }

        public async Task<List<User>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            List<string> userIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(x => x.Id).ToList();
            List<User> roleUser = _context.Users.Where(x => !userIds.Contains(x.Id)).ToList();
            List<User> result = roleUser.Where(x => x.CompanyId == companyId).ToList();

            return result;
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            bool result = await _userManager.IsInRoleAsync(user, roleName);

            return result;
        }

        public async Task<bool> RemoveUserFromRoleAsync(User user, string roleName)
        {
            bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;

            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(User user, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;

            return result;
        }
    }
}