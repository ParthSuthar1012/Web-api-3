using practical1.Models;


namespace Repository.Repository.IRepository
{
    public interface IProductRepository:IRepository<Product>
    {
       
        void update(Product product);
    }
}
