using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArkForge.Server;
using ArkForge.Configuration;

namespace ArkForge.App.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? currentView;

        private readonly ArkGameServer _sharedServer;

        private readonly DashboardViewModel _dashboardViewModel;
        private readonly ServerViewModel _serverViewModel;
        private readonly ModsViewModel _modsViewModel;
        private readonly ConfigurationViewModel _configurationViewModel;
        private readonly BackupViewModel _backupViewModel;
        private readonly ConsoleViewModel _consoleViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        public ShellViewModel()
        {
            var configService = new ConfigService();
            var config = configService.Load();
            _sharedServer = new ArkGameServer(config);

            _dashboardViewModel = new DashboardViewModel();
            _serverViewModel = new ServerViewModel(_sharedServer);
            _modsViewModel = new ModsViewModel(config, configService);
            _configurationViewModel = new ConfigurationViewModel();
            _backupViewModel = new BackupViewModel();
            _consoleViewModel = new ConsoleViewModel(_sharedServer);
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