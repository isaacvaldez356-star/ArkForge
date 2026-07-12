using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArkForge.Common.Models;
using ArkForge.Configuration;

namespace ArkForge.App.ViewModels
{
    public partial class ConfigurationViewModel : ObservableObject
    {
        private readonly ServerConfig _config;
        private readonly ConfigService _configService;
        private readonly GameSettingsService _gameSettingsService;

        [ObservableProperty]
        private double xpMultiplier;

        [ObservableProperty]
        private double tamingSpeedMultiplier;

        [ObservableProperty]
        private double harvestAmountMultiplier;

        [ObservableProperty]
        private double matingIntervalMultiplier;

        [ObservableProperty]
        private double babyMatureSpeedMultiplier;

        [ObservableProperty]
        private double playerDamageMultiplier;

        [ObservableProperty]
        private double dinoDamageMultiplier;

        [ObservableProperty]
        private double structureDamageMultiplier;

        [ObservableProperty]
        private double playerResistanceMultiplier;

        [ObservableProperty]
        private double dinoResistanceMultiplier;

        [ObservableProperty]
        private bool allowThirdPersonPlayer;

        [ObservableProperty]
        private bool showMapPlayerLocation;

        [ObservableProperty]
        private bool enablePvE;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        public ConfigurationViewModel(ServerConfig config, ConfigService configService)
        {
            _config = config;
            _configService = configService;
            _gameSettingsService = new GameSettingsService();

            var s = _config.GameSettings;
            XpMultiplier = s.XpMultiplier;
            TamingSpeedMultiplier = s.TamingSpeedMultiplier;
            HarvestAmountMultiplier = s.HarvestAmountMultiplier;
            MatingIntervalMultiplier = s.MatingIntervalMultiplier;
            BabyMatureSpeedMultiplier = s.BabyMatureSpeedMultiplier;
            PlayerDamageMultiplier = s.PlayerDamageMultiplier;
            DinoDamageMultiplier = s.DinoDamageMultiplier;
            StructureDamageMultiplier = s.StructureDamageMultiplier;
            PlayerResistanceMultiplier = s.PlayerResistanceMultiplier;
            DinoResistanceMultiplier = s.DinoResistanceMultiplier;
            AllowThirdPersonPlayer = s.AllowThirdPersonPlayer;
            ShowMapPlayerLocation = s.ShowMapPlayerLocation;
            EnablePvE = s.EnablePvE;
        }

        [RelayCommand]
        private void Save()
        {
            var s = _config.GameSettings;
            s.XpMultiplier = (float)XpMultiplier;
            s.TamingSpeedMultiplier = (float)TamingSpeedMultiplier;
            s.HarvestAmountMultiplier = (float)HarvestAmountMultiplier;
            s.MatingIntervalMultiplier = (float)MatingIntervalMultiplier;
            s.BabyMatureSpeedMultiplier = (float)BabyMatureSpeedMultiplier;
            s.PlayerDamageMultiplier = (float)PlayerDamageMultiplier;
            s.DinoDamageMultiplier = (float)DinoDamageMultiplier;
            s.StructureDamageMultiplier = (float)StructureDamageMultiplier;
            s.PlayerResistanceMultiplier = (float)PlayerResistanceMultiplier;
            s.DinoResistanceMultiplier = (float)DinoResistanceMultiplier;
            s.AllowThirdPersonPlayer = AllowThirdPersonPlayer;
            s.ShowMapPlayerLocation = ShowMapPlayerLocation;
            s.EnablePvE = EnablePvE;

            _configService.Save(_config);
            _gameSettingsService.Apply(_config.ServerPath, s);

            StatusMessage = "Configuración guardada. Reinicia el servidor para aplicar los cambios.";
        }
    }
}