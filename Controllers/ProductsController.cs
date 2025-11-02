using Microsoft.AspNetCore.Mvc;
using GestionBoutiqueElevate.Services;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _repo;

        public ProductsController(IProductRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> Index(string? q)
        {
            ViewData["Query"] = q ?? "";
            var data = await _repo.SearchAsync(q);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create() => View(new Product { IsActive = true });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model)
        {
            if (!ModelState.IsValid) return View(model);
            await _repo.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product model)
        {
            if (!ModelState.IsValid) return View(model);
            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
