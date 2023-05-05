using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pracical1.dataAccess;
using practical1.Models;

namespace Parth_practical2
{
    public class IndexModel : PageModel
    {
        private readonly pracical1.dataAccess.ApplicationDbContext _context;

        public IndexModel(pracical1.dataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; } = default!;
        
        public async Task OnGetAsync(string searchString)
        {

            ViewData["CategoryId"] = new SelectList(_context.categories, "Name", "Name");
            if (_context.products != null)
            {
                var products = await _context.products.Include(a => a.Category).ToListAsync();

                if (!string.IsNullOrEmpty(searchString))
                {
                    products = await _context.products.Where(c => c.Name.ToLower().Contains(searchString.ToLower()) || c.Description.ToLower().Contains(searchString.ToLower())).Include(a => a.Category).ToListAsync();
                }
                Product =  products.ToList() ;
                

            }
        }

        public IActionResult OnPostToggleStatus(int id)
        {
            Product product = _context.products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // Toggle the status of the product
            product.IsActive = !product.IsActive;
            _context.SaveChanges();

            return RedirectToPage();
        }










    }
}
