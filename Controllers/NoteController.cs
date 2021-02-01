using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.Controllers
{
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
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Note note)
        {
            if (ModelState.IsValid)
            {
                var user =await userManager.GetUserAsync(User);
                note.AccountId = user.Id;
                note.DateTime=DateTime.Now;
                await context.Notes.AddAsync(note);
                await context.SaveChangesAsync();
               return RedirectToAction("Index");
            }

            return View(note);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id != null)
            {
                var note =await context.Notes.FirstOrDefaultAsync(p => p.Id == id);
                if (note != null) return View(note);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Note note)
        {
            if (ModelState.IsValid)
            {
                context.Notes.Update(note);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(note);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id != null)
            {
                var note = await context.Notes.FirstOrDefaultAsync(p => p.Id == id);
                if (note != null) return View(note);
            }

            return NotFound();
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(Guid? id)
        {
            if (id != null)
            {
                var note =await context.Notes.FirstOrDefaultAsync(p => p.Id == id);
                if (note != null)
                {
                    context.Notes.Remove(note);
                    await context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return  NotFound();

        }
    }
}
