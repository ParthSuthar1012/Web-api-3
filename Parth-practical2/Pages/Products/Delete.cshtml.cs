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
    public class DeleteModel : PageModel
    {
        private readonly pracical1.dataAccess.ApplicationDbContext _context;

        public DeleteModel(pracical1.dataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products.FirstOrDefaultAsync(m => m.productId == id);

            if (product == null)
            {
                return NotFound();
            }
            else 
            {
                ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "Name");
                Product = product;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }
            var product = await _context.products.FindAsync(id);

            if (product != null)
            {
                Product = product;
                _context.products.Remove(Product);
                TempData["success"] = " Deleted successfully!!";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
