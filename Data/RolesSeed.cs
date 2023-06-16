using System;
using System.Linq;
using System.Threading.Tasks;
using Birthminder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Birthminder.Data
{
    public static class RoleSeed
    {
        private static object _userManager;

        public static async Task SeedRolesAsync(UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            await roleManager.CreateAsync(new AppRole(Roles.Employee.ToString()));
            await roleManager.CreateAsync(new AppRole(Roles.Admin.ToString()));
        }

    }

}