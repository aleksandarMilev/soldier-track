namespace SoldierTrack.Services.Common
{
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models.Base;

    public static class QueryableExtension
    {
        public static IQueryable<T> AllDeletable<T>(this ApplicationDbContext data) 
            where T : class, IDeletableEntity
        {
            return data.Set<T>().Where(e => !e.IsDeleted);
        }

        public static IQueryable<T> AllDeletableAsNoTracking<T>(this ApplicationDbContext data)
           where T : class, IDeletableEntity
        {
            return data.Set<T>().AsNoTracking().Where(e => !e.IsDeleted);
        }
    }
}
