namespace GestionBoutiqueElevate.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; } = "";          // Code produit
        public string Name { get; set; } = "";
        public string? Category { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }              // Prix unitaire
        public int Stock { get; set; }                  // Quantité en stock
        public bool IsActive { get; set; } = true;

        public bool IsLowStock(int threshold = 5) => Stock <= threshold;
        public decimal InventoryValue => Price * Stock;
    }
}
