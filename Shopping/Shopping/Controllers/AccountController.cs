using Microsoft.AspNetCore.Mvc;
using Shopping.Data;
using Shopping.Helpers;
using Shopping.Models;

namespace Shopping.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly DataContext _context;
        //private readonly ICombosHelper _combosHelper;
        //private readonly IBlobHelper _blobHelper;

        public AccountController(IUserHelper userHelper, DataContext context)
        {
            _userHelper = userHelper;
            _context = context;
            //_combosHelper = combosHelper;
            //_blobHelper = blobHelper;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
