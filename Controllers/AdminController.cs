using LostAndFoundSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LostAndFoundSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Not logged in → go to Login
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            // Logged in but not Admin → go to Dashboard silently
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Dashboard");

            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalRoles = await _context.Roles.CountAsync();
            ViewBag.TotalItems = await _context.Items.CountAsync();

            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();

            return View(users);

        }
    }

}