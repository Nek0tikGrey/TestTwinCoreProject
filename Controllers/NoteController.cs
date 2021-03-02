using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.Controllers
{
    [Authorize(Roles ="User")]
    public class NoteController : Controller
    {
        private readonly UserManager<Account> userManager;
        private readonly TwinCoreDbContext context;
        public NoteController(UserManager<Account> userManager,TwinCoreDbContext context)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            var data = context.Notes.Where(p => p.AccountId == user.Id);
            return View(await data.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(Guid? id)
        {
            if (id == null)
                return View(new Note());
            else
            {
                var note = await context.Notes.FindAsync(id);
                if (note != null) return View(note);
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Guid? id, Note note)
        {
            if (ModelState.IsValid)
            {
                if (id == Guid.Empty)
                {
                    note.DateTime=DateTime.Now;
                    var user = await userManager.GetUserAsync(User);
                    note.AccountId = user.Id;

                    await context.Notes.AddAsync(note);
                    await context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        var user = await userManager.GetUserAsync(User);
                        note.AccountId = user.Id;
                        context.Notes.Update(note);
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TransactionModelExists(note.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                   
                }

                return Json(new
                {
                    IsValid = true,
                    html = Helper.RenderRazorViewToString(this, "_ViewAll", await 
                        context.Notes.Where( p => p.AccountId == userManager.GetUserAsync(User).Result.Id).ToListAsync())
                });
            }
            return Json(new { IsValid = false, html = Helper.RenderRazorViewToString(this,"CreateOrEdit",note) });
        }
        private bool TransactionModelExists(Guid id)
        {
            return context.Notes.Any(e => e.Id == id);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id != Guid.Empty)
            {
                var note =await context.Notes.FindAsync(id);
                if (note != null)
                {
                    context.Notes.Remove(note);
                    await context.SaveChangesAsync();
                    return Json(new
                    {
                        IsValid = true,
                        html = Helper.RenderRazorViewToString(this, "_ViewAll", await
                            context.Notes.Where(p => p.AccountId == userManager.GetUserAsync(User).Result.Id).ToListAsync())
                    });
                }
            }
            return  NotFound();

        }
    }
}
