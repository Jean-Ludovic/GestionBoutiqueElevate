using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GestionBoutiqueElevate.Models;
using GestionBoutiqueElevate.Services;
using System.Threading.Tasks;

namespace GestionBoutiqueElevate.Controllers
{
    public class AdminController : Controller
    {
        private readonly IClientRepository _clients;
        private readonly IEmployeeRepository _employees;
        private readonly IOrderRepository _orders;
        private readonly IProductRepository _products;
        private readonly IInvoiceService _invoice;
        private readonly IAnnouncementRepository _announcements;
        private readonly ILogger<AdminController> _logger;

        // === UN SEUL CONSTRUCTEUR ===
        public AdminController(
            IClientRepository clients,
            IEmployeeRepository employees,
            IOrderRepository orders,
            IProductRepository products,
            IInvoiceService invoice,
            IAnnouncementRepository announcements,
            ILogger<AdminController> logger)
        {
            _clients = clients;
            _employees = employees;
            _orders = orders;
            _products = products;
            _invoice = invoice;
            _announcements = announcements;
            _logger = logger;
        }

        // ------- Annonces / Promotions -------
        [HttpGet]
        public async Task<IActionResult> Announcements()
        {
            var all = await _announcements.GetAllAsync();
            return View(all);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement([FromForm] string title, [FromForm] string message, [FromForm] string adminCode)
        {
            var adm = await _employees.GetByCodeAsync((adminCode ?? "").Trim().ToUpperInvariant());
            if (adm == null || adm.Role != EmployeeRole.Admin) return BadRequest("Code admin invalide.");
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message)) return BadRequest("Champs requis.");
            var a = await _announcements.AddAsync(new Announcement { Title = title.Trim(), Message = message.Trim(), IsActive = true });
            return Json(new { ok = true, id = a.Id });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAnnouncement([FromForm] int id, [FromForm] string adminCode)
        {
            var adm = await _employees.GetByCodeAsync((adminCode ?? "").Trim().ToUpperInvariant());
            if (adm == null || adm.Role != EmployeeRole.Admin) return BadRequest("Code admin invalide.");
            await _announcements.ToggleActiveAsync(id);
            return Json(new { ok = true });
        }

        // ------- Dashboard -------
        [HttpGet] public IActionResult Index() => View();

        // ------- Clients -------
        [HttpGet]
        public async Task<IActionResult> Clients()
        {
            var list = await _clients.GetAllAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> SetClientCredit([FromForm] int clientId, [FromForm] decimal amount, [FromForm] string adminCode)
        {
            var adm = await _employees.GetByCodeAsync((adminCode ?? "").Trim().ToUpperInvariant());
            if (adm == null || adm.Role != EmployeeRole.Admin) return BadRequest("Code admin invalide.");
            var cl = await _clients.GetByIdAsync(clientId);
            if (cl == null) return NotFound("Client introuvable.");
            cl.Credit = amount < 0 ? 0 : decimal.Round(amount, 2);
            await _clients.UpdateAsync(cl);
            return Json(new { ok = true, clientId = cl.Id, credit = cl.Credit });
        }

        // ------- Employés -------
        [HttpGet]
        public async Task<IActionResult> Employees()
        {
            var list = await _employees.GetAllAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromForm] string fullName, [FromForm] string adminCode)
        {
            var adm = await _employees.GetByCodeAsync((adminCode ?? "").Trim().ToUpperInvariant());
            if (adm == null || adm.Role != EmployeeRole.Admin) return BadRequest("Code admin invalide.");
            var code = await _employees.GenerateCodeAsync();
            var e = await _employees.AddAsync(new Employee { FullName = fullName.Trim(), Code = code, Role = EmployeeRole.Employee });
            return Json(new { ok = true, id = e.Id, code = e.Code, name = e.FullName });
        }

        // ------- Commandes / Factures -------
        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var list = await _orders.GetAllAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
            var pdf = await _invoice.RenderOrderPdfAsync(id);
            if (pdf == null || pdf.Length == 0) return NotFound();
            return File(pdf, "application/pdf", $"invoice_{id}.pdf");
        }

        // ------- Produits -------
        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var list = await _products.GetAllAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] string adminCode, [FromForm] string name, [FromForm] string sku, [FromForm] decimal price, [FromForm] int stock)
        {
            var adm = await _employees.GetByCodeAsync((adminCode ?? "").Trim().ToUpperInvariant());
            if (adm == null || adm.Role != EmployeeRole.Admin) return BadRequest("Code admin invalide.");
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(sku)) return BadRequest("Champs requis.");
            if (price < 0 || stock < 0) return BadRequest("Valeurs négatives interdites.");
            var p = new Product { Name = name.Trim(), Sku = sku.Trim().ToUpperInvariant(), Price = decimal.Round(price, 2), Stock = stock };
            await _products.AddAsync(p);
            return Json(new { ok = true, id = p.Id, name = p.Name, sku = p.Sku, price = p.Price, stock = p.Stock });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromForm] string adminCode, [FromForm] int id, [FromForm] string name, [FromForm] string sku, [FromForm] decimal price)
        {
            var adm = await _employees.GetByCodeAsync((adminCode ?? "").Trim().ToUpperInvariant());
            if (adm == null || adm.Role != EmployeeRole.Admin) return BadRequest("Code admin invalide.");
            var p = await _products.GetByIdAsync(id);
            if (p == null) return NotFound("Produit introuvable.");
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(sku)) return BadRequest("Champs requis.");
            if (price < 0) return BadRequest("Prix négatif interdit.");
            p.Name = name.Trim();
            p.Sku = sku.Trim().ToUpperInvariant();
            p.Price = decimal.Round(price, 2);
            await _products.UpdateAsync(p);
            return Json(new { ok = true, id = p.Id, name = p.Name, sku = p.Sku, price = p.Price, stock = p.Stock });
        }

        [HttpPost]
        public async Task<IActionResult> AdjustStock([FromForm] string adminCode, [FromForm] int id, [FromForm] int delta)
        {
            var adm = await _employees.GetByCodeAsync((adminCode ?? "").Trim().ToUpperInvariant());
            if (adm == null || adm.Role != EmployeeRole.Admin) return BadRequest("Code admin invalide.");
            var p = await _products.GetByIdAsync(id);
            if (p == null) return NotFound("Produit introuvable.");
            var s = p.Stock + delta;
            if (s < 0) return BadRequest("Stock ne peut pas devenir négatif.");
            p.Stock = s;
            await _products.UpdateAsync(p);
            return Json(new { ok = true, id = p.Id, stock = p.Stock });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct([FromForm] string adminCode, [FromForm] int id)
        {
            var adm = await _employees.GetByCodeAsync((adminCode ?? "").Trim().ToUpperInvariant());
            if (adm == null || adm.Role != EmployeeRole.Admin) return BadRequest("Code admin invalide.");
            var p = await _products.GetByIdAsync(id);
            if (p == null) return NotFound("Produit introuvable.");
            p.Name = p.Name + " (désactivé)";
            p.Price = 0;
            p.Stock = 0;
            await _products.UpdateAsync(p);
            return Json(new { ok = true, id = p.Id });
        }
    }
}
