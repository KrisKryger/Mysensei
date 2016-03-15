using Microsoft.AspNet.Identity;
using MySensei.Models;
using System.Threading.Tasks;
namespace MySensei.Infrastructure
{
    public static class IdentityExtensions
    {
        public static async Task<AppUser> FindByNameOrEmailAsync(this UserManager<AppUser> userManager, string email, string password)
        {

            var userForEmail = await userManager.FindByEmailAsync(email);
            var username = userForEmail.UserName;
            
            return await userManager.FindAsync(username, password);
        }
    }
}