using System.Collections.Generic;
using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> SearchAsync(string? query);
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task AddAsync(Client client);
      
        Task UpdateAsync(Client client);
    }
}
