﻿using System;

namespace Zaapix.Client.Utils
{
    public class Events
    {
        // Chargement des données statiques (Dungeons etc.)
        public event Action? OnGetStaticData;

        // Listings de l'utilisateur mis à jour
        public event Action? OnUserListingsUpdated;

        // Server  mis à jour
        public event Action? OnServerUpdated;

        // Personnages mis à jour
        public event Action? OnCharactersUpdated;

        // Changement d'état de groupe (création, ajout, leave etc.)
        public event Action? OnGroupStateChanged;

        // Méthodes d'invocation
        public void InvokeGetStaticData()
        {
            OnGetStaticData?.Invoke();
        }

        public void InvokeServerUpdated()
        {
            OnServerUpdated?.Invoke();
        }

        public void InvokeUserListingsUpdated()
        {
            OnUserListingsUpdated?.Invoke();
        }

        public void InvokeCharactersUpdated()
        {
            OnCharactersUpdated?.Invoke();
        }

        public void InvokeGroupStateChanged()
        {
            OnGroupStateChanged?.Invoke();
        }
    }
}