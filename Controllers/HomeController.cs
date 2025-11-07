using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GestionBoutiqueElevate.Services;
using GestionBoutiqueElevate.Models;
using System.Diagnostics;

namespace GestionBoutiqueElevate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAnnouncementRepository _ann;

        public HomeController(ILogger<HomeController> logger, IAnnouncementRepository ann)
        {
            _logger = logger;
            _ann = ann;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Splash() => View();

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var promos = await _ann.GetActiveAsync();
            return View(promos);
        }

        [HttpGet]
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
