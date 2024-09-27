namespace SoldierTrack.Web.Common.Attributes.Filter
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Web.Common.Extensions;

    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class AthleteAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var identity = context.HttpContext.User.Identity;

            if (identity != null && !identity.IsAuthenticated)
            {
                context.Result = new RedirectToPageResult("/Account/Register", new { area = "Identity" });
                return;
            }

            if (context
                .HttpContext
                .RequestServices
                .GetRequiredService(typeof(IAthleteService)) is not AthleteService athleteService)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return;
            }

            var athleteExists = false;

            Task
                .Run(async () =>
                {
                    athleteExists = await athleteService.UserIsAthleteByUserIdAsync(context.HttpContext.User.GetId()!);
                })
                .GetAwaiter()
                .GetResult();

            var userIsAdmin = context.HttpContext.User.IsInRole(AdminRoleName);

            if (!athleteExists && !userIsAdmin)
            {
                context.Result = new RedirectToActionResult("Create", "Athlete", new { area = "" });
            }
        }
    }
}
