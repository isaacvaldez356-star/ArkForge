using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ArkForge.Server;
using ArkForge.Common.Models;

namespace ArkForge.App.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly ArkGameServer _server;
        private readonly ServerConfig _config;
        private readonly DispatcherTimer _timer;

        [ObservableProperty]
        private string statusText = "Offline";

        [ObservableProperty]
        private string mapText = "No seleccionado";

        [ObservableProperty]
        private string modsCountText = "0";

        [ObservableProperty]
        private string playersText = "0 / 0";

        public DashboardViewModel(ArkGameServer server, ServerConfig config)
        {
            _server = server;
            _config = config;

            MapText = _config.Map;
            ModsCountText = _config.Mods.Count(m => m.IsEnabled).ToString();
            PlayersText = $"0 / {_config.MaxPlayers}";

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _timer.Tick += async (s, e) => await RefreshAsync();
            _timer.Start();
        }

        private async Task RefreshAsync()
        {
            try
            {
                StatusText = _server.IsRunning ? "En línea" : "Offline";

                if (_server.IsRunning)
                {
                    var response = await _server.SendCommandAsync("listplayers");
                    var count = CountPlayers(response);
                    PlayersText = $"{count} / {_config.MaxPlayers}";
                }
                else
                {
                    PlayersText = $"0 / {_config.MaxPlayers}";
                }
            }
            catch
            {
                // Ignoramos errores pasajeros de conexión durante el refresco automático
            }
        }

        private int CountPlayers(string? rconResponse)
        {
            if (string.IsNullOrWhiteSpace(rconResponse))
                return 0;

            if (rconResponse.Contains("No Players Connected"))
                return 0;

            // Cada jugador conectado aparece en una línea numerada, ej: "1. NombreJugador, SteamID"
            return rconResponse.Split('\n').Count(line => line.Trim().Length > 0 && char.IsDigit(line.Trim()[0]));
        }
    }
}