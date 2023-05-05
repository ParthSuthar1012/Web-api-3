
using pracical1.dataAccess;
using practical1.Models;

using Repository.Repository.IRepository;

namespace Repository.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository 
    {
        private ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext _db) : base(_db)
        {
            _context = _db;
        }

       



        public void update(Category obj)
        {
            _context.categories.Update(obj);
        }
    }
}
