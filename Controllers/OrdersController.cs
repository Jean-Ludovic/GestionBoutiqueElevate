using Microsoft.AspNetCore.Mvc;
using GestionBoutiqueElevate.Services;
using GestionBoutiqueElevate.Models;
using System.Text.RegularExpressions;


namespace GestionBoutiqueElevate.Controllers
{
	public class OrdersController : Controller
	{
		private readonly IOrderRepository _orders;
		private readonly IClientRepository _clients;
		private readonly IProductRepository _products;
		private readonly ICouponService _coupons;
        private readonly IEmployeeRepository _employees;

        public OrdersController(IOrderRepository orders, IClientRepository clients, IProductRepository products, ICouponService coupons,
            IEmployeeRepository employees)
		{
			_orders = orders; _clients = clients; _products = products; _coupons = coupons;
            _employees = employees;
        }

		[HttpGet]
		public IActionResult Create()
		{
			// Page vide avec client STORE CUSTOMER
			var model = new Order();
			return View(model);
		}

        // ---- Endpoints AJAX pour la page Create ----

        [HttpGet]
        public async Task<IActionResult> SearchClients(string q)
        {
            var items = await _clients.SearchAsync(q);
            var res = items.Select(c => new {
                id = c.Id,
                name = $"{c.FirstName} {c.LastName}",
                email = c.Email,
                credit = c.Credit
            });
            return Json(res);
        }


        [HttpGet]
		public async Task<IActionResult> SearchProducts(string q)
		{
			var items = await _products.SearchAsync(q);
			var res = items.Select(p => new { id = p.Id, sku = p.Sku, name = p.Name, price = p.Price, stock = p.Stock });
			return Json(res);
		}

		[HttpPost]
		public async Task<IActionResult> ValidateCoupon([FromForm] string code, [FromForm] decimal subtotal)
		{
			if (string.IsNullOrWhiteSpace(code)) return Json(new { ok = false, message = "Code vide." });
			var coupon = await _coupons.GetByCodeAsync(code);
			if (coupon == null) return Json(new { ok = false, message = "Coupon invalide." });
			var discount = _coupons.ComputeDiscount(coupon, subtotal);
			return Json(new { ok = true, discount });
		}

		// Sauvegarde finale de la commande
		
		[ValidateAntiForgeryToken]
        public class CreateOrderDto
        {
            public int? ClientId { get; set; }
            public string? ClientName { get; set; }
            public List<OrderItem> Items { get; set; } = new();
            public string? CouponCode { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal TaxRate { get; set; }
            public string EmployeeCode { get; set; } = "";
            public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        }

        [HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			var o = await _orders.GetAsync(id);
			if (o == null) return NotFound();
			return View(o);
		}
        [HttpPost]
        public async Task<IActionResult> ValidateEmployee([FromForm] string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return Json(new { ok = false, message = "Code requis." });
            var m = Regex.Match(code.Trim().ToUpperInvariant(), "^[A-Z]{2}-\\d{2}$");
            if (!m.Success) return Json(new { ok = false, message = "Format invalide (ex: XX-00)." });
            var emp = await _employees.GetByCodeAsync(code);
            if (emp == null) return Json(new { ok = false, message = "Code inconnu." });
            return Json(new { ok = true, id = emp.Id, code = emp.Code, name = emp.FullName, role = emp.Role.ToString() });
        }

    }
}
