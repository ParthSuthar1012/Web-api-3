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
    public class EditModel : PageModel
    {
        private readonly pracical1.dataAccess.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(pracical1.dataAccess.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;   
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product =  await _context.products.FirstOrDefaultAsync(m => m.productId == id);
            if (product == null)
            {
                return NotFound();
            }
            
            Product = product;
         
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(wwwRootPath, @"Images\Product");
                var extension = Path.GetExtension(file.FileName);

                if (Product.Imageurl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, Product.Imageurl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                Product.Imageurl = @"\Images\Product\" + fileName + extension;
            }
            
            Product.ModifiedOn = DateTime.Now;
            _context.Attach(Product).State = EntityState.Modified;

            try
            {
                TempData["success"] = " Update successfully!!";
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.productId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
          return (_context.products?.Any(e => e.productId == id)).GetValueOrDefault();
        }
    }
}
