using LostAndFoundSystem.Data;
using LostAndFoundSystem.Interfaces;
using LostAndFoundSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LostAndFoundSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public UserController(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();
            return View(user);
        }

        public async Task<IActionResult> Create()
        {
            var roles = await _context.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "RoleName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user, string Password)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
                user.LastLogIn = DateTime.Now;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var roles = await _context.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "RoleName");
            return View(user);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            var roles = await _context.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "RoleName", user.RoleId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var roles = await _context.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "RoleName", user.RoleId);
            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user != null)
            {
                if (user.Role?.RoleName == "Admin")
                {
                    TempData["Error"] = "Admin user cannot be deleted!";
                    return RedirectToAction("Index", "Admin");
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Admin");
        }
    }
}