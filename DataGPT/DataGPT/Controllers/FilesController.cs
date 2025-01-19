using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DataGPT.Models;
using System.Security.Claims;
namespace Test.Controllers
{
    public class FilesController : Controller
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public FilesController()
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        private string GetCurrentUserId()
        {
            // Replace with your actual user ID retrieval logic
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private string GetSanitizedUserId(string userId)
        {
            // Replace invalid characters with underscores or use a hash function
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                userId = userId.Replace(c, '_');
            }
            return userId;
        }

        private string GetUserDirectory()
        {
            var userId = GetCurrentUserId();
            var sanitizedUserId = GetSanitizedUserId(userId);
            var userDirectory = Path.Combine(_storagePath, sanitizedUserId);

            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            return userDirectory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userDirectory = GetUserDirectory();
            var files = Directory.GetFiles(userDirectory)
                .Select(f => new UserFile
                {
                    FileName = Path.GetFileName(f),
                    FilePath = f,
                    UploadDate = System.IO.File.GetCreationTime(f),
                    UserId = GetCurrentUserId()
                })
                .ToList();

            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var userDirectory = GetUserDirectory();
                var filePath = Path.Combine(userDirectory, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return RedirectToAction("Index");
            }

            return View("Index");
        }

        [HttpPost]
        public IActionResult Delete(string fileName)
        {
            var userDirectory = GetUserDirectory();
            var filePath = Path.Combine(userDirectory, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Download(string fileName)
        {
            var userDirectory = GetUserDirectory();
            var filePath = Path.Combine(userDirectory, fileName);
            if (System.IO.File.Exists(filePath))
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
            }

            return RedirectToAction("Index");
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
        {
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.ms-word"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".csv", "text/csv"}
        };
        }
    }

}
