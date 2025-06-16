using DofusGroupFinder.Domain.DTO;
using System.Collections.Concurrent;

namespace DofusGroupFinder.Application.Services
{
    public class PresenceService : IPresenceService
    {
        private readonly ConcurrentDictionary<Guid, PresenceInfo> _presences = new();

        public void Connect(Guid accountId)
        {
            _presences[accountId] = new PresenceInfo
            {
                AccountId = accountId,
                IsConnected = true,
                LastPing = DateTime.UtcNow
            };
        }

        public void Disconnect(Guid accountId)
        {
            _presences.TryRemove(accountId, out _);
        }

        public void Ping(Guid accountId, bool? isInGame = null, bool? isInGroup = null)
        {
            if (_presences.TryGetValue(accountId, out var presence))
            {
                if (isInGame.HasValue) presence.IsInGame = isInGame.Value;
                if (isInGroup.HasValue) presence.IsInGroup = isInGroup.Value;
            }
        }

        public bool IsAvailable(Guid accountId)
        {
            if (_presences.TryGetValue(accountId, out var presence))
            {
                return presence.IsConnected && !presence.IsInGroup;
            }
            return false;
        }

        public PresenceInfo? GetPresence(Guid accountId)
        {
            _presences.TryGetValue(accountId, out var presence);
            return presence;
        }

        public void Cleanup()
        {
            var threshold = DateTime.UtcNow.AddSeconds(-60);
            foreach (var entry in _presences)
            {
                if (entry.Value.LastPing < threshold)
                {
                    _presences.TryRemove(entry.Key, out _);
                }
            }
        }

        public void SetGroup(Guid accountId, Guid? groupId)
        {
            if (!_presences.ContainsKey(accountId)) return;
            _presences[accountId].CurrentGroupId = groupId;
        }
    }
}