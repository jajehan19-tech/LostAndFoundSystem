using LostAndFoundSystem.Data;
using LostAndFoundSystem.Interfaces;
using LostAndFoundSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LostAndFoundSystem.Controllers
{
    public class ItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ItemController(IUnitOfWork unitOfWork, AppDbContext context, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var items = await _context.Items
                .Include(i => i.User)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Items
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.ItemId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "UserName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Item item, IFormFile? PhotoFile)
        {
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PhotoFile.CopyToAsync(stream);
                }
                item.PhotoPath = "/uploads/" + fileName;
            }

            item.DateReported = DateTime.Now;
            item.UserId = HttpContext.Session.GetInt32("UserId") ?? item.UserId;
            ModelState.Clear();

            await _unitOfWork.Items.AddAsync(item);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "UserId", "UserName", item.UserId);
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Item item, IFormFile? PhotoFile)
        {
            var existing = await _context.Items.FindAsync(item.ItemId);
            if (existing == null) return NotFound();

            existing.ItemName = item.ItemName;
            existing.Description = item.Description;
            existing.Category = item.Category;
            existing.Location = item.Location;
            existing.Status = item.Status;
            existing.UserId = item.UserId;

            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PhotoFile.CopyToAsync(stream);
                }
                existing.PhotoPath = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var item = await _context.Items
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                // Delete photo file from server if exists
                if (!string.IsNullOrEmpty(item.PhotoPath))
                {
                    var fullPath = Path.Combine(_env.WebRootPath,
                        item.PhotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }

                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}