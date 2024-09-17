namespace SoldierTrack.Services.Common
{
    using AutoMapper;

    public static class AutoMapperConfig<T>
        where T : Profile, new()
    {
        public static IMapper CreateMapper()
        {
            var mapConfiguration = new MapperConfiguration(config =>
            {
                config.AddProfile<T>();
            });

            return mapConfiguration.CreateMapper();
        }
    }
}
