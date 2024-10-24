using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SketchNow.Models;
using SketchNow.Properties;

namespace SketchNow.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private CanvasPages _canvasPages = new();
    private int _previousPageIndex = 0;

    [ObservableProperty]
    private ObservableCollection<Color> _colorList =
    [
        Color.FromRgb(28, 27, 31), Colors.White, Color.FromRgb(255, 26, 0), Color.FromRgb(47, 47, 255),
        Color.FromRgb(0, 174, 128), Color.FromRgb(157, 118, 241), Color.FromRgb(255, 219, 29),
        Color.FromRgb(234, 43, 180)
    ];

    [ObservableProperty] private Color _selectedColor = Colors.Transparent;
    [ObservableProperty] private ObservableCollection<double> _strokeSizeList = [5, 7, 9, 11, 13, 20];
    [ObservableProperty] private double _selectedStrokeSize;

    [ObservableProperty]
    private DrawingAttributes _currentDrawingAttributes = new()
    {
        Color = Colors.Transparent,
        IgnorePressure = Settings.Default.IgnorePressure,
        FitToCurve = Settings.Default.FitToCurve,
        Height = 5,
        Width = 5,
        IsHighlighter = true
    };
    [ObservableProperty] private InkCanvasEditingMode _selectedEditingMode = InkCanvasEditingMode.None;

    public enum WhiteBoardMode
    {
        Screen = 0,
        MultiPages = 1
    }

    [ObservableProperty] private int _selectedToolIndex = 0;
    [ObservableProperty] private int _selectedCanvasModeIndex = (int)WhiteBoardMode.Screen;
    [ObservableProperty] private bool _isMultiPageMode = false;
    [ObservableProperty] private bool _useEraseByStroke = Settings.Default.EraseByStroke;
    [ObservableProperty] private Brush _inkCanvasBackground = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

    partial void OnSelectedStrokeSizeChanged(double value) =>
        CurrentDrawingAttributes.Width = CurrentDrawingAttributes.Height = value;
    partial void OnSelectedColorChanged(Color value) => CurrentDrawingAttributes.Color = value;
    partial void OnUseEraseByStrokeChanged(bool value)
    {
        Settings.Default.EraseByStroke = value;
        Settings.Default.Save();
        OnSelectedToolIndexChanged(SelectedToolIndex);
    }
    partial void OnSelectedToolIndexChanged(int value)
    {
        SelectedEditingMode = value switch
        {
            0 => InkCanvasEditingMode.None,
            1 => InkCanvasEditingMode.Ink,
            2 => UseEraseByStroke ? InkCanvasEditingMode.EraseByStroke : InkCanvasEditingMode.EraseByPoint,
            3 => InkCanvasEditingMode.Select,
            _ => InkCanvasEditingMode.None
        };
    }

    [RelayCommand]
    private void ToggleEditMode(int value) => SelectedToolIndex = value;

    partial void OnSelectedCanvasModeIndexChanged(int value)
    {
        IsMultiPageMode = value == (int)WhiteBoardMode.MultiPages;
        switch (value)
        {
            case (int)WhiteBoardMode.Screen:
                InkCanvasBackground = new SolidColorBrush(Colors.Transparent);
                if (CanvasPages.SelectedIndex != 0)
                    _previousPageIndex = CanvasPages.SelectedIndex;
                CanvasPages.SelectedIndex = 0;
                break;
            case (int)WhiteBoardMode.MultiPages:
                InkCanvasBackground = new SolidColorBrush(Colors.White);
                if (CanvasPages.SelectedIndex == 0)
                    if (CanvasPages.Length == 1)
                        CanvasPages.AddPageCommand.Execute(null);
                    else
                        CanvasPages.SelectedIndex = _previousPageIndex;
                break;
        }
    }
    [RelayCommand]
    private static void Close()
    {
        Application.Current.Shutdown();
    }
    [RelayCommand]
    private void ToggleMultiPageMode(bool value) => SelectedCanvasModeIndex =
        value ? (int)WhiteBoardMode.MultiPages : (int)WhiteBoardMode.Screen;
    public MainWindowViewModel()
    {
        CurrentDrawingAttributes.AttributeChanged += (sender, e) =>
        {
            Settings.Default.FitToCurve = CurrentDrawingAttributes.FitToCurve;
            Settings.Default.IgnorePressure = CurrentDrawingAttributes.IgnorePressure;
            Settings.Default.Save();
        };
    }
}