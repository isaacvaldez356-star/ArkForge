using ArkForge.Common.Models;
using ArkForge.Configuration;
using ArkForge.Server;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

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

        private readonly ModService _modService;

        public ModsViewModel(ServerConfig config, ConfigService configService)
        {
            _config = config;
            _configService = configService;
            _modService = new ModService(config);

            foreach (var mod in _config.Mods)
            {
                Mods.Add(mod);
                mod.PropertyChanged += (s, e) => _configService.Save(_config);
            }
        }

        [RelayCommand]
        private async Task AddMod()
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

            StatusMessage = "Buscando nombre del mod...";

            var modName = await _modService.GetModNameAsync(id);

            var newMod = new ModInfo { WorkshopId = id, Name = modName, IsEnabled = true };
            Mods.Add(newMod);
            newMod.PropertyChanged += (s, e) => _configService.Save(_config);
            _config.Mods.Add(newMod);
            _configService.Save(_config);

            StatusMessage = $"\"{modName}\" agregado. Se descargará automáticamente al iniciar el servidor.";
        }

        [ObservableProperty]
        private bool isRefreshingNames;

        [RelayCommand]
        private async Task RefreshModNames()
        {
            IsRefreshingNames = true;
            StatusMessage = "Actualizando nombres...";

            foreach (var mod in Mods)
            {
                var newName = await _modService.GetModNameAsync(mod.WorkshopId);
                mod.Name = newName;
            }

            // Forzamos que la lista se refresque visualmente
            var temp = Mods.ToList();
            Mods.Clear();
            foreach (var mod in temp)
                Mods.Add(mod);

            _configService.Save(_config);
            IsRefreshingNames = false;
            StatusMessage = "Nombres actualizados.";
        }

        [RelayCommand]
        private void RemoveMod(ModInfo mod)
        {
            Mods.Remove(mod);
            _config.Mods.Remove(mod);
            _configService.Save(_config);
        }

        
    }
}
