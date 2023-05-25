
using pracical1.dataAccess;
using practical1.Models;

using Repository.Repository.IRepository;

namespace Repository.Repository
{
    public class EmpolyeeRepository : Repository<Empolyee>, IEmpolyeeRepository 
    {
        private ApplicationDbContext _context;
        public EmpolyeeRepository(ApplicationDbContext _db) : base(_db)
        {
            _context = _db;
        }

       



        public void update(Empolyee obj)
        {
            var objFromDb = _context.empolyees.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Salary = obj.Salary;
                objFromDb.Phone = obj.Phone;
                objFromDb.Email = obj.Email;
                objFromDb.Department = obj.Department;
            }
        }
    }
}
