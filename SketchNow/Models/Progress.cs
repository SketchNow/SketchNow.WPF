using CommunityToolkit.Mvvm.ComponentModel;

namespace SketchNow.Models;

public partial class Progress : ObservableObject
{
    [ObservableProperty]
    public partial int Value { get; set; }

    [ObservableProperty]
    public partial bool IsIndeterminate { get; set; }

    [ObservableProperty]
    public partial bool IsVisible { get; set; }
}