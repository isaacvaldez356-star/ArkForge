namespace ArkForge.Server
{
    public class LogWatcherService
    {
        private CancellationTokenSource? _cts;
        private long _lastPosition;

        public event Action<string>? LineReceived;

        public void Start(string logFilePath)
        {
            Stop(); // por si ya había uno corriendo

            _cts = new CancellationTokenSource();
            _lastPosition = 0;

            Task.Run(() => WatchLoop(logFilePath, _cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts = null;
        }

        private async Task WatchLoop(string logFilePath, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (File.Exists(logFilePath))
                    {
                        using var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        stream.Seek(_lastPosition, SeekOrigin.Begin);

                        using var reader = new StreamReader(stream);
                        string? line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            LineReceived?.Invoke(line);
                        }

                        _lastPosition = stream.Position;
                    }
                }
                catch (IOException)
                {
                    // El archivo puede estar bloqueado momentáneamente, lo intentamos de nuevo en el siguiente ciclo
                }

                await Task.Delay(1000, token).ContinueWith(_ => { }); // revisa cada 1 segundo
            }
        }
    }
}