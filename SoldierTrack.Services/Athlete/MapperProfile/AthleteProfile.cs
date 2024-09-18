namespace SoldierTrack.Services.Athlete.MapperProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete.Models.Base;

    public class AthleteProfile : Profile
    {
        public AthleteProfile()
        {
            this.CreateMap<AthleteServiceModel, Athlete>();
        }
    }
}
