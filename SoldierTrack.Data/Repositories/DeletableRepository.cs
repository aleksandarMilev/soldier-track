namespace SoldierTrack.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data.Models.Base;
    using SoldierTrack.Data.Repositories.Base;

    public class DeletableRepository<T> : IDeletableRepository<T>
        where T : class, IDeletableEntity
    {
        private readonly ApplicationDbContext data;

        public DeletableRepository(ApplicationDbContext data) => this.data = data;

        public IQueryable<T> All() => this.data.Set<T>().Where(e => !e.IsDeleted);

        public IQueryable<T> AllAsNoTracking() => this.data.Set<T>().AsNoTracking().Where(e => !e.IsDeleted);

        public IQueryable<T> AllWithDeleted() => this.data.Set<T>();

        public IQueryable<T> AllWithDeletedAsNoTracking() => this.data.Set<T>().AsNoTracking();

        public void Add(T entity) => this.data.Add(entity);

        public void Update(T entity) => this.data.Update(entity);

        public void SoftDelete(T entity) => this.data.SoftDelete(entity);

        public void Delete(T entity) => this.data.Remove(entity);

        public void Restore(T entity) => this.data.Restore(entity);

        public async Task SaveChangesAsync() => await this.data.SaveChangesAsync();
    }
}
