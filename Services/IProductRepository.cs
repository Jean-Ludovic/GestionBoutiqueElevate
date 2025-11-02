using System.Collections.Generic;
using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> SearchAsync(string? query);
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product p);
        Task UpdateAsync(Product p);
        Task DeleteAsync(int id);  // <== ajouter ceci
    }
}
