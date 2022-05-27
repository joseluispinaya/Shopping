#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using Shopping.Models;
using Vereyon.Web;
using static Shopping.Helpers.ModalHelper;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public CountriesController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Countries.ToListAsync());
            return View(await _context.Countries
                .Include(c => c.States)
                .ThenInclude(s => s.Cities)
                .ToListAsync());
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries
                .Include(c => c.States)
                .ThenInclude(s => s.Cities)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        //public IActionResult Create()
        //{
        //    Country country = new() { States = new List<State>() };
        //    return View(country);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Country country)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Add(country);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateException dbUpdateException)
        //        {
        //            if (dbUpdateException.InnerException.Message.Contains("duplicada"))
        //            {
        //                ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
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
        //    return View(country);
        //}

        [NoDirectAccess]
        public async Task<IActionResult> AddState(int id)
        {
            
            Country country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            StateViewModel model = new()
            {
                CountryId = country.Id,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddState(StateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    State state = new()
                    {
                        Cities = new List<City>(),
                        Country = await _context.Countries.FindAsync(model.CountryId),
                        Name = model.Name,
                    };
                    _context.Add(state);
                    await _context.SaveChangesAsync();
                    Country country = await _context.Countries
                        .Include(c => c.States)
                        .ThenInclude(s => s.Cities)
                        .FirstOrDefaultAsync(c => c.Id == model.CountryId);
                    _flashMessage.Info("Registro creado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllStates", country) });

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un departamento/estado con el mismo nombre en este país.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddState", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> EditState(int id)
        {
           
            State state = await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            StateViewModel model = new()
            {
                CountryId = state.Country.Id,
                Id = state.Id,
                Name = state.Name,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditState(int id, StateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    State state = new()
                    {
                        Id = model.Id,
                        Name = model.Name,
                    };
                    _context.Update(state);
                    Country country = await _context.Countries
                        .Include(c => c.States)
                        .ThenInclude(s => s.Cities)
                        .FirstOrDefaultAsync(c => c.Id == model.CountryId);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Registro actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllStates", country) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un Departamento / Estado con el mismo nombre en este país.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditState", model) });
        }
        public async Task<IActionResult> DetailsState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            State state = await _context.States
                .Include(s => s.Country)
                .Include(s => s.Cities)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddCity(int id)
        {
            
            State state = await _context.States.FindAsync(id);
            if (state == null)
            {
                return NotFound();
            }

            CityViewModel model = new()
            {
                StateId = state.Id,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    City city = new()
                    {
                        State = await _context.States.FindAsync(model.StateId),
                        Name = model.Name,
                    };
                    _context.Add(city);
                    await _context.SaveChangesAsync();
                    State state = await _context.States
                        .Include(s => s.Cities)
                        .FirstOrDefaultAsync(c => c.Id == model.StateId);
                    _flashMessage.Confirmation("Registro creado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllCities", state) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una ciudad con el mismo nombre en este Departamento / Estado.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddCity", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> EditCity(int id)
        {

            City city = await _context.Cities
                .Include(c => c.State)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            CityViewModel model = new()
            {
                StateId = city.State.Id,
                Id = city.Id,
                Name = city.Name,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(int id, CityViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    City city = new()
                    {
                        Id = model.Id,
                        Name = model.Name,
                    };
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                    State state = await _context.States
                        .Include(s => s.Cities)
                        .FirstOrDefaultAsync(c => c.Id == model.StateId);
                    _flashMessage.Confirmation("Registro actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllCities", state) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una ciuad con el mismo nombre en este Departamento / Estado.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditCity", model) });
        }
        //public async Task<IActionResult> DetailsCity(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    City city = await _context.Cities
        //        .Include(c => c.State)
        //        .FirstOrDefaultAsync(c => c.Id == id);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(city);
        //}


        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Country country = await _context.Countries
        //        .Include(c => c.States)
        //        .FirstOrDefaultAsync(c => c.Id == id);
        //    if (country == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(country);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Country country)
        //{
        //    if (id != country.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(country);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateException dbUpdateException)
        //        {
        //            if (dbUpdateException.InnerException.Message.Contains("duplicada"))
        //            {
        //                ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
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
        //    return View(country);
        //}

        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Country country = await _context.Countries
        //        .Include(c => c.States)
        //        .FirstOrDefaultAsync(c => c.Id == id);
        //    if (country == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(country);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var country = await _context.Countries.FindAsync(id);
        //    _context.Countries.Remove(country);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        [NoDirectAccess]
        public async Task<IActionResult> Delete(int id)
        {
            
            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            try
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el país porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Country());
            }
            else
            {
                Country country = await _context.Countries.FindAsync(id);
                if (country == null)
                {
                    return NotFound();
                }

                return View(country);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(country);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado.");
                    }
                    else //Update
                    {
                        _context.Update(country);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro actualizado.");
                    }
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(
                            this,
                            "_ViewAll",
                            _context.Countries
                                .Include(c => c.States)
                                .ThenInclude(s => s.Cities)
                                .ToList())
                    });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un país con el mismo nombre.");
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

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", country) });
        }


        //public async Task<IActionResult> DeleteState(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    State state = await _context.States
        //        .Include(s => s.Country)
        //        .FirstOrDefaultAsync(s => s.Id == id);
        //    if (state == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(state);
        //}

        //[HttpPost, ActionName("DeleteState")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteStateConfirmed(int id)
        //{
        //    State state = await _context.States
        //        .Include(s => s.Country)
        //        .FirstOrDefaultAsync(s => s.Id == id);
        //    _context.States.Remove(state);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Details), new { Id = state.Country.Id });
        //}

        [NoDirectAccess]
        public async Task<IActionResult> DeleteState(int id)
        {

            State state = await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            try
            {
                _context.States.Remove(state);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el estado / departamento porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Details), new { id = state.Country.Id });
        }

        [NoDirectAccess]
        public async Task<IActionResult> DeleteCity(int id)
        {
            City city = await _context.Cities
                .Include(c => c.State)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            try
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar la ciudad porque tiene registros relacionados.");
            }

            _flashMessage.Info("Registro borrado.");
            return RedirectToAction(nameof(DetailsState), new { id = city.State.Id });
        }
    }
}
