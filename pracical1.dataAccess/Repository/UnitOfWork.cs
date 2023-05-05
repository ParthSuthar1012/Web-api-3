
using pracical1.dataAccess;
using Repository.Repository.IRepository;

namespace Repository.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext _db)
        {
            _context = _db;


            productRepository = new ProdductRepository(_context);
            orderRepository = new OrderRepository(_context);
            orderItemRepository = new OrderItemRepository(_context);
        }

        public ICategoryRepository CategoryRepository { get; private set; }

        public IProductRepository productRepository { get; private set; }
        public IOrderRepository orderRepository { get; private set; }
        public IOrderItemRepository orderItemRepository { get; private set; }



        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
