using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArkForge.Common.Models;
using ArkForge.Configuration;

namespace ArkForge.App.ViewModels
{
    public partial class ModsViewModel : ObservableObject
    {
        private readonly ServerConfig _config;
        private readonly ConfigService _configService;

        public ObservableCollection<ModInfo> Mods { get; } = new();

        [ObservableProperty]
        private string newModId = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        public ModsViewModel(ServerConfig config, ConfigService configService)
        {
            _config = config;
            _configService = configService;

            foreach (var mod in _config.Mods)
                Mods.Add(mod);
        }

        [RelayCommand]
        private void AddMod()
        {
            if (string.IsNullOrWhiteSpace(NewModId))
                return;

            var id = NewModId.Trim();
            NewModId = string.Empty;

            if (!id.All(char.IsDigit))
            {
                StatusMessage = "El ID debe ser solo números (el que aparece en la URL de Steam Workshop).";
                return;
            }

            if (Mods.Any(m => m.WorkshopId == id))
            {
                StatusMessage = "Ese mod ya está en la lista.";
                return;
            }

            var newMod = new ModInfo { WorkshopId = id, Name = $"Mod {id}", IsEnabled = true };
            Mods.Add(newMod);
            _config.Mods.Add(newMod);
            _configService.Save(_config);

            StatusMessage = $"Mod {id} agregado. Se descargará automáticamente al iniciar el servidor.";
        }

        [RelayCommand]
        private void RemoveMod(ModInfo mod)
        {
            Mods.Remove(mod);
            _config.Mods.Remove(mod);
            _configService.Save(_config);
        }

        [RelayCommand]
        private void ToggleMod(ModInfo mod)
        {
            mod.IsEnabled = !mod.IsEnabled;
            _configService.Save(_config);
        }
    }
}
