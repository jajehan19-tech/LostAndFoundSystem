using LostAndFoundSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LostAndFoundSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.TotalItems = await _context.Items.CountAsync();
            ViewBag.LostItems = await _context.Items.CountAsync(i => i.Category == "Lost");
            ViewBag.FoundItems = await _context.Items.CountAsync(i => i.Category == "Found");
            ViewBag.PendingItems = await _context.Items.CountAsync(i => i.Status == "Pending");

            return View();
        }
    }
}