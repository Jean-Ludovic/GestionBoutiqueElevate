using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByCodeAsync(string code);
                    Task<IEnumerable<Employee>> GetAllAsync();
                    Task<Employee> AddAsync(Employee e);
        Task<string> GenerateCodeAsync();
    }
}
