#nullable disable
namespace SoldierTrack.Web.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;

    using Data.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class LoginModel : PageModel
    {
        private readonly UserManager<Athlete> userManager;
        private readonly SignInManager<Athlete> signInManager;

        public LoginModel(UserManager<Athlete> userManager, SignInManager<Athlete> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(this.ErrorMessage))
            {
                this.ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= this.Url.Content("~/");
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(this.Input.Email);
                if (user != null && user.IsDeleted)
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                    return this.Page();
                }

                var result = await this.signInManager
                    .PasswordSignInAsync(
                        this.Input.Email, 
                        this.Input.Password, 
                        this.Input.RememberMe, 
                        lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    this.TempData["SuccessMessage"] = "Welcome!";

                    return this.LocalRedirect(returnUrl);
                }

                this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                return this.Page();
            }

            return this.Page();
        }
    }
}
