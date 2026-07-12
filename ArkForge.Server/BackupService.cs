using System.IO.Compression;
using ArkForge.Common.Models;

namespace ArkForge.Server
{
    public class BackupInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public long SizeBytes { get; set; }
    }

    public class BackupService
    {
        private readonly ServerConfig _config;

        public BackupService(ServerConfig config)
        {
            _config = config;
        }

        private string BackupsFolder => Path.Combine(_config.ServerPath, "..", "Backups");

        private string SavedArksFolder => Path.Combine(_config.ServerPath, "ShooterGame", "Saved", "SavedArks");

        public async Task<bool> CreateBackupAsync(IProgress<string>? progress = null)
        {
            if (!Directory.Exists(SavedArksFolder))
            {
                progress?.Report("[ERROR] No se encontró la carpeta de guardado del mundo. ¿El servidor ya se inició al menos una vez?");
                return false;
            }

            Directory.CreateDirectory(BackupsFolder);

            var fileName = $"Backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.zip";
            var fullPath = Path.Combine(BackupsFolder, fileName);

            progress?.Report("Creando backup...");

            await Task.Run(() => ZipFile.CreateFromDirectory(SavedArksFolder, fullPath, CompressionLevel.Optimal, false));

            progress?.Report($"Backup creado: {fileName}");
            return true;
        }

        public List<BackupInfo> GetBackups()
        {
            if (!Directory.Exists(BackupsFolder))
                return new List<BackupInfo>();

            return Directory.GetFiles(BackupsFolder, "*.zip")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .Select(f => new BackupInfo
                {
                    FileName = f.Name,
                    FullPath = f.FullName,
                    CreatedAt = f.CreationTime,
                    SizeBytes = f.Length
                })
                .ToList();
        }

        public async Task<bool> RestoreBackupAsync(string backupFullPath, IProgress<string>? progress = null)
        {
            if (!File.Exists(backupFullPath))
            {
                progress?.Report("[ERROR] El archivo de backup no existe.");
                return false;
            }

            progress?.Report("Restaurando backup...");

            await Task.Run(() =>
            {
                if (Directory.Exists(SavedArksFolder))
                    Directory.Delete(SavedArksFolder, recursive: true);

                ZipFile.ExtractToDirectory(backupFullPath, SavedArksFolder);
            });

            progress?.Report("Backup restaurado correctamente.");
            return true;
        }

        public void DeleteBackup(string backupFullPath)
        {
            if (File.Exists(backupFullPath))
                File.Delete(backupFullPath);
        }
    }
}