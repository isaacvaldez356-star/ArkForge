using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArkForge.Server;

namespace ArkForge.App.ViewModels
{
    public partial class ConsoleViewModel : ObservableObject
    {
        private readonly ArkGameServer _server;

        public ObservableCollection<string> Lines { get; } = new();

        [ObservableProperty]
        private string commandText = string.Empty;

        public ConsoleViewModel(ArkGameServer server)
        {
            _server = server;
        }

        [RelayCommand]
        private async Task SendCommand()
        {
            if (string.IsNullOrWhiteSpace(CommandText))
                return;

            var command = CommandText;
            Lines.Add($"> {command}");
            CommandText = string.Empty;

            var response = await _server.SendCommandAsync(command);
            Lines.Add(response ?? "[Sin respuesta]");
        }

        [RelayCommand]
        private void Clear()
        {
            Lines.Clear();
        }
    }
}