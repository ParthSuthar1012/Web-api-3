
using pracical1.dataAccess;
using practical1.Models;

using Repository.Repository.IRepository;

namespace Repository.Repository
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        private ApplicationDbContext _context;
        public OrderItemRepository(ApplicationDbContext _db) : base(_db)
        {
            _context = _db;
        }

       



      
    }
}
