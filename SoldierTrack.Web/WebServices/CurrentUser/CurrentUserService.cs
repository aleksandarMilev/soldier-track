namespace SoldierTrack.Web.WebServices.CurrentUser
{
    using System.Security.Claims;
    using Extensions;

    using static Constants.WebConstants;

    public class CurrentUserService(IHttpContextAccessor httpContext) : ICurrentUserService
    {
        private readonly ClaimsPrincipal user = httpContext.HttpContext?.User!;

        public string? GetUsername()
            => this.user?.Identity?.Name;

        public string? GetId()
            => this.user?.GetId();

        public bool IsAdmin()
            => this.user.IsInRole(AdminRoleName);
    }
}
