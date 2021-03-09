using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestTwinCoreProject.Models;
using TestTwinCoreProject.ViewModels;

namespace TestTwinCoreProject.Controllers
{
    [Authorize(Roles ="User")]
    public class UserController : Controller
    {
        private static readonly RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        readonly UserManager<Account> _userManager;
        private readonly TwinCoreDbContext context;
        public UserController(UserManager<Account> userManager, TwinCoreDbContext context)
        {
            _userManager = userManager;
            this.context = context;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Account user = new Account { Email = model.Email, UserName = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    InviteUser invite = new InviteUser();

                    do
                    {
                        invite.InviteCode = GenRandomString("QWERTYUIOPASDFGHJKLZXCVBNqwertyuiopasdfghjklzxcvbnm1234567890", 25);
                    } while (context.InviteUsers.Any(predicate => predicate.InviteCode == invite.InviteCode));

                    invite.UserId = user.Id;
                    context.InviteUsers.Add(invite);
                    await context.SaveChangesAsync();


                    //Enable to production
                    //EmailService emailService = new EmailService();
                    //await emailService.SendEmailAsync(model.Email, "Invide to Nekotik site", "Invite: "
                    //    + $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Account/Register/"
                    //    + invite.InviteCode);

                    return RedirectToAction("Index");
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
        string GenRandomString(string Alphabet, int Length)
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder(Length - 1);
            int Position = 0;
            for (int i = 0; i < Length; i++)
            {
                Position = rnd.Next(0, Alphabet.Length - 1);
                sb.Append(Alphabet[Position]);
            }
            return sb.ToString();

        }
        public async Task<IActionResult> Edit(string id)
        {
            Account user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Account user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user != null)
                {
                    user.Email = model.Email;

 
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            Account user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}
