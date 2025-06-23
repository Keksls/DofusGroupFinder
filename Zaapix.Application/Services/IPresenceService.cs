using Zaapix.Domain.DTO;

namespace Zaapix.Application.Services
{
    public interface IPresenceService
    {
        void Connect(Guid accountId);
        void Disconnect(Guid accountId);
        void Ping(Guid accountId, bool? isInGame = null, bool? isInGroup = null);
        bool IsAvailable(Guid accountId);
        PresenceInfo? GetPresence(Guid accountId);
        void Cleanup();
    }
}