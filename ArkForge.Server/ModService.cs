using System.Diagnostics;
using ArkForge.Common.Models;

namespace ArkForge.Server
{
    public class ModService
    {
        private readonly ServerConfig _config;

        // App ID de ARK (el mismo que usamos para el servidor, Workshop usa el mismo juego base)
        private const string ArkAppId = "346110";

        public ModService(ServerConfig config)
        {
            _config = config;
        }

        public async Task<bool> DownloadModAsync(string workshopId, IProgress<string>? progress = null)
        {
            var steamCmdExe = Path.Combine(_config.SteamCmdPath, "steamcmd.exe");

            if (!File.Exists(steamCmdExe))
            {
                progress?.Report("[ERROR] SteamCMD no está instalado. Instala el servidor primero.");
                return false;
            }

            progress?.Report($"Descargando mod {workshopId}...");

            var arguments = $"+login anonymous +workshop_download_item {ArkAppId} {workshopId} +quit";

            var startInfo = new ProcessStartInfo
            {
                FileName = steamCmdExe,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process != null)
                await process.WaitForExitAsync();

            var downloadedPath = GetDownloadedModPath(workshopId);
            var success = Directory.Exists(downloadedPath);

            progress?.Report(success
                ? $"Mod {workshopId} descargado correctamente."
                : $"[ERROR] No se pudo descargar el mod {workshopId}.");

            return success;
        }

        private string GetDownloadedModPath(string workshopId)
        {
            return Path.Combine(_config.SteamCmdPath, "steamapps", "workshop", "content", ArkAppId, workshopId);
        }

        public async Task<bool> InstallModToServerAsync(string workshopId, IProgress<string>? progress = null)
        {
            var sourcePath = GetDownloadedModPath(workshopId);

            if (!Directory.Exists(sourcePath))
            {
                progress?.Report($"[ERROR] El mod {workshopId} no se ha descargado todavía.");
                return false;
            }

            var modsFolder = Path.Combine(_config.ServerPath, "ShooterGame", "Content", "Mods");
            var destinationPath = Path.Combine(modsFolder, workshopId);

            progress?.Report($"Copiando mod {workshopId} al servidor...");

            await Task.Run(() => CopyDirectory(sourcePath, destinationPath));

            // El archivo .mod debe quedar al mismo nivel que la carpeta, no adentro
            var modFileSource = Path.Combine(sourcePath, $"{workshopId}.mod");
            var modFileDestination = Path.Combine(modsFolder, $"{workshopId}.mod");

            if (File.Exists(modFileSource))
            {
                File.Copy(modFileSource, modFileDestination, overwrite: true);
                progress?.Report($"Mod {workshopId} instalado en el servidor.");
                return true;
            }
            else
            {
                progress?.Report($"[ADVERTENCIA] No se encontró el archivo {workshopId}.mod — el mod puede no cargar correctamente.");
                return false;
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var destSubDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        }
    }
}