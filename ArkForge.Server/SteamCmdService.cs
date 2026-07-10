using System.IO.Compression;
using System.Net.Http;

namespace ArkForge.Server
{
    public class SteamCmdService
    {
        private const string SteamCmdUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";

        public async Task EnsureInstalledAsync(string steamCmdPath, IProgress<double>? progress = null)
        {
            var exePath = Path.Combine(steamCmdPath, "steamcmd.exe");

            if (File.Exists(exePath))
                return; // Ya está instalado

            Directory.CreateDirectory(steamCmdPath);

            var zipPath = Path.Combine(steamCmdPath, "steamcmd.zip");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(SteamCmdUrl, HttpCompletionOption.ResponseHeadersRead);
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None);

                var buffer = new byte[8192];
                long totalRead = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    totalRead += bytesRead;

                    if (totalBytes > 0)
                        progress?.Report((double)totalRead / totalBytes * 100);
                }
            }

            ZipFile.ExtractToDirectory(zipPath, steamCmdPath, overwriteFiles: true);
            File.Delete(zipPath);
        }
    }
}