using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
	public interface ICouponService
	{
		Task<Coupon?> GetByCodeAsync(string code);
		decimal ComputeDiscount(Coupon coupon, decimal subtotal);
	}
}
