namespace ArkForge.Core.Interfaces
{
    public interface IGameServer
    {
        Task InstallAsync(IProgress<double>? progress = null);
        Task StartAsync();
        Task StopAsync();
        Task RestartAsync();

        bool IsRunning { get; }
        bool IsInstalled { get; }

        event Action<string>? OutputReceived;
    }
}