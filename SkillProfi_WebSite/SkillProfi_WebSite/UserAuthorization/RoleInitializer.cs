using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.UserAuthorization
{
    public class RoleInitializer
    {
        public static readonly string RoleAdmin = "administrator";
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminLogin = "admin";
            string password = "!Aa123";
            if (await roleManager.FindByNameAsync(RoleAdmin) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(RoleAdmin));
            }
            if (await userManager.FindByNameAsync(adminLogin) == null)
            {
                User admin = new() { UserName = adminLogin };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, RoleAdmin);
                }
            }
        }
    }
}
