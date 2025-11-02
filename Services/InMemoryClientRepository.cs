using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
    public class InMemoryClientRepository : IClientRepository
    {
        private readonly List<Client> _clients = new();

        public InMemoryClientRepository()
        {
            // Données de démo (modifie à volonté)
            _clients.AddRange(new[]
            {
                new Client { Id = 1, FirstName = "Sarah", LastName = "Dubois", Email="sarah@example.com", Phone="514-555-0101", City="Montréal", Address="123 Rue Sainte-Catherine" },
                new Client { Id = 2, FirstName = "Jean", LastName = "Nguyen", Email="jean.nguyen@example.com", Phone="438-555-9922", City="Laval", Address="55 Boul. des Prairies" },
                new Client { Id = 3, FirstName = "Aïcha", LastName = "Traoré", Email="aicha.t@example.com", Phone="581-555-7766", City="Québec", Address="9 Rue Saint-Jean" },
                new Client { Id = 4, FirstName = "Marco", LastName = "Rossi", Email="m.rossi@example.com", Phone="450-555-3344", City="Longueuil", Address="221 Chemin Chambly" },
            });
        }

        public Task<IEnumerable<Client>> GetAllAsync() =>
            Task.FromResult(_clients.AsEnumerable());

        public Task<Client?> GetByIdAsync(int id) =>
            Task.FromResult(_clients.FirstOrDefault(c => c.Id == id));

        public Task AddAsync(Client client)
        {
            client.Id = _clients.Count == 0 ? 1 : _clients.Max(c => c.Id) + 1;
            _clients.Add(client);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Client>> SearchAsync(string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Task.FromResult(_clients.AsEnumerable());

            query = query.Trim().ToLowerInvariant();

            var results = _clients.Where(c =>
                   (c.FirstName + " " + c.LastName).ToLowerInvariant().Contains(query)
                || c.Email.ToLowerInvariant().Contains(query)
                || c.Phone.ToLowerInvariant().Contains(query)
                || c.City.ToLowerInvariant().Contains(query));

            return Task.FromResult(results);
        }
        public Task UpdateAsync(Client client)
        {
            var i = _clients.FindIndex(c => c.Id == client.Id);
            if (i >= 0) _clients[i] = client;
            return Task.CompletedTask;
        }

    }
}
