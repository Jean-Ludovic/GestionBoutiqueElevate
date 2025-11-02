using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
	public class InMemoryCouponService : ICouponService
	{
		private readonly List<Coupon> _coupons = new()
		{
			new Coupon{ Code="PROMO10", PercentOff=0.10m, IsActive=true },
			new Coupon{ Code="SAVE5", AmountOff=5m, IsActive=true },
			new Coupon{ Code="VIP20", PercentOff=0.20m, IsActive=true },
		};

		public Task<Coupon?> GetByCodeAsync(string code) =>
			Task.FromResult(_coupons.FirstOrDefault(c => c.IsActive && c.Code.ToLower() == code.ToLower()));

		public decimal ComputeDiscount(Coupon coupon, decimal subtotal)
		{
			var d = coupon.AmountOff > 0 ? coupon.AmountOff : subtotal * coupon.PercentOff;
			if (d < 0) d = 0;
			return decimal.Round(d, 2);
		}
	}
}
