using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GestionBoutiqueElevate.Services;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _repo;

        public ClientsController(IClientRepository repo)
        {
            _repo = repo;
        }

        // /Clients?q=jean
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
            var client = await _repo.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpGet]
        public IActionResult Create() => View(new Client());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client model)
        {
            if (!ModelState.IsValid) return View(model);
            await _repo.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }
    }
}
