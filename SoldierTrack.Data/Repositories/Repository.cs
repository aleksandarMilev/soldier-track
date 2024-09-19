namespace SoldierTrack.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data.Repositories.Base;

    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly ApplicationDbContext data;

        public Repository(ApplicationDbContext data) => this.data = data;

        public IQueryable<T> All() => this.data.Set<T>();

        public IQueryable<T> AllAsNoTracking() => this.data.Set<T>().AsNoTracking();

        public void Add(T entity) => this.data.Add(entity);

        public void Update(T entity) => this.data.Update(entity);

        public void Delete(T entity) => this.data.Remove(entity);

        public async Task SaveChangesAsync() => await this.data.SaveChangesAsync();
    }
}
