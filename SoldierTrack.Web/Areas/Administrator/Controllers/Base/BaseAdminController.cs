namespace SoldierTrack.Web.Areas.Administrator.Controllers.Base
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static SoldierTrack.Web.Constants.WebConstants;

    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class BaseAdminController : Controller
    {
    }
}
