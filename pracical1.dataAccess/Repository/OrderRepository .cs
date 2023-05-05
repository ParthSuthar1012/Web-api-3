
using pracical1.dataAccess;
using practical1.Models;

using Repository.Repository.IRepository;

namespace Repository.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository 
    {
        private ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext _db) : base(_db)
        {
            _context = _db;
        }

       



        public void update(Order obj)
        {
            var objFromDb = _context.orders.FirstOrDefault(u => u.OrderId == obj.OrderId);
            if (objFromDb != null)
            {
                objFromDb.OrderDate = obj.OrderDate;
                objFromDb.Note = obj.Note;
                objFromDb.IsActive = obj.IsActive;
                objFromDb.DisountAmount = obj.DisountAmount;
                objFromDb.CreatedOn = obj.CreatedOn;
                objFromDb.ModifiedOn = obj.ModifiedOn;

                objFromDb.StatusType = obj.StatusType;

                objFromDb.TotalAmount = obj.TotalAmount;
                objFromDb.CustomerName = obj.CustomerName;
                objFromDb.CustomerEmail = obj.CustomerEmail;
                objFromDb.CustomerContactNo = obj.CustomerContactNo;


            }
        }
    }
}
