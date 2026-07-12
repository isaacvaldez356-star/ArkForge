namespace ArkForge.Common.Models
{
    public class ServerConfig
    {
        public string AdminPassword { get; set; } = "ArkForge123";
        public string ServerPath { get; set; } = "C:\\ArkForge\\Server";
        public string SteamCmdPath { get; set; } = "C:\\ArkForge\\SteamCMD";
        public List<ModInfo> Mods { get; set; } = new();
        public GameSettings GameSettings { get; set; } = new();
        public string Map { get; set; } = "TheIsland";
        public int MaxPlayers { get; set; } = 10;
        public int QueryPort { get; set; } = 27015;
        public int GamePort { get; set; } = 7777;
        public int RconPort { get; set; } = 27020;

    }
}