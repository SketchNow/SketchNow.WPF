using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

using SketchNow.Models;
using SketchNow.Models.Enums;

namespace SketchNow.ViewModels;

public partial class MainWindowViewModel : ObservableRecipient, IRecipient<ValueChangedMessage<Settings>>
{
    public MainWindowViewModel(IMessenger messenger)
    {
        messenger.RegisterAll(this);
    }

    private readonly CustomCursors _customCursors = new();
    [ObservableProperty] public partial Cursor InkCanvasCursor { get; set; } = Cursors.Arrow;


    [ObservableProperty] public partial CanvasPages CanvasPages { get; set; } = new();
    private int _previousPageIndex;

    [ObservableProperty]
    public partial ObservableCollection<Color> ColorList { get; set; } =
    [
        Color.FromRgb(28, 27, 31),
        Colors.White,
        Color.FromRgb(255, 26, 0),
        Color.FromRgb(47, 47, 255),
        Color.FromRgb(0, 174, 128),
        Color.FromRgb(157, 118, 241),
        Color.FromRgb(255, 219, 29),
        Color.FromRgb(234, 43, 180)
    ];

    [ObservableProperty] public partial Color SelectedColor { get; set; } = Colors.Black;

    partial void OnSelectedColorChanged(Color value) => CurrentDrawingAttributes.Color = value;
    [ObservableProperty] public partial ObservableCollection<double> StrokeSizeList { get; set; } = [5, 7, 9, 11, 13, 20];
    [ObservableProperty] public partial double SelectedStrokeSize { get; set; }

    partial void OnSelectedStrokeSizeChanged(double value) =>
        CurrentDrawingAttributes.Width = CurrentDrawingAttributes.Height = value;

    [ObservableProperty]
    public partial DrawingAttributes CurrentDrawingAttributes { get; set; } = new()
    {
        Color = Colors.Black,
        IgnorePressure = Properties.Settings.Default.IsIgnorePressure,
        FitToCurve = Properties.Settings.Default.IsFitToCurve,
        Height = 5,
        Width = 5,
        IsHighlighter = false
    };

    [ObservableProperty] public partial InkCanvasEditingMode SelectedEditingMode { get; set; } = InkCanvasEditingMode.None;

    [ObservableProperty] public partial int SelectedToolIndex { get; set; }
    [ObservableProperty] public partial WhiteBoardMode SelectedWhiteBoardMode { get; set; } = WhiteBoardMode.Screen;
    [ObservableProperty] public partial bool IsMultiPageMode { get; set; }


    [RelayCommand]
    private void ToggleEditMode(int value) => SelectedToolIndex = value;
    partial void OnSelectedToolIndexChanged(int value)
    {
        SelectedEditingMode = value switch
        {
            0 => InkCanvasEditingMode.None,
            1 => InkCanvasEditingMode.Ink,
            2 => IsEraseByStroke ? InkCanvasEditingMode.EraseByStroke : InkCanvasEditingMode.EraseByPoint,
            3 => InkCanvasEditingMode.Select,
            _ => InkCanvasEditingMode.None
        };
        WindowBackgroundBrush = value switch
        {
            0 => new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
            1 => new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)),
            _ => new SolidColorBrush(Color.FromArgb(1, 0, 0, 0))
        };
        InkCanvasCursor = value switch
        {
            0 => Cursors.Arrow,
            1 => _customCursors.Circle,
            2 => _customCursors.Remove,
            3 => _customCursors.Select,
            _ => Cursors.Arrow
        };
    }

    partial void OnSelectedWhiteBoardModeChanged(WhiteBoardMode value)
    {
        IsMultiPageMode = value == WhiteBoardMode.MultiPages;
        switch (value)
        {
            case WhiteBoardMode.Screen:
                if (CanvasPages.SelectedIndex != 0)
                    _previousPageIndex = CanvasPages.SelectedIndex;
                CanvasPages.SelectedIndex = 0;
                break;
            case WhiteBoardMode.MultiPages:
                if (CanvasPages.SelectedIndex == 0)
                    if (CanvasPages.Pages.Count == 1)
                        CanvasPages.AddPageCommand.Execute(null);
                    else
                        CanvasPages.SelectedIndex = _previousPageIndex;
                break;
        }
    }

    [RelayCommand]
    private void ToggleMultiPageMode(bool value) => SelectedWhiteBoardMode =
        value ? WhiteBoardMode.MultiPages : WhiteBoardMode.Screen;


    [ObservableProperty] public partial Brush WindowBackgroundBrush { get; set; } = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

    public void Receive(ValueChangedMessage<Settings> message)
    {
        CurrentDrawingAttributes.FitToCurve = message.Value.IsFitToCurve;
        CurrentDrawingAttributes.IgnorePressure = message.Value.IsIgnorePressure;
        IsEraseByStroke = message.Value.IsEraseByStroke;
        SelectedBackground = message.Value.SelectedBackground;
    }


    /// <summary>
    /// Synchronize with <see cref="Settings.IsEraseByStroke"/>
    /// </summary>
    [ObservableProperty] public partial bool IsEraseByStroke { get; set; } = Properties.Settings.Default.IsEraseByStroke;
    partial void OnIsEraseByStrokeChanged(bool value)
    {
        switch (SelectedEditingMode)
        {
            case InkCanvasEditingMode.EraseByStroke:
            case InkCanvasEditingMode.EraseByPoint:
                SelectedEditingMode = value ? InkCanvasEditingMode.EraseByStroke : InkCanvasEditingMode.EraseByPoint;
                break;
        }
    }

    /// <summary>
    /// Synchronize with <see cref="Settings.IsFitToCurve"/>
    /// </summary>
    [ObservableProperty]
    private Brush _selectedBackground = Properties.Settings.Default.SelectedBackground;
}