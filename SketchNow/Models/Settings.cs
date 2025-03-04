using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SketchNow.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty]
    public partial bool IsFitToCurve { get; set; }

    [ObservableProperty]
    public partial bool IsIgnorePressure { get; set; }

    [ObservableProperty]
    public partial bool IsEraseByStroke { get; set; }

    [ObservableProperty]
    public partial Brush SelectedBackground { get; set; }

    public Settings()
    {
        IsFitToCurve = Properties.Settings.Default.IsFitToCurve;
        IsIgnorePressure = Properties.Settings.Default.IsIgnorePressure;
        IsEraseByStroke = Properties.Settings.Default.IsEraseByStroke;
        SelectedBackground = Properties.Settings.Default.SelectedBackground;
    }

    public Settings(bool isFitToCurve, bool isIgnorePressure, bool isEraseByStroke, Brush selectedBackground)
    {
        IsFitToCurve = isFitToCurve;
        IsIgnorePressure = isIgnorePressure;
        IsEraseByStroke = isEraseByStroke;
        SelectedBackground = selectedBackground;
    }

    [RelayCommand]
    private void SaveSettings()
    {
        Properties.Settings.Default.IsFitToCurve = IsFitToCurve;
        Properties.Settings.Default.IsIgnorePressure = IsIgnorePressure;
        Properties.Settings.Default.IsEraseByStroke = IsEraseByStroke;
        Properties.Settings.Default.SelectedBackground = SelectedBackground;
        Properties.Settings.Default.Save();
    }
}