namespace GestionBoutiqueElevate.Models
{
    public class Client
    {

        public int Id { get; set; }

        // Identity de base
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";

        // Optionnel pour affichage
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Helper calculé
        public string FullName => $"{FirstName} {LastName}".Trim();

        public decimal Credit { get; set; } = 0m;
    }
}
