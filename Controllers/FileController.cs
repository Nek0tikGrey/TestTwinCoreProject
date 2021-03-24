using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.Controllers
{
    [Authorize(Roles = "User")]
    public class FileController : Controller
    {
        TwinCoreDbContext _context;

        IWebHostEnvironment _appEnvironment;

        public FileController(TwinCoreDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _appEnvironment = webHostEnvironment;
        }
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


    }
}
