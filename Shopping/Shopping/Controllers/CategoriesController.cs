using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using Vereyon.Web;
using static Shopping.Helpers.ModalHelper;

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
            //return View(await _context.Categories.ToListAsync());
            return View(await _context.Categories
                .Include(c => c.ProductCategories)
                .ToListAsync());
        }
        //public IActionResult Create()
        //{
        //    return View();
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

        //            if (dbUpdateException.InnerException.Message.Contains("duplicada"))
        //            {
        //                //ModelState.AddModelError(string.Empty, "Ya existe una Categoria con el mismo nombre.");
        //                _flashMessage.Danger("Ya existe una Categoria con el mismo nombre.");

        //            }
        //            else
        //            {
        //                //ModelState.AddModelError(string.Empty, $"{ret?.InnerException?.Message}");
        //                _flashMessage.Danger(dbUpdateException.InnerException.Message);
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            //ModelState.AddModelError(string.Empty, exception.Message);
        //            _flashMessage.Danger(exception.Message);
        //        }
        //    }
        //    return View(category);
        //}
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Category category = await _context.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Category category)
        //{
        //    if (id != category.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(category);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateException dbUpdateException)
        //        {
        //            if (dbUpdateException.InnerException.Message.Contains("duplicada"))
        //            {
        //                _flashMessage.Danger("Ya existe una categoría con el mismo nombre.");
        //            }
        //            else
        //            {
        //                _flashMessage.Danger(dbUpdateException.InnerException.Message);
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            _flashMessage.Danger(exception.Message);
        //        }
        //    }

        //    return View(category);
        //}
        

        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar la categoría porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Category());
            }
            else
            {
                Category category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
        }

        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Category category = await _context.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(category);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado.");
                    }
                    else //Update
                    {
                        _context.Update(category);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro actualizado.");
                    }
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
                    return View(category);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(category);
                }

                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.Categories.Include(c => c.ProductCategories).ToList()) });

            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", category) });
        }


        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Category category = await _context.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    Category category = await _context.Categories.FindAsync(id);
        //    _context.Categories.Remove(category);
        //    await _context.SaveChangesAsync();
        //    _flashMessage.Info("Registro borrado.");
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
