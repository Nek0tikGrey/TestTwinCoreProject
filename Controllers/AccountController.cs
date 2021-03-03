using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestTwinCoreProject.Models;
using TestTwinCoreProject.ViewModels;

namespace TestTwinCoreProject.Controllers
{
    [Authorize(Roles ="User")]
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
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult>Register(string id)
        {
            if (_context.InviteUsers.Any(predicate => predicate.InviteCode == id))
            {
                RegisterViewModel model = new RegisterViewModel();
                var invite = _context.InviteUsers.FirstOrDefault(predicate => predicate.InviteCode == id);

                if (invite == null)
                    return NotFound("Invite code is not found");
                Account user =await _userManager.FindByIdAsync(invite.UserId.ToString());
                model.Email = user.Email;
                model.InviteCode = id;
                model.DateBirthday = DateTime.Now;
                return View(model);
            }
            return BadRequest("Invalid invide code");
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var invite = _context.InviteUsers.FirstOrDefault(predicate => predicate.InviteCode == model.InviteCode);

                if (invite == null)
                    return NotFound("Invite code is not found");
                Account user = await _userManager.FindByIdAsync(invite.UserId.ToString());
                user.DateBirthday = model.DateBirthday;
                var resultInformation = await _userManager.UpdateAsync(user);


                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<Account>)) as IPasswordValidator<Account>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<Account>)) as IPasswordHasher<Account>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, model.Password);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                        await _userManager.UpdateAsync(user);
                        await _userManager.AddToRoleAsync(user, "User");
                        return RedirectToAction("Index","Home");
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

                if (resultInformation.Succeeded)
                {
                    await _signInManager.SignInAsync(user,false);
                    _context.InviteUsers.Remove(invite);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in resultInformation.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [AllowAnonymous]
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
        [AllowAnonymous]
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
            data.BirthDate =DateTime.Parse( user.DateBirthday.ToString("yyyy-MM-dd"));
            data.Id = user.Id;
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserInformation(UserAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.DateBirthday = model.BirthDate;
                await  _userManager.UpdateAsync(user);
            }
            return RedirectToAction("UserAccount");
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
