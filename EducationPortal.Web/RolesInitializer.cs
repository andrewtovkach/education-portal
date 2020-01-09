using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EducationPortal.Web
{
    public class RolesInitializer
    {
        public static IEnumerable<string> RoleNamesCollection = new List<string> { "admin", "tutor", "user" };
        private const string AdminEmail = "admin@gmail.com";
        private const string AdminPassword = "Admin123456@";

        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in RoleNamesCollection)
            {
                await AddNewRoleIfNotExist(roleManager, roleName);
            }

            await CreateAdminUserIfNotExist(userManager, AdminEmail, AdminPassword);
        }

        private static async Task CreateAdminUserIfNotExist(UserManager<IdentityUser> userManager, string adminEmail, string adminPassword)
        {
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new IdentityUser
                {
                    Email = adminEmail,
                    UserName = adminEmail
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }

        private static async Task AddNewRoleIfNotExist(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
