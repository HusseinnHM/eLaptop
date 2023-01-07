using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eLaptop;
using eLaptop.Data;
using eLaptop.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public class DataSeed
{
    public static async Task Seed(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        var webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedSecurity(userManager, roleManager, context);
    }


    private static async Task SeedSecurity(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        if (roleManager.Roles.Any()) return;

        #region - Roles -

        foreach (var name in new[] { WebConstance.AdminRole, WebConstance.CustomerRole })
        {
            await roleManager.CreateAsync(new IdentityRole(name));
        }

        #endregion


        #region - Users -

        var admin = new ApplicationUser()
        {
            FullName = "Admin",
            Email = "admin@gmail.com",
            UserName = "admin",
            City = "aleppo",
            Area = "test",
            StreetAddress = "1"
        }; 
        await userManager.CreateAsync(admin, "1234");
        await userManager.AddToRoleAsync(admin, WebConstance.AdminRole);

        #endregion
    }
    
}