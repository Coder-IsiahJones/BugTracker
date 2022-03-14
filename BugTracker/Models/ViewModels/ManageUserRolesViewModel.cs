using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BugTracker.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public User User { get; set; }

        public MultiSelectList Roles { get; set; }

        public List<string> SelectedRoles { get; set; }
    }
}