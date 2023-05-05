
using pracical1.dataAccess;
using practical1.Models;

using Repository.Repository.IRepository;

namespace Repository.Repository
{
	public class ProdductRepository : Repository<Product>, IProductRepository
	{
		private ApplicationDbContext _db;

		public ProdductRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}


		public void update(Product obj)
		{
			var objFromDb = _db.products.FirstOrDefault(u => u.productId == obj.productId);
			if (objFromDb != null)
			{
				objFromDb.Name = obj.Name;
				objFromDb.Description = obj.Description;
				objFromDb.IsActive = obj.IsActive;
				objFromDb.Price = obj.Price;
				objFromDb.CreatedOn = obj.CreatedOn;
				objFromDb.ModifiedOn = obj.ModifiedOn;
				
				objFromDb.Quantity = obj.Quantity;
				
				objFromDb.CategoryId = obj.CategoryId;
				objFromDb.Category = obj.Category;
               
                if (obj.Imageurl != null)
				{
					objFromDb.Imageurl = obj.Imageurl;
				}
			}
		}

		
	}
}
