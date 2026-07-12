using ArkForge.Common.Models;

namespace ArkForge.Configuration
{
    public class GameSettingsService
    {
        public void Apply(string serverPath, GameSettings settings)
        {
            WriteGameIni(serverPath, settings);
            WriteGameUserSettingsIni(serverPath, settings);
        }

        private void WriteGameIni(string serverPath, GameSettings settings)
        {
            var iniPath = Path.Combine(serverPath, "ShooterGame", "Saved", "Config", "WindowsServer", "Game.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(iniPath)!);

            var lines = File.Exists(iniPath) ? File.ReadAllLines(iniPath).ToList() : new List<string>();

            var values = new Dictionary<string, string>
            {
                ["XPMultiplier"] = settings.XpMultiplier.ToString("0.0"),
                ["TamingSpeedMultiplier"] = settings.TamingSpeedMultiplier.ToString("0.0"),
                ["HarvestAmountMultiplier"] = settings.HarvestAmountMultiplier.ToString("0.0"),
                ["MatingIntervalMultiplier"] = settings.MatingIntervalMultiplier.ToString("0.0"),
                ["BabyMatureSpeedMultiplier"] = settings.BabyMatureSpeedMultiplier.ToString("0.0"),
                ["PlayerDamageMultiplier"] = settings.PlayerDamageMultiplier.ToString("0.0"),
                ["DinoDamageMultiplier"] = settings.DinoDamageMultiplier.ToString("0.0"),
                ["StructureDamageMultiplier"] = settings.StructureDamageMultiplier.ToString("0.0"),
                ["PlayerResistanceMultiplier"] = settings.PlayerResistanceMultiplier.ToString("0.0"),
                ["DinoResistanceMultiplier"] = settings.DinoResistanceMultiplier.ToString("0.0"),
            };

            UpdateIniSection(lines, "[/script/shootergame.shootergamemode]", values);

            File.WriteAllLines(iniPath, lines);
        }

        private void WriteGameUserSettingsIni(string serverPath, GameSettings settings)
        {
            var iniPath = Path.Combine(serverPath, "ShooterGame", "Saved", "Config", "WindowsServer", "GameUserSettings.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(iniPath)!);

            var lines = File.Exists(iniPath) ? File.ReadAllLines(iniPath).ToList() : new List<string>();

            var values = new Dictionary<string, string>
            {
                ["AllowThirdPersonPlayer"] = settings.AllowThirdPersonPlayer.ToString(),
                ["ShowMapPlayerLocation"] = settings.ShowMapPlayerLocation.ToString(),
                ["ServerPVE"] = settings.EnablePvE.ToString(),
            };

            UpdateIniSection(lines, "[ServerSettings]", values);

            File.WriteAllLines(iniPath, lines);
        }

        private void UpdateIniSection(List<string> lines, string sectionName, Dictionary<string, string> values)
        {
            var sectionIndex = lines.FindIndex(l => l.Trim().Equals(sectionName, StringComparison.OrdinalIgnoreCase));

            if (sectionIndex == -1)
            {
                lines.Add(sectionName);
                sectionIndex = lines.Count - 1;
            }

            foreach (var kvp in values)
            {
                var nextSectionIndex = lines.Count;
                var keyIndex = -1;

                for (var i = sectionIndex + 1; i < lines.Count; i++)
                {
                    if (lines[i].Trim().StartsWith('[') && lines[i].Trim().EndsWith(']'))
                    {
                        nextSectionIndex = i;
                        break;
                    }
                    if (lines[i].Trim().StartsWith($"{kvp.Key}=", StringComparison.OrdinalIgnoreCase))
                    {
                        keyIndex = i;
                        break;
                    }
                }

                var newLine = $"{kvp.Key}={kvp.Value}";

                if (keyIndex != -1)
                    lines[keyIndex] = newLine;
                else
                    lines.Insert(nextSectionIndex, newLine);
            }
        }
    }
}