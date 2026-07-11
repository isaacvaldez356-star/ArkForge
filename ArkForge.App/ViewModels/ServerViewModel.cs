using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArkForge.Server;
using ArkForge.Configuration;

namespace ArkForge.App.ViewModels
{
    public partial class ServerViewModel : ObservableObject
    {
        private readonly ArkGameServer _server;
        

        [ObservableProperty]
        private double installProgress;

        [ObservableProperty]
        private bool isInstalling;

        [ObservableProperty]
        private string statusText = "No instalado";

        public ServerViewModel(ArkGameServer server)
        {
            _server = server;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (_server.IsRunning)
                StatusText = "En línea";
            else if (_server.IsInstalled)
                StatusText = "Instalado (detenido)";
            else
                StatusText = "No instalado";
        }

        [RelayCommand]
        private async Task InstallAsync()
        {
            IsInstalling = true;

            var progress = new Progress<double>(p => InstallProgress = p);
            await _server.InstallAsync(progress);

            IsInstalling = false;
            UpdateStatus();
        }

        [RelayCommand]
        private async Task StartAsync()
        {
            await _server.StartAsync();
            UpdateStatus();
        }

        [RelayCommand]
        private async Task StopAsync()
        {
            await _server.StopAsync();
            UpdateStatus();
        }
    }
}