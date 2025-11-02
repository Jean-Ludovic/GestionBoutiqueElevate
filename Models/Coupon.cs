namespace GestionBoutiqueElevate.Models
{
	public class Coupon
	{
		public string Code { get; set; } = "";
		public decimal PercentOff { get; set; } // 0.10 => 10%
		public decimal AmountOff { get; set; }  // remise fixe $
		public bool IsActive { get; set; } = true;
	}
}
