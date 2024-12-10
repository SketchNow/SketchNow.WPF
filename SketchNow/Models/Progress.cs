using CommunityToolkit.Mvvm.ComponentModel;

namespace SketchNow.Models;

public partial class Progress : ObservableObject
{
    [ObservableProperty]
    private int _value;
    [ObservableProperty]
    private bool _isIndeterminate;
    [ObservableProperty]
    private bool _isVisible;
}