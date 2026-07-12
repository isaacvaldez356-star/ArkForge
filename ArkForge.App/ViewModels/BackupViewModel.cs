using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArkForge.Server;
using ArkForge.Common.Models;

namespace ArkForge.App.ViewModels
{
    public partial class BackupViewModel : ObservableObject
    {
        private readonly BackupService _backupService;

        public ObservableCollection<BackupInfo> Backups { get; } = new();

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        public BackupViewModel(ServerConfig config)
        {
            _backupService = new BackupService(config);
            LoadBackups();
        }

        private void LoadBackups()
        {
            Backups.Clear();
            foreach (var backup in _backupService.GetBackups())
                Backups.Add(backup);
        }

        [RelayCommand]
        private async Task CreateBackup()
        {
            IsBusy = true;
            var progress = new Progress<string>(msg => StatusMessage = msg);

            await _backupService.CreateBackupAsync(progress);

            LoadBackups();
            IsBusy = false;
        }

        [RelayCommand]
        private async Task RestoreBackup(BackupInfo backup)
        {
            IsBusy = true;
            var progress = new Progress<string>(msg => StatusMessage = msg);

            await _backupService.RestoreBackupAsync(backup.FullPath, progress);

            IsBusy = false;
        }

        [RelayCommand]
        private void DeleteBackup(BackupInfo backup)
        {
            _backupService.DeleteBackup(backup.FullPath);
            LoadBackups();
            StatusMessage = "Backup eliminado.";
        }
    }
}