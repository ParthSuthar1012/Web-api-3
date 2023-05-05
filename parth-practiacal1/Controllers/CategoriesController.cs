using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using parth_practiacal1.Models;
using pracical1.dataAccess;
using practical1.Models;

namespace parth_practiacal1.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public IActionResult Index(string searchString)
        {
            var categories = _context.categories.ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c => c.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }

            return View(categories);
        }


        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.categories == null)
            {
                return NotFound();
            }

          
            var category = await _context.categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
         
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        private IActionResult View(Category category, Category model)
        {
            throw new NotImplementedException();
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Name,Description,IsActive,CreatedOn,ModifiedOn,ParentCategoryId")] Category category)
        {
            if (ModelState.IsValid)
            {    category.CreatedOn = DateTime.Now;
               
                _context.Add(category);
                TempData["success"] = " Crated successfully!!";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["warning"] = "Add valid Data";
            }
            return View(category);
        }

        public IActionResult Addchild()
        {
            var model = new Category
            {
            
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Addchild([Bind("CategoryId,Name,Description,IsActive,CreatedOn,ModifiedOn,ParentCategoryId")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                TempData["success"] = "Child Added Successfully";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.categories == null)
            {
                return NotFound();
            }

            var category = await _context.categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,Description,IsActive,CreatedOn,ModifiedOn,ParentCategoryId")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.ModifiedOn = DateTime.Now;
                    _context.Update(category);
                    TempData["success"] = "Update Successfully";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var categoryfromdb = _context.categories.Find(id);
            if (categoryfromdb == null)
            {
                return NotFound();
            }
            return View(categoryfromdb);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category obj)
        {
            DeleteCategoryAndSubcategories(obj.CategoryId);
            TempData["success"] = "Delete Successfully";
            return RedirectToAction("Index");
        }

        // for subcategory check
        private void DeleteCategoryAndSubcategories(int categoryId)
        {
            var subcategories = _context.categories.Where(x => x.ParentCategoryId == categoryId).ToList();

            foreach (var subcategory in subcategories)
            {
                DeleteCategoryAndSubcategories(subcategory.CategoryId);
            }

            var category = _context.categories.Find(categoryId);

            if (category != null)
            {
                _context.categories.Remove(category);
                _context.SaveChanges();
            }
        }

        //for status change 
        public IActionResult ChangeStatus(int id)
        {
            var category = _context.categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            category.IsActive = !category.IsActive; 

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private bool CategoryExists(int id)
        {
          return (_context.categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
