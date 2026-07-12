namespace ArkForge.Common.Models
{
    public class GameSettings
    {
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
        public bool AllowThirdPersonPlayer { get; set; } = true;
        public bool ShowMapPlayerLocation { get; set; } = true;
        public bool EnablePvE { get; set; } = false;
    }
}