using UnityEngine;
using System;

namespace EasyMobile
{
    public partial class GameServicesSettings
    {
        public bool IsSavedGamesEnabled { get { return _enableSavedGames; } }

        public SavedGameConflictResolutionStrategy AutoConflictResolutionStrategy { get { return _autoConflictResolutionStrategy; } set { _autoConflictResolutionStrategy = value; } }

        public GPGSSavedGameDataSource GPGSDataSource { get { return _gpgsDataSource; } set { _gpgsDataSource = value; } }

        [SerializeField]
        private bool _enableSavedGames = false;
        [SerializeField]
        private SavedGameConflictResolutionStrategy _autoConflictResolutionStrategy = SavedGameConflictResolutionStrategy.UseBase;
        [SerializeField]
        private GPGSSavedGameDataSource _gpgsDataSource = GPGSSavedGameDataSource.ReadCacheOrNetwork;
    }
}

