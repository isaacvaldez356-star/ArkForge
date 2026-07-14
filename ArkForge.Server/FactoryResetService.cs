using ArkForge.Common.Models;

namespace ArkForge.Server
{
    public class FactoryResetService
    {
        public void Reset(ServerConfig config)
        {
            // Resetea los multiplicadores y opciones del juego a sus valores por defecto
            config.GameSettings = new GameSettings();

            // Desactiva los mods sin borrarlos de la lista (para no tener que re-descargarlos)
            foreach (var mod in config.Mods)
                mod.IsEnabled = false;

            // Borra el mundo guardado para que el próximo inicio genere un mapa nuevo
            var savedArksPath = Path.Combine(config.ServerPath, "ShooterGame", "Saved", "SavedArks");
            if (Directory.Exists(savedArksPath))
                Directory.Delete(savedArksPath, recursive: true);
        }
    }
}