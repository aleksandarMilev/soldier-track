namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;

    using static SoldierTrack.Web.Common.Constants.WebConstants;

    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class AthleteController : Controller
    {
        private readonly IAthleteService athleteService;
        private readonly IMapper mapper;

        public AthleteController(IAthleteService athleteService, IMapper mapper)
        {
            this.athleteService = athleteService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = AdminRoleName)]
        public async Task<IActionResult> GetAll(string? searchTerm = null, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var model = await this.athleteService.GetPageModelsAsync(searchTerm, pageIndex, pageSize);
            return this.View(model);
        }
    }
}
