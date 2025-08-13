namespace SoldierTrack.Web.WebServices.CurrentUser
{
    public interface ICurrentUserService
    {
        string? GetUsername();

        string? GetId();

        bool IsAdmin();
    }
}
