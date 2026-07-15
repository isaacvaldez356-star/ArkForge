namespace ArkForge.Common.Models
{
    public class GameSettings
    {
        // Multiplicadores (ya existentes)
        public float XpMultiplier { get; set; } = 1.0f;
        public float TamingSpeedMultiplier { get; set; } = 1.0f;
        public float HarvestAmountMultiplier { get; set; } = 1.0f;
        public float MatingIntervalMultiplier { get; set; } = 1.0f;
        public float BabyMatureSpeedMultiplier { get; set; } = 1.0f;
        public float PlayerDamageMultiplier { get; set; } = 1.0f;
        public float DinoDamageMultiplier { get; set; } = 1.0f;
        public float StructureDamageMultiplier { get; set; } = 1.0f;
        public float PlayerResistanceMultiplier { get; set; } = 1.0f;
        public float DinoResistanceMultiplier { get; set; } = 1.0f;

        // General
        public string ServerName { get; set; } = "Servidor ArkForge";
        public string ServerPassword { get; set; } = string.Empty;
        public bool ServerHardcore { get; set; } = false;
        public bool EnablePvE { get; set; } = false;

        // Dificultad y progresión
        public float DifficultyOffset { get; set; } = 1.0f;
        public float OverrideOfficialDifficulty { get; set; } = 5.0f;
        public float ItemStackSizeMultiplier { get; set; } = 1.0f;

        // Mundo e interfaz
        public float DayCycleSpeedScale { get; set; } = 1.0f;
        public float DayTimeSpeedScale { get; set; } = 1.0f;
        public float NightTimeSpeedScale { get; set; } = 1.0f;
        public float PlayerCharacterStaminaDrainMultiplier { get; set; } = 1.0f;
        public float PlayerCharacterFoodDrainMultiplier { get; set; } = 1.0f;
        public float PlayerCharacterWaterDrainMultiplier { get; set; } = 1.0f;
        public float DinoCharacterFoodDrainMultiplier { get; set; } = 1.0f;
        public bool ServerCrosshair { get; set; } = false;
        public bool AllowThirdPersonPlayer { get; set; } = true;
        public bool ShowMapPlayerLocation { get; set; } = true;
        public bool AlwaysNotifyPlayerJoined { get; set; } = false;
        public bool AlwaysNotifyPlayerLeft { get; set; } = false;

        // Estructuras
        public bool DisableStructureDecay { get; set; } = false;
        public int MaxStructuresInRange { get; set; } = 10500;

        // Avanzado (texto libre, una línea por configuración, formato Clave=Valor)
        public string AdvancedGameUserSettings { get; set; } = string.Empty;
        public string AdvancedGameIni { get; set; } = string.Empty;
    }
}