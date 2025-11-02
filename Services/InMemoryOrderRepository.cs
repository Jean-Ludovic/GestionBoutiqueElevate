using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
	public class InMemoryOrderRepository : IOrderRepository
	{
		private readonly List<Order> _orders = new();
		public Task<int> CreateAsync(Order order)
		{
			order.Id = _orders.Count == 0 ? 1 : _orders.Max(o => o.Id) + 1;
			_orders.Add(order);
			return Task.FromResult(order.Id);
		}

		public Task<Order?> GetAsync(int id) =>
			Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));

		public Task<IEnumerable<Order>> GetAllAsync() =>
			Task.FromResult(_orders.AsEnumerable());
	}
}
