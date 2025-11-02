using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public Task<IEnumerable<Product>> GetAllAsync() =>
            Task.FromResult(_products.AsEnumerable());

        public Task<IEnumerable<Product>> SearchAsync(string? query)
        {
            var q = (query ?? "").Trim().ToUpperInvariant();
            var r = _products.Where(p =>
                string.IsNullOrEmpty(q) ||
                p.Name.ToUpperInvariant().Contains(q) ||
                p.Sku.ToUpperInvariant().Contains(q));
            return Task.FromResult(r.AsEnumerable());
        }

        public Task<Product?> GetByIdAsync(int id) =>
            Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task AddAsync(Product p)
        {
            p.Id = _products.Count == 0 ? 1 : _products.Max(x => x.Id) + 1;
            _products.Add(p);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Product p)
        {
            var i = _products.FindIndex(x => x.Id == p.Id);
            if (i >= 0) _products[i] = p;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)   // <== implémentation
        {
            var i = _products.FindIndex(x => x.Id == id);
            if (i >= 0) _products.RemoveAt(i);
            return Task.CompletedTask;
        }
    }
}
