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
        //public async Task<IActionResult> Index() => View(await _context.Files.ToListAsync());
        [HttpPost]
        public async Task<IActionResult> AddFiles(IFormFileCollection uploads,FileModel.Type type, Guid guid)
        {
            foreach (var uploadedFile in uploads)
            {
                string path = "/files/" + uploadedFile.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                FileModel file = new FileModel { Name = uploadedFile.FileName,TypeTo = type,Guid = guid, Path = path, Extensions = uploadedFile.FileName.Split(new char[] { '.' })[1] };
                await _context.Files.AddAsync(file);
            }
            await _context.SaveChangesAsync();

            return Ok();
        }
        //[HttpGet]
        //public async Task<IActionResult> ShowFile(Guid id)
        //{       
        //    FileModel result =await _context.Files.Where(op => op.Id == id).FirstOrDefaultAsync();
        //    if (result != null)
        //        return View(result);
        //    else
        //        return NotFound();
        //}

        //private async Task<IActionResult> GetFile(Guid id)
        //{
        //    FileModel file = await _context.Files.Where(op => op.Id == id).FirstOrDefaultAsync();
        //    if (file == null)
        //        return NotFound();
        //    string path = _appEnvironment.WebRootPath + file.Path;
           
        //    return PhysicalFile(path, "application/" + file.Extensions, file.Name);
        //}

    }
}
