using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestTwinCoreProject.Models;
using TestTwinCoreProject.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using TestTwinCoreProject.Utility.CryptoInfrastructure;

namespace TestTwinCoreProject.Controllers
{
    [Authorize(Roles ="User")]
    public class NoteController : Controller
    {
        private readonly UserManager<Account> userManager;
        private readonly TwinCoreDbContext context;
        private readonly IWebHostEnvironment environment;
        private ICryptoService cryptoService;
        public NoteController(UserManager<Account> userManager,TwinCoreDbContext context,IWebHostEnvironment environment, ICryptoService cryptoService)
        {
            this.cryptoService = cryptoService;
            this.environment = environment;
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index(string theme, DateTime dateFrom,DateTime dateTo,int page=1, SortState sortOrder=SortState.ThemeAsc)
        {
            var account = await userManager.GetUserAsync(User);

            int pageSize = 5;
            IQueryable<Note> notes = context.Notes.Where(p => p.AccountId == account.Id);
            
            if (!String.IsNullOrEmpty(theme))
            {
                notes = notes.Where(predicate => predicate.Title.Contains(theme));
            }

            if (dateFrom >DateTime.MinValue && dateTo >DateTime.MinValue)
                notes = notes.Where(p => p.DateTime <= dateTo && p.DateTime >= dateFrom);
            else if(dateTo!>DateTime.MinValue)
                notes = notes.Where(p => p.DateTime <= dateTo);
            else if(dateFrom>DateTime.MinValue)
                notes = notes.Where(p => p.DateTime >= dateFrom);           

            var count = await notes.CountAsync();

            switch (sortOrder)
            {
                case SortState.ThemeDesc:
                    notes = notes.OrderByDescending(s => s.Title);
                    break;
                case SortState.DateAsc:
                    notes = notes.OrderBy(s => s.DateTime);
                    break;
                case SortState.DateDesc:
                    notes = notes.OrderByDescending(s => s.DateTime);
                    break;
                default:
                    notes = notes.OrderBy(s => s.Title);
                    break;
            }

            var items = await notes.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            foreach (var t in items)
                cryptoService.Decrypt(t);

            NoteIndexViewModel viewModel = new NoteIndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(theme, dateFrom, dateTo),
                Notes = items
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(Guid? id)
        {
            if (id == null)
                return View(new Note());
            else
            {
                var note = await context.Notes.FindAsync(id);
                cryptoService.Decrypt(note);
                if (note != null) return View(note);
                else
                {
                    return NotFound();
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Guid? id, Note note, IFormFileCollection uploads)
        {
            if (ModelState.IsValid)
            {
                Account user;
                if (id == Guid.Empty)
                {
                    note.DateTime=DateTime.Now;
                    user = await userManager.GetUserAsync(User);
                    note.AccountId = user.Id;
                    cryptoService.Encrypt(note);
                    await context.Notes.AddAsync(note);
                    await context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        user = await userManager.GetUserAsync(User);
                        note.AccountId = user.Id;
                        cryptoService.Encrypt(note);
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

                if (uploads != null)
                {
                    foreach (var uploadedFile in uploads)
                    {
                        string path = "/files/" + uploadedFile.FileName;
                        using (var fileStream = new FileStream(environment.WebRootPath + path, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                        }
                        FileModel file = new FileModel { Name = uploadedFile.FileName, TypeTo = FileModel.Type.Note, Guid = note.Id, Path = path, Extensions = uploadedFile.FileName.Split(new char[] { '.' })[1] };
                        await context.Files.AddAsync(file);
                    }
                    await context.SaveChangesAsync();
                }

                int pageSize = 5;
                IQueryable<Note> notes = context.Notes.Where(p => p.AccountId == userManager.GetUserAsync(User).Result.Id);
                foreach (var t in notes)
                    cryptoService.Decrypt(t);
                notes = notes.OrderByDescending(s => s.Title);
                var count = await notes.CountAsync();
                var items = await notes.Skip((1 - 1) * pageSize).Take(pageSize).ToListAsync();
                NoteIndexViewModel viewModel = new NoteIndexViewModel
                {
                    PageViewModel = new PageViewModel(count, 1, pageSize),
                    SortViewModel = new SortViewModel(SortState.ThemeAsc),
                    FilterViewModel = new FilterViewModel("", DateTime.Now, DateTime.Now),
                    Notes = items
                };
                return Json(new
                {
                    IsValid = true,
                    html = Helper.RenderRazorViewToString(this, "_ViewAll", viewModel)
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
                    if ((DateTime.Now-note.DateTime).TotalDays >= 2)
                        return Forbid("Notes older than 2 days is forbiden");

                    context.Notes.Remove(note);
                    await context.SaveChangesAsync();

                    int pageSize = 5;
                    IQueryable<Note> notes = context.Notes.Where(p => p.AccountId == userManager.GetUserAsync(User).Result.Id);
                    notes = notes.OrderByDescending(s => s.Title);                        
                    var count = await notes.CountAsync();
                    var items = await notes.Skip((1 - 1) * pageSize).Take(pageSize).ToListAsync();
                    NoteIndexViewModel viewModel = new NoteIndexViewModel
                    {
                        PageViewModel = new PageViewModel(count, 1, pageSize),
                        SortViewModel = new SortViewModel(SortState.ThemeAsc),
                        FilterViewModel = new FilterViewModel("", DateTime.Now, DateTime.Now),
                        Notes = items
                    };

                    return Json(new
                    {
                        IsValid = true,
                        html = Helper.RenderRazorViewToString(this, "_ViewAll", viewModel)
                    });
                }
            }
            return  NotFound();

        }
    }
}
