using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Vereyon.Web;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public CategoriesController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Add(category);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        //catch (DbUpdateException exception) when (exception?.InnerException?.Message.Contains("Cannot insert duplicate key row in object") ?? false)
        //        //{
        //        //}
        //        catch (DbUpdateException dbUpdateException)
        //        {
        //            //if (dbUpdateException.InnerException.Message.Contains("duplicada"))
        //            //{
        //            //    ModelState.AddModelError(string.Empty, "Ya existe una Categoria con el mismo nombre.");
        //            //}
        //            //else
        //            var dv = dbUpdateException?.InnerException;
        //            if (dv != null && dv.InnerException != null)
        //            {
        //                ModelState.AddModelError(string.Empty, "Ya existe una Categoria con el mismo nombre.");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            ModelState.AddModelError(string.Empty, exception.Message);
        //        }
        //    }
        //    return View(category);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //var ret2 = dbUpdateException?.InnerException;
        //        try
        //        {
        //            _context.Add(category);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        //catch (DbUpdateException exception) when (exception?.InnerException?.Message.Contains("duplicada") ?? false)
        //        //{
        //        //    ModelState.AddModelError(string.Empty, "Ya existe una Categoria con el mismo nombre.");
        //        //}

        //        catch (DbUpdateException dbUpdateException)
        //        {
        //            var ret = dbUpdateException?.InnerException;

        //            //if (dbUpdateException.InnerException != null && dbUpdateException.InnerException.Message.Contains("duplicada"))
        //            if (ret != null && ret.Message.Contains("duplicada"))
        //            {
        //                ModelState.AddModelError(string.Empty, "Ya existe una Categoria con el mismo nombre.");

        //            }
        //            else
        //            {
        //                ModelState.AddModelError(string.Empty, $"{ret?.InnerException?.Message}");
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            ModelState.AddModelError(string.Empty, exception.Message);
        //        }
        //    }
        //    return View(category);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                //var ret2 = dbUpdateException?.InnerException;
                try
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                //catch (DbUpdateException exception) when (exception?.InnerException?.Message.Contains("duplicada") ?? false)
                //{
                //    ModelState.AddModelError(string.Empty, "Ya existe una Categoria con el mismo nombre.");
                //}

                catch (DbUpdateException dbUpdateException)
                {
                    var ret = dbUpdateException?.InnerException;

                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        //ModelState.AddModelError(string.Empty, "Ya existe una Categoria con el mismo nombre.");
                        _flashMessage.Danger("Ya existe una Categoria con el mismo nombre.");

                    }
                    else
                    {
                        //ModelState.AddModelError(string.Empty, $"{ret?.InnerException?.Message}");
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    //ModelState.AddModelError(string.Empty, exception.Message);
                    _flashMessage.Danger(exception.Message);
                }
            }
            return View(category);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe una categoría con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }

            return View(category);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Category category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro borrado.");
            return RedirectToAction(nameof(Index));
        }
    }
}
