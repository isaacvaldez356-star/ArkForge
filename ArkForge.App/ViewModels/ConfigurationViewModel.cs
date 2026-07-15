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

        // Multiplicadores
        [ObservableProperty] private double xpMultiplier;
        [ObservableProperty] private double tamingSpeedMultiplier;
        [ObservableProperty] private double harvestAmountMultiplier;
        [ObservableProperty] private double matingIntervalMultiplier;
        [ObservableProperty] private double babyMatureSpeedMultiplier;
        [ObservableProperty] private double playerDamageMultiplier;
        [ObservableProperty] private double dinoDamageMultiplier;
        [ObservableProperty] private double structureDamageMultiplier;
        [ObservableProperty] private double playerResistanceMultiplier;
        [ObservableProperty] private double dinoResistanceMultiplier;

        // General
        [ObservableProperty] private string serverName = string.Empty;
        [ObservableProperty] private string serverPassword = string.Empty;
        [ObservableProperty] private bool serverHardcore;
        [ObservableProperty] private bool enablePvE;

        // Dificultad
        [ObservableProperty] private double difficultyOffset;
        [ObservableProperty] private double overrideOfficialDifficulty;
        [ObservableProperty] private double itemStackSizeMultiplier;

        // Mundo e interfaz
        [ObservableProperty] private double dayCycleSpeedScale;
        [ObservableProperty] private double dayTimeSpeedScale;
        [ObservableProperty] private double nightTimeSpeedScale;
        [ObservableProperty] private double playerCharacterStaminaDrainMultiplier;
        [ObservableProperty] private double playerCharacterFoodDrainMultiplier;
        [ObservableProperty] private double playerCharacterWaterDrainMultiplier;
        [ObservableProperty] private double dinoCharacterFoodDrainMultiplier;
        [ObservableProperty] private bool serverCrosshair;
        [ObservableProperty] private bool allowThirdPersonPlayer;
        [ObservableProperty] private bool showMapPlayerLocation;
        [ObservableProperty] private bool alwaysNotifyPlayerJoined;
        [ObservableProperty] private bool alwaysNotifyPlayerLeft;

        // Estructuras
        [ObservableProperty] private bool disableStructureDecay;
        [ObservableProperty] private double maxStructuresInRange;

        // Avanzado
        [ObservableProperty] private string advancedGameUserSettings = string.Empty;
        [ObservableProperty] private string advancedGameIni = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        public ConfigurationViewModel(ServerConfig config, ConfigService configService)
        {
            _config = config;
            _configService = configService;
            _gameSettingsService = new GameSettingsService();

            ReloadFromConfig();
        }

        public void ReloadFromConfig()
        {
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

            ServerName = s.ServerName;
            ServerPassword = s.ServerPassword;
            ServerHardcore = s.ServerHardcore;
            EnablePvE = s.EnablePvE;

            DifficultyOffset = s.DifficultyOffset;
            OverrideOfficialDifficulty = s.OverrideOfficialDifficulty;
            ItemStackSizeMultiplier = s.ItemStackSizeMultiplier;

            DayCycleSpeedScale = s.DayCycleSpeedScale;
            DayTimeSpeedScale = s.DayTimeSpeedScale;
            NightTimeSpeedScale = s.NightTimeSpeedScale;
            PlayerCharacterStaminaDrainMultiplier = s.PlayerCharacterStaminaDrainMultiplier;
            PlayerCharacterFoodDrainMultiplier = s.PlayerCharacterFoodDrainMultiplier;
            PlayerCharacterWaterDrainMultiplier = s.PlayerCharacterWaterDrainMultiplier;
            DinoCharacterFoodDrainMultiplier = s.DinoCharacterFoodDrainMultiplier;
            ServerCrosshair = s.ServerCrosshair;
            AllowThirdPersonPlayer = s.AllowThirdPersonPlayer;
            ShowMapPlayerLocation = s.ShowMapPlayerLocation;
            AlwaysNotifyPlayerJoined = s.AlwaysNotifyPlayerJoined;
            AlwaysNotifyPlayerLeft = s.AlwaysNotifyPlayerLeft;

            DisableStructureDecay = s.DisableStructureDecay;
            MaxStructuresInRange = s.MaxStructuresInRange;

            AdvancedGameUserSettings = s.AdvancedGameUserSettings;
            AdvancedGameIni = s.AdvancedGameIni;
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

            s.ServerName = ServerName;
            s.ServerPassword = ServerPassword;
            s.ServerHardcore = ServerHardcore;
            s.EnablePvE = EnablePvE;

            s.DifficultyOffset = (float)DifficultyOffset;
            s.OverrideOfficialDifficulty = (float)OverrideOfficialDifficulty;
            s.ItemStackSizeMultiplier = (float)ItemStackSizeMultiplier;

            s.DayCycleSpeedScale = (float)DayCycleSpeedScale;
            s.DayTimeSpeedScale = (float)DayTimeSpeedScale;
            s.NightTimeSpeedScale = (float)NightTimeSpeedScale;
            s.PlayerCharacterStaminaDrainMultiplier = (float)PlayerCharacterStaminaDrainMultiplier;
            s.PlayerCharacterFoodDrainMultiplier = (float)PlayerCharacterFoodDrainMultiplier;
            s.PlayerCharacterWaterDrainMultiplier = (float)PlayerCharacterWaterDrainMultiplier;
            s.DinoCharacterFoodDrainMultiplier = (float)DinoCharacterFoodDrainMultiplier;
            s.ServerCrosshair = ServerCrosshair;
            s.AllowThirdPersonPlayer = AllowThirdPersonPlayer;
            s.ShowMapPlayerLocation = ShowMapPlayerLocation;
            s.AlwaysNotifyPlayerJoined = AlwaysNotifyPlayerJoined;
            s.AlwaysNotifyPlayerLeft = AlwaysNotifyPlayerLeft;

            s.DisableStructureDecay = DisableStructureDecay;
            s.MaxStructuresInRange = (int)MaxStructuresInRange;

            s.AdvancedGameUserSettings = AdvancedGameUserSettings;
            s.AdvancedGameIni = AdvancedGameIni;

            _configService.Save(_config);
            _gameSettingsService.Apply(_config.ServerPath, s);

            StatusMessage = "Configuración guardada. Reinicia el servidor para aplicar los cambios.";
        }

        [RelayCommand]
        private void ApplyAnnunakiProfile()
        {
            XpMultiplier = 15;
            TamingSpeedMultiplier = 20;
            HarvestAmountMultiplier = 5;
            MatingIntervalMultiplier = 0.5;
            BabyMatureSpeedMultiplier = 20;
            PlayerDamageMultiplier = 1;
            DinoDamageMultiplier = 1;
            StructureDamageMultiplier = 1;
            PlayerResistanceMultiplier = 1;
            DinoResistanceMultiplier = 1;

            StatusMessage = "Perfil 'Ultimate Annunaki' aplicado. No olvides darle Guardar.";
        }
    }
}