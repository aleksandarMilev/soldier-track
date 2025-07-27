namespace SoldierTrack.Web.Extensions
{
    public static class EndpointRouteBuilder
    {
        public static void MapDefaultAreaRoute(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                 name: "Areas",
                 pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        }
    }
}
