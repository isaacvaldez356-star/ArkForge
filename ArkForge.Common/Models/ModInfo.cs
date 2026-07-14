using CommunityToolkit.Mvvm.ComponentModel;

namespace ArkForge.Common.Models
{
    public partial class ModInfo : ObservableObject
    {
        [ObservableProperty]
        private string workshopId = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private bool isEnabled = true;
    }
}