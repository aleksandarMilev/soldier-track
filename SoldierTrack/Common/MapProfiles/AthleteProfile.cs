namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Web.Models.Athlete;

    public class AthleteProfile : Profile
    {
        public AthleteProfile()
        {
            this.CreateMap<AthleteFormModel, AthleteServiceModel>()
                .ReverseMap();
        }
    }
}
