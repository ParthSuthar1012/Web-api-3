using practical1.Models;


namespace Repository.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
       
        void update(Order order);
    }
}
