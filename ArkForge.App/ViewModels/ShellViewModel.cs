using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ArkForge.App.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? currentView;

        private readonly DashboardViewModel _dashboardViewModel;
        private readonly ServerViewModel _serverViewModel;
        private readonly ModsViewModel _modsViewModel;
        private readonly ConfigurationViewModel _configurationViewModel;
        private readonly BackupViewModel _backupViewModel;
        private readonly ConsoleViewModel _consoleViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        public ShellViewModel()
        {
            _dashboardViewModel = new DashboardViewModel();
            _serverViewModel = new ServerViewModel();
            _modsViewModel = new ModsViewModel();
            _configurationViewModel = new ConfigurationViewModel();
            _backupViewModel = new BackupViewModel();
            _consoleViewModel = new ConsoleViewModel();
            _settingsViewModel = new SettingsViewModel();

            CurrentView = _dashboardViewModel;
        }

        [RelayCommand]
        private void ShowDashboard() => CurrentView = _dashboardViewModel;

        [RelayCommand]
        private void ShowServer() => CurrentView = _serverViewModel;

        [RelayCommand]
        private void ShowMods() => CurrentView = _modsViewModel;

        [RelayCommand]
        private void ShowConfiguration() => CurrentView = _configurationViewModel;

        [RelayCommand]
        private void ShowBackup() => CurrentView = _backupViewModel;

        [RelayCommand]
        private void ShowConsole() => CurrentView = _consoleViewModel;

        [RelayCommand]
        private void ShowSettings() => CurrentView = _settingsViewModel;
    }
}