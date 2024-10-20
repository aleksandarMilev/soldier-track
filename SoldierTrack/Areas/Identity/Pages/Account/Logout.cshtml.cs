#nullable disable
namespace SoldierTrack.Web.Areas.Identity.Pages.Account
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using SoldierTrack.Data.Models;

    public class LogoutModel : PageModel
    {
        private readonly SignInManager<Athlete> signInManager;

        public LogoutModel(SignInManager<Athlete> signInManager) => this.signInManager = signInManager;

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await this.signInManager.SignOutAsync();

            if (returnUrl != null)
            {
                return this.LocalRedirect(returnUrl);
            }

            return this.RedirectToPage();
        }
    }
}
