using TasteOfHome.Models;

namespace TasteOfHome.Services
{
    public interface ILiveEventsService
    {
        Task<List<LiveEvent>> SearchEventsAsync(
            string? keyword,
            string? city,
            string? category,
            double? latitude,
            double? longitude,
            int size = 20,
            CancellationToken cancellationToken = default);
    }
}