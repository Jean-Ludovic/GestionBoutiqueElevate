using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionBoutiqueElevate.Models
{
    public enum PaymentMethod { Cash, Credit }
    public class Order
	{
		public int Id { get; set; }
		public int? ClientId { get; set; } // null => STORE CUSTOMER
		public string ClientName { get; set; } = "STORE CUSTOMER";
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public List<OrderItem> Items { get; set; } = new();
		public string? CouponCode { get; set; }
		public decimal TaxRate { get; set; } = 0.15m; // 15% par défaut

		public decimal Subtotal => Items.Sum(i => i.LineTotal);
		public decimal DiscountAmount { get; set; } // fixé après validation coupon
		public decimal Tax => Math.Round((Subtotal - DiscountAmount) * TaxRate, 2);
		public decimal Total => Math.Max(0, Math.Round(Subtotal - DiscountAmount + Tax, 2));

        public int? EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public decimal CreditUsed { get; set; }
        public decimal CashPaid { get; set; }
    }

	public class OrderItem
	{
		public int ProductId { get; set; }
		public string Sku { get; set; } = "";
		public string Name { get; set; } = "";
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal LineTotal => Math.Round(UnitPrice * Quantity, 2);
	}
}
