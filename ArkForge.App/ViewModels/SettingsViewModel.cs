using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArkForge.Common.Models;
using ArkForge.Configuration;
using ArkForge.Server;

namespace ArkForge.App.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ServerConfig _config;
        private readonly ConfigService _configService;
        private readonly ArkGameServer _server;
        private readonly FactoryResetService _resetService;
        private readonly ConfigurationViewModel _configurationViewModel;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        public SettingsViewModel(ServerConfig config, ConfigService configService, ArkGameServer server, ConfigurationViewModel configurationViewModel)
        {
            _config = config;
            _configService = configService;
            _server = server;
            _resetService = new FactoryResetService();
            _configurationViewModel = configurationViewModel;
        }

        [RelayCommand]
        private async Task FactoryReset()
        {
            var confirm = MessageBox.Show(
                "Esto va a:\n\n" +
                "• Resetear todos los multiplicadores y opciones a sus valores por defecto\n" +
                "• Desactivar todos los mods (sin borrarlos, podrás reactivarlos después)\n" +
                "• Borrar el mundo actual (se generará un mapa nuevo la próxima vez que inicies)\n\n" +
                "Los backups y la instalación del servidor NO se van a borrar.\n\n" +
                "¿Estás seguro de que quieres continuar?",
                "Confirmar reset de fábrica",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            if (_server.IsRunning)
            {
                StatusMessage = "Deteniendo el servidor antes de resetear...";
                await _server.StopAsync();
            }

            _resetService.Reset(_config);
            _configService.Save(_config);
            _configurationViewModel.ReloadFromConfig();

            StatusMessage = "Reset completo. La configuración y el mapa quedaron como nuevos.";
        }
    }
}