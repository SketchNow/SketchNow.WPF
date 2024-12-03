using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

using SketchNow.Models;
using SketchNow.Properties;

namespace SketchNow.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ISnackbarMessageQueue _messageQueue;

    public MainWindowViewModel(ISnackbarMessageQueue messageQueue)
    {
        _messageQueue = messageQueue;

        CurrentDrawingAttributes.AttributeChanged += (_, _) =>
        {
            Settings.Default.FitToCurve = CurrentDrawingAttributes.FitToCurve;
            Settings.Default.IgnorePressure = CurrentDrawingAttributes.IgnorePressure;
            Settings.Default.Save();
        };
    }

    private readonly CustomCursors _customCursors = new();
    [ObservableProperty] private Cursor _inkCanvasCursor = Cursors.Arrow;
    [ObservableProperty] private CanvasPages _canvasPages = new();
    private int _previousPageIndex;
    [ObservableProperty]
    private ObservableCollection<Color> _colorList =
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
    [ObservableProperty] private Color _selectedColor = Colors.Black;
    [ObservableProperty] private ObservableCollection<double> _strokeSizeList = [5, 7, 9, 11, 13, 20];
    [ObservableProperty] private double _selectedStrokeSize;

    [ObservableProperty]
    private DrawingAttributes _currentDrawingAttributes = new()
    {
        Color = Colors.Black,
        IgnorePressure = Settings.Default.IgnorePressure,
        FitToCurve = Settings.Default.FitToCurve,
        Height = 5,
        Width = 5,
        IsHighlighter = false
    };
    [ObservableProperty] private InkCanvasEditingMode _selectedEditingMode = InkCanvasEditingMode.None;
    public enum WhiteBoardMode
    {
        Screen = 0,
        MultiPages = 1
    }
    [ObservableProperty] private int _selectedToolIndex;
    [ObservableProperty] private int _selectedCanvasModeIndex = (int)WhiteBoardMode.Screen;
    [ObservableProperty] private bool _isMultiPageMode;
    [ObservableProperty] private bool _useEraseByStroke = Settings.Default.EraseByStroke;
    [ObservableProperty] private ObservableCollection<Brush> _backGroundBrushes =
    [
        new SolidColorBrush(Colors.White),
        new SolidColorBrush(Color.FromRgb(255, 253, 209)),
        new SolidColorBrush(Color.FromRgb(1, 71, 54)),
        new SolidColorBrush(Color.FromRgb(25, 25, 25)),
        new DrawingBrush
        {
            Viewport = new Rect(0, 0, 50, 50),
            ViewportUnits = BrushMappingMode.Absolute,
            TileMode = TileMode.Tile,
            Drawing = new DrawingGroup
            {
                Children =
                {
                    new GeometryDrawing
                    {
                        Brush = Brushes.White,
                        Geometry = new RectangleGeometry(new Rect(0, 0, 1, 1))
                    },
                    new GeometryDrawing
                    {
                        Brush = new SolidColorBrush(Color.FromArgb(50,255,244,103)),
                        Geometry = Geometry.Parse("M 0,0 L 0,1 0.1,1 0.1,0.1 1,0.1 1,0 Z")
                    }
                }
            }
        }
    ];
    [ObservableProperty] private Brush _selectedBrush = new SolidColorBrush(Colors.White);
    [ObservableProperty] private Brush _windowBackgroundBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
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

    [RelayCommand]
    private void ToggleEditMode(int value) => SelectedToolIndex = value;
    partial void OnSelectedCanvasModeIndexChanged(int value)
    {
        IsMultiPageMode = value == (int)WhiteBoardMode.MultiPages;
        switch (value)
        {
            case (int)WhiteBoardMode.Screen:
                if (CanvasPages.SelectedIndex != 0)
                    _previousPageIndex = CanvasPages.SelectedIndex;
                CanvasPages.SelectedIndex = 0;
                break;
            case (int)WhiteBoardMode.MultiPages:
                if (CanvasPages.SelectedIndex == 0)
                    if (CanvasPages.Pages.Count == 1)
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
}