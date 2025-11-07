using Microsoft.AspNetCore.Mvc;

namespace GestionBoutiqueElevate.Controllers
{
    public class RefundsController : Controller
    {
        [HttpGet]
        public IActionResult Create() => View();
    }
}
