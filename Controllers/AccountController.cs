using LostAndFoundSystem.Data;
using LostAndFoundSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LostAndFoundSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // ─── LOGIN ───────────────────────────────────────────
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("UserRole", user.Role?.RoleName ?? "User");

                    user.LastLogIn = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError("", "Invalid email or password");
            }
            return View(model);
        }

        // ─── LOGOUT ──────────────────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ─── REGISTER ────────────────────────────────────────
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string Password)
        {
            // Check if email already exists
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existing != null)
            {
                ViewBag.Error = "Email already registered.";
                return View(user);
            }

            // Always register as User (RoleId = 2)
            // Admin can never be created from register page
            var userRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "User");

            if (userRole == null)
            {
                ViewBag.Error = "User role not found. Contact admin.";
                return View(user);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
            user.LastLogIn = DateTime.Now;
            user.RoleId = userRole.Id;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        // ─── PROFILE ─────────────────────────────────────────
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToAction("Login");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(User updatedUser, string? NewPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToAction("Login");

            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;
            user.Address = updatedUser.Address;

            if (!string.IsNullOrEmpty(NewPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("UserName", user.UserName);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }
    }
}