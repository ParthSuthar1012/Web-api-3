using pracical1.dataAccess;
using Microsoft.EntityFrameworkCore;
using Repository.Repository.IRepository;
using System.Linq.Expressions;

namespace Repository.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbset;
        private ApplicationDbContext db;

        public Repository(ApplicationDbContext db)
		{
			_context =db;
           
			this.dbset = _context.Set<T>();
		}


   
        public void Add(T entitiy)
        {
            dbset.Add(entitiy);
        }

        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (includeProperties != null)
            {
                foreach(var property in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(property);
                }
            }
         
            return query.ToList();  
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {

            IQueryable<T> query = dbset;
            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            query =query.Where(filter);
             return query.FirstOrDefault();
        }

        public void Remove(T entitiy)
        {
           dbset.Remove(entitiy);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
           dbset.RemoveRange(entities);
        }
    }
}
