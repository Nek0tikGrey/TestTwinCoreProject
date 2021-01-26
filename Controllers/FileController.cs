using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.Controllers
{

    public class FileController : Controller
    {
        TwinCoreDbContext _context;

        IWebHostEnvironment _appEnvironment;

        public FileController(TwinCoreDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _appEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index() => View(await _context.Files.ToListAsync());
        [HttpPost]
        public async Task<IActionResult> AddFiles(IFormFileCollection uploads)
        {
            foreach (var uploadedFile in uploads)
            {
                // путь к папке Files
                string path = "/Files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path, Extensions = uploadedFile.FileName.Split(new char[] { '.' })[1] };
                _context.Files.Add(file);
            }
            _context.SaveChanges();
           
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ShowFile(Guid id)
        {
         

            FileModel result = _context.Files.Where(op => op.Id == id).FirstOrDefault();
            if (result != null)
                return View(result);
            else
                return NotFound();
        }

        private async Task<IActionResult> GetFile(Guid id)
        {
            FileModel file = await _context.Files.Where(op => op.Id == id).FirstOrDefaultAsync();
            if (file == null)
                return NotFound();
            string path = _appEnvironment.WebRootPath + file.Path;
           
            return PhysicalFile(path, "application/" + file.Extensions, file.Name);
        }

    }
}
