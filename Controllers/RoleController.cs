using LostAndFoundSystem.Interfaces;
using LostAndFoundSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFoundSystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            return View(roles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Roles.AddAsync(role);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Roles.Update(role);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role != null)
            {
                _unitOfWork.Roles.Delete(role);
                await _unitOfWork.SaveAsync();
            }
            return RedirectToAction("Index");
        }
    }
}