using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace SketchNow.Models;

public partial class SketchNowSettings : ObservableObject
{
    [ObservableProperty]
    public required partial bool IsIgnorePressure { get; set; }

    [ObservableProperty]
    public required partial bool IsEraseByStroke { get; set; }

    [ObservableProperty]
    public required partial bool IsFitToCurve { get; set; }

    [ObservableProperty]
    public required partial Brush SelectedBackground { get; set; }
}