using System.Diagnostics;
using ArkForge.Core.Interfaces;
using ArkForge.Common.Models;

namespace ArkForge.Server
{
    public class ArkGameServer : IGameServer
    {
        private readonly ServerConfig _config;
        private readonly SteamCmdService _steamCmd;
        private Process? _process;

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
                       $"?MaxPlayers={_config.MaxPlayers} -server -RCONPort={_config.RconPort}";

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                UseShellExecute = false
            };

            _process = Process.Start(startInfo);
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            if (IsRunning)
            {
                _process!.Kill();
                _process = null;
            }
            return Task.CompletedTask;
        }

        public async Task RestartAsync()
        {
            await StopAsync();
            await Task.Delay(2000);
            await StartAsync();
        }
    }
}