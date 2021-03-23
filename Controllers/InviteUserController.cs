using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.Controllers
{
    [Authorize(Roles="Admin")]
    public class InviteUserController : Controller
    {
        private readonly TwinCoreDbContext _context;

        public InviteUserController(TwinCoreDbContext context)
        {
            _context = context;
        }

        // GET: InviteUser
        public async Task<IActionResult> Index()
        {
            return View(await _context.InviteUsers.ToListAsync());
        }
        // GET: InviteUser/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inviteUser = await _context.InviteUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inviteUser == null)
            {
                return NotFound();
            }

            return View(inviteUser);
        }

        // POST: InviteUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inviteUser = await _context.InviteUsers.FindAsync(id);
            _context.InviteUsers.Remove(inviteUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InviteUserExists(Guid id)
        {
            return _context.InviteUsers.Any(e => e.Id == id);
        }
    }
}
