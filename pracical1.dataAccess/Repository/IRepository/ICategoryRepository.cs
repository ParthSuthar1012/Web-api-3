
using practical1.Models;

namespace Repository.Repository.IRepository
{
    public interface ICategoryRepository:IRepository<Category>
    {
       
        void update(Category category);
    }
}
