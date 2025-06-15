using System.Diagnostics;

namespace DofusGroupFinder.Client.Services
{
    public enum UserStatus
    {
        Offline,
        Available,
        InGroup
    }

    public class StatusService
    {
        public UserStatus CurrentStatus { get; private set; } = UserStatus.Offline;

        private readonly string _dofusProcessName = "Dofus"; // ⚠ à adapter selon le nom de ton process

        private bool _hasGroup = false;

        public void SetGroupState(bool hasGroup)
        {
            _hasGroup = hasGroup;
            UpdateStatus();
        }

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
    }
}
