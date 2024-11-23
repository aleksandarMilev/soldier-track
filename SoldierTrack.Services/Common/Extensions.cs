namespace SoldierTrack.Services.Common
{
    using Data;
    using Data.Models;
    using Data.Models.Base;
    using Microsoft.EntityFrameworkCore;

    using static SoldierTrack.Services.Common.Constants;

    public static class Extensions
    {
        public static IQueryable<T> AllDeletable<T>(this ApplicationDbContext data)
            where T : class, IDeletableEntity 
                => data
                    .Set<T>()
                    .Where(e => !e.IsDeleted);

        public static IQueryable<T> AllDeletableAsNoTracking<T>(this ApplicationDbContext data)
           where T : class, IDeletableEntity 
                => data
                    .Set<T>()
                    .AsNoTracking()
                    .Where(e => !e.IsDeleted);

        public static IQueryable<Athlete> AllAthletes(this ApplicationDbContext data) 
            => data
               .Set<Athlete>()
               .Where(a => !a.IsDeleted && a.Email != AdminEmail);

        public static IQueryable<Athlete> AllAthletesAsNoTracking(this ApplicationDbContext data) 
            => data
                .Set<Athlete>()
                .AsNoTracking()
                .Where(a => !a.IsDeleted && a.Email != AdminEmail);
    }
}
