using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject
{
    public class DataInitializer
    {
        public static async Task InitializeAsync(UserManager<Account> userManager, RoleManager<ApplicationRole> roleManager)
        {
            string adminEmail = "sstopchak@gmail.com";
            string pas = "asdasdasdasD123123$";
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new ApplicationRole("Admin"));
            }
            if (await roleManager.FindByNameAsync("employee") == null)
            {
                await roleManager.CreateAsync(new ApplicationRole("User"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new Account { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, pas);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    await userManager.AddToRoleAsync(admin, "User");
                }
            }
        }
    }
}
