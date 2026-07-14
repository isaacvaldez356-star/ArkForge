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

        public bool IsRunning
        {
            get
            {
                if (_process is { HasExited: false })
                    return true;

                // Aunque no lo hayamos iniciado en esta sesión, revisamos si sigue corriendo en Windows
                return Process.GetProcessesByName("ShooterGameServer").Length > 0;
            }
        }

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

            UpdateActiveModsIni();

            var exePath = Path.Combine(_config.ServerPath, "ShooterGame", "Binaries", "Win64", "ShooterGameServer.exe");

            var enabledMods = _config.Mods.Where(m => m.IsEnabled).Select(m => m.WorkshopId).ToList();
            var automanagedFlag = enabledMods.Count > 0 ? " -automanagedmods" : "";

            var args = $"{_config.Map}?listen?QueryPort={_config.QueryPort}?Port={_config.GamePort}" +
                       $"?MaxPlayers={_config.MaxPlayers}?ServerAdminPassword={_config.AdminPassword}?RCONEnabled=True" +
                       $" -server -RCONPort={_config.RconPort} -log -servergamelog{automanagedFlag}";

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

        private void UpdateActiveModsIni()
        {
            var iniPath = Path.Combine(_config.ServerPath, "ShooterGame", "Saved", "Config", "WindowsServer", "GameUserSettings.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(iniPath)!);

            var enabledMods = _config.Mods.Where(m => m.IsEnabled).Select(m => m.WorkshopId).ToList();
            var activeModsLine = $"ActiveMods={string.Join(",", enabledMods)}";

            var lines = File.Exists(iniPath) ? File.ReadAllLines(iniPath).ToList() : new List<string>();

            var sectionIndex = lines.FindIndex(l => l.Trim().Equals("[ServerSettings]", StringComparison.OrdinalIgnoreCase));

            if (sectionIndex == -1)
            {
                lines.Add("[ServerSettings]");
                lines.Add(activeModsLine);
            }
            else
            {
                var activeModsIndex = -1;
                var nextSectionIndex = lines.Count;

                for (var i = sectionIndex + 1; i < lines.Count; i++)
                {
                    if (lines[i].Trim().StartsWith('[') && lines[i].Trim().EndsWith(']'))
                    {
                        nextSectionIndex = i;
                        break;
                    }
                    if (lines[i].Trim().StartsWith("ActiveMods=", StringComparison.OrdinalIgnoreCase))
                    {
                        activeModsIndex = i;
                        break;
                    }
                }

                if (activeModsIndex != -1)
                    lines[activeModsIndex] = activeModsLine;
                else
                    lines.Insert(nextSectionIndex, activeModsLine);
            }

            File.WriteAllLines(iniPath, lines);
        }

        public Task StopAsync()
        {
            if (_process is { HasExited: false })
            {
                _process.Kill();
                _process = null;
            }
            else
            {
                // El servidor puede estar corriendo desde otra sesión de ArkForge
                foreach (var proc in Process.GetProcessesByName("ShooterGameServer"))
                {
                    proc.Kill();
                }
            }

            _logWatcher.Stop();
            _rconClient?.Dispose();
            _rconClient = null;
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
            try
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
            catch (Exception)
            {
                _rconClient?.Dispose();
                _rconClient = null;
                return null;
            }
        }
    }
}