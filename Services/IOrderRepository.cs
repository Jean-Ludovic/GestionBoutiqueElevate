using System.Collections.Generic;
using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
	public interface IOrderRepository
	{
		Task<int> CreateAsync(Order order);
		Task<Order?> GetAsync(int id);
		Task<IEnumerable<Order>> GetAllAsync();
	}
}
