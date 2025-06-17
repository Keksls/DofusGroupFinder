using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.Entities;
using System.Diagnostics;

namespace DofusGroupFinder.Client.Services
{
    public enum UserStatus
    {
        Offline,
        Available,
        InGroup
    }

    public class GroupManagerService
    {
        private readonly string _dofusProcessName = "Dofus";
        private bool _hasGroup = false;

        public UserStatus CurrentStatus { get; private set; } = UserStatus.Offline;
        public Guid? CurrentListingId { get; private set; }
        public List<Character> GroupMembers { get; private set; } = new();

        public GroupManagerService()
        {
        }

        // Gestion du process de Dofus (détection jeu lancé)
        public void CheckGameRunning()
        {
            var processes = Process.GetProcessesByName(_dofusProcessName);
            if (processes.Length == 0)
            {
                CurrentStatus = UserStatus.Offline;
            }
            else
            {
                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            if (_hasGroup)
                CurrentStatus = UserStatus.InGroup;
            else
                CurrentStatus = UserStatus.Available;
        }

        public void SetGroupState(bool hasGroup)
        {
            _hasGroup = hasGroup;
            UpdateStatus();
        }

        // Gestion du groupe (API)

        public async Task CreateGroupAsync(Guid listingId)
        {
            // Vérifie qu'on possède bien la listing
            var myListings = await App.ApiClient.GetMyListingsAsync();
            var listing = myListings?.FirstOrDefault(l => l.Id == listingId);
            if (listing == null)
                throw new Exception("You don't own this listing");

            // Ajoute tous ses personnages automatiquement dans le groupe
            foreach (var c in listing.Characters)
            {
                GroupMemberRequest groupMemberRequest = new GroupMemberRequest
                {
                    CharacterId = c.CharacterId,
                    Name = c.Name,
                    Class = c.Class,
                    Level = c.Level,
                    Role = c.Role
                };
                await App.ApiClient.AddGroupMemberAsync(listingId, groupMemberRequest);
            }

            CurrentListingId = listingId;
            await RefreshGroupAsync();
        }

        public async Task RefreshGroupAsync()
        {
            if (CurrentListingId == null) return;

            GroupMembers = await App.ApiClient.GetGroupMembersAsync(CurrentListingId.Value) ?? new();
            SetGroupState(true);
            App.Events.InvokeGroupStateChanged();
        }

        public async Task AddMemberAsync(Guid listingId, GroupMemberRequest request)
        {
            await App.ApiClient.AddGroupMemberAsync(listingId, request);
            await RefreshGroupAsync();
        }

        public async Task RemoveMemberAsync(Guid listingId, Guid characterId)
        {
            await App.ApiClient.RemoveGroupMemberAsync(listingId, characterId);
            await RefreshGroupAsync();
        }

        public async Task DisbandGroupAsync(bool deleteListing)
        {
            if (CurrentListingId == null)
                return;

            try
            {
                // Appel serveur pour disband proprement
                await App.ApiClient.DisbandGroupAsync(CurrentListingId.Value, deleteListing);
                if (deleteListing)
                    App.Events.InvokeUserListingsUpdated();
            }
            catch (Exception ex)
            {
                // Log optionnel
                Console.WriteLine("Error while disbanding group: " + ex.Message);
            }

            CurrentListingId = null;
            GroupMembers.Clear();
            SetGroupState(false);
            App.Events.InvokeGroupStateChanged();
        }

        public async Task RestoreGroupAsync()
        {
            var myListings = await App.ApiClient.GetMyListingsAsync();
            if (myListings == null) return;

            foreach (var listing in myListings)
            {
                if (listing.GroupMembers.Any())
                {
                    // On prend le premier groupe trouvé (tu peux complexifier si besoin)
                    CurrentListingId = listing.Id;
                    GroupMembers = listing.GroupMembers
                        .Select(g => new Character
                        {
                            Id = g.CharacterId ?? Guid.Empty,
                            Name = g.Name,
                            Level = g.Level,
                            Class = g.Class,
                            Role = g.Role
                        })
                        .ToList();
                    SetGroupState(true);
                    App.Events.InvokeGroupStateChanged();
                    return;
                }
            }

            // Si aucun groupe
            CurrentListingId = null;
            GroupMembers.Clear();
            SetGroupState(false);
            App.Events.InvokeGroupStateChanged();
        }
    }
}