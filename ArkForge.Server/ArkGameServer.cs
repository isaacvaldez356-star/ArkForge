using System.Diagnostics;
using ArkForge.Core.Interfaces;
using ArkForge.Common.Models;
using ArkForge.Rcon;

namespace ArkForge.Server
{
    public class ArkGameServer : IGameServer
    {
        private readonly ServerConfig _config;
        private readonly SteamCmdService _steamCmd;
        private readonly LogWatcherService _logWatcher = new();
        private RconClient? _rconClient;
        private Process? _process;

        public event Action<string>? OutputReceived
        {
            add => _logWatcher.LineReceived += value;
            remove => _logWatcher.LineReceived -= value;
        }

        // App ID oficial de "ARK: Survival Evolved Dedicated Server" en Steam
        private const string ArkAppId = "376030";

        public ArkGameServer(ServerConfig config)
        {
            _config = config;
            _steamCmd = new SteamCmdService();
        }

        public bool IsInstalled => File.Exists(
            Path.Combine(_config.ServerPath, "ShooterGame", "Binaries", "Win64", "ShooterGameServer.exe"));

        public bool IsRunning => _process is { HasExited: false };

        public async Task InstallAsync(IProgress<double>? progress = null)
        {
            await _steamCmd.EnsureInstalledAsync(_config.SteamCmdPath, progress);

            var steamCmdExe = Path.Combine(_config.SteamCmdPath, "steamcmd.exe");

            var arguments = $"+force_install_dir \"{_config.ServerPath}\" " +
                             $"+login anonymous +app_update {ArkAppId} validate +quit";

            var startInfo = new ProcessStartInfo
            {
                FileName = steamCmdExe,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            };

            using var process = Process.Start(startInfo);
            if (process != null)
                await process.WaitForExitAsync();
        }

        public Task StartAsync()
        {
            if (IsRunning)
                return Task.CompletedTask;

            var exePath = Path.Combine(_config.ServerPath, "ShooterGame", "Binaries", "Win64", "ShooterGameServer.exe");

            var args = $"{_config.Map}?listen?QueryPort={_config.QueryPort}?Port={_config.GamePort}" +
           $"?MaxPlayers={_config.MaxPlayers}?ServerAdminPassword={_config.AdminPassword}?RCONEnabled=True" +
           $" -server -RCONPort={_config.RconPort} -log -servergamelog";

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _process = Process.Start(startInfo);

            var logPath = Path.Combine(_config.ServerPath, "ShooterGame", "Saved", "Logs", "ShooterGame.log");
            _logWatcher.Start(logPath);

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            if (IsRunning)
            {
                _process!.Kill();
                _process = null;
            }
            _logWatcher.Stop();
            return Task.CompletedTask;
        }

        public async Task RestartAsync()
        {
            await StopAsync();
            await Task.Delay(2000);
            await StartAsync();
        }

        public async Task<string?> SendCommandAsync(string command)
        {
            if (_rconClient == null || !_rconClient.IsConnected)
            {
                _rconClient = new RconClient();
                var connected = await _rconClient.ConnectAsync("127.0.0.1", _config.RconPort, _config.AdminPassword);

                if (!connected)
                    return "[No se pudo conectar por RCON. ¿El servidor ya terminó de arrancar?]";
            }

            var response = await _rconClient.SendCommandAsync(command);
            return response ?? "[Sin respuesta]";
        }
    }
}