#nullable disable
namespace SoldierTrack.Web.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data.Models;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.ModelValidationConstants.AthleteConstants;

    public class RegisterModel : PageModel
    {
        private readonly UserManager<Athlete> userManager;
        private readonly SignInManager<Athlete> signInManager;
        private readonly IUserStore<Athlete> userStore;

        public RegisterModel(
            UserManager<Athlete> userManager,
            SignInManager<Athlete> signInManager,
            IUserStore<Athlete> userStore)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userStore = userStore;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = RequiredError)]
            [StringLength(
                NamesMaxLength,
                MinimumLength = NamesMinLength,
                ErrorMessage = LengthError)]
            [Display(Name = "First Name")]
            public string FirstName { get; init; } = null!;

            [Required(ErrorMessage = RequiredError)]
            [StringLength(
                    NamesMaxLength,
                    MinimumLength = NamesMinLength,
                    ErrorMessage = LengthError)]
            [Display(Name = "Last Name")]
            public string LastName { get; init; } = null!;

            [Required]
            [Display(Name = "Phone Number")]
            [StringLength(
                    PhoneLength,
                    MinimumLength = PhoneLength,
                    ErrorMessage = LengthError)]
            [RegularExpression("\\b\\d{10}\\b", ErrorMessage = InvalidPhoneFormat)]
            public string PhoneNumber { get; init; } = null!;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; init; }

            [Required]
            [StringLength(
                100,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; init; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; init; }
        }

        public void OnGet(string returnUrl = null) => this.ReturnUrl = returnUrl;

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");

            var phone = this.Input.PhoneNumber;
            if (await this.userManager.Users.AnyAsync(u => u.PhoneNumber == phone))
            {
                this.ModelState.AddModelError("Input.PhoneNumber", string.Format(PhoneDuplicate, phone));
                return this.Page();
            }

            var email = this.Input.Email;
            if (await this.userManager.Users.AnyAsync(u => u.Email == email))
            {
                this.ModelState.AddModelError("Input.Email", string.Format(EmailDuplicate, email));
                return this.Page();
            }

            if (this.ModelState.IsValid)
            {
                var user = new Athlete()
                {
                    FirstName = this.Input.FirstName,
                    LastName = this.Input.LastName,
                    Email = this.Input.Email,
                    PhoneNumber = this.Input.PhoneNumber,
                };

                await this.userStore.SetUserNameAsync(user, this.Input.Email, CancellationToken.None);
                var result = await this.userManager.CreateAsync(user, this.Input.Password);

                if (result.Succeeded)
                {
                    await this.signInManager.SignInAsync(user, isPersistent: true);

                    this.TempData["SuccessMessage"] = "Welcome!";
                    return this.LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return this.Page();
        }
    }
}
