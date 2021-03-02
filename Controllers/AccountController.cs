using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestTwinCoreProject.Models;
using TestTwinCoreProject.ViewModels;

namespace TestTwinCoreProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly TwinCoreDbContext _context;

        public AccountController(UserManager<Account> userManager, SignInManager<Account> signInManager, TwinCoreDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Account user = new Account { Email = model.Email, UserName = model.Email, DateBirthday = model.DateBirthday };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangePassword()
        {
            Account user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Name = user.UserName };
            return View(model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                Account user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<Account>)) as IPasswordValidator<Account>;
                    var passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<Account>)) as IPasswordHasher<Account>;

                    IdentityResult result =
                        await passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);

                    if (result.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> UserAccount()
        {
            UserAccountViewModel data = new UserAccountViewModel();
            var user = await _userManager.GetUserAsync(User);
            data.UserName = user.UserName;
            data.Email = user.Email;
            data.Avatars = _context.Files.Where(p => p.TypeTo == FileModel.Type.Avatar && p.Guid == user.Id).ToList();
            data.BirthDate = user.DateBirthday;
            data.Id = user.Id;
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAvatar()
        {
            // var user = await _userManager.GetUserAsync(User);
            string path = "";
            /*path= _context.Files.LastOrDefault(p => p.Guid == user.Id).Path;*/

            if (path == string.Empty) path = "img/avatar.png";
            ViewData.Add("img", path);
            return PartialView();
        }

    }
}
