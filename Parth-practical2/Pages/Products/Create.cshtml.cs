using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using pracical1.dataAccess;
using practical1.Models;

namespace Parth_practical2
{
    public class CreateModel : PageModel
    {
        private readonly pracical1.dataAccess.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
      

        public CreateModel(pracical1.dataAccess.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;   
        }

        public IActionResult OnGet()
        {
        ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "Name");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public  IActionResult OnPost(IFormFile? file)
        {
            if (!ModelState.IsValid || _context.products == null || Product == null)
            {
              
                
                return Page();
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(wwwRootPath, @"Images\Product");
                var extension = Path.GetExtension(file.FileName);
                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                Product.Imageurl = @"\Images\Product\" + fileName + extension;
            }
            Product.CreatedOn= DateTime.Now;
            _context.products.Add(Product);
            TempData["success"] = " Crated successfully!!";
            _context.SaveChanges();

            return RedirectToPage("./Index");
        }
    }
}
