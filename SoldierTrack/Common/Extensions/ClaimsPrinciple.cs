namespace SoldierTrack.Web.Common.Extensions
{
    using System.Security.Claims;

    using static Constants.WebConstants;

    public static class ClaimsPrinciple
    {
        public static string? GetId(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(AdminRoleName);
    }
}
