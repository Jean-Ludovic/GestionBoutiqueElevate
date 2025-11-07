using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
    public class InMemoryAnnouncementRepository : IAnnouncementRepository
    {
        private readonly List<Announcement> _items = new()
        {
            new Announcement{ Id=1, Title="Promo d’ouverture", Message="-10% sur toutes les commandes aujourd’hui.", IsActive=true },
            new Announcement{ Id=2, Title="Info", Message="Comptoir fermé à 19h ce soir.", IsActive=true }
        };

        public Task<IEnumerable<Announcement>> GetActiveAsync() =>
            Task.FromResult(_items.Where(x => x.IsActive).OrderByDescending(x => x.PublishedAt).AsEnumerable());

        public Task<IEnumerable<Announcement>> GetAllAsync() =>
            Task.FromResult(_items.OrderByDescending(x => x.PublishedAt).AsEnumerable());

        public Task<Announcement> AddAsync(Announcement a)
        {
            a.Id = _items.Count == 0 ? 1 : _items.Max(x => x.Id) + 1;
            a.PublishedAt = DateTime.UtcNow;
            _items.Add(a);
            return Task.FromResult(a);
        }

        public Task ToggleActiveAsync(int id)
        {
            var i = _items.FirstOrDefault(x => x.Id == id);
            if (i != null) i.IsActive = !i.IsActive;
            return Task.CompletedTask;
        }
    }
}
