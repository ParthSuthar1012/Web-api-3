using practical1.Models;


namespace Repository.Repository.IRepository
{
    public interface IEmpolyeeRepository : IRepository<Empolyee>
    {
       
        void update(Empolyee empolyee);
    }
}
