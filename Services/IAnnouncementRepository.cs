using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
    public interface IAnnouncementRepository
    {
        Task<IEnumerable<Announcement>> GetActiveAsync();
        Task<IEnumerable<Announcement>> GetAllAsync();
        Task<Announcement> AddAsync(Announcement a);
        Task ToggleActiveAsync(int id);
    }
}
