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
using SketchNow.Models.Enums;
using SketchNow.Properties;

using Velopack;
using Velopack.Sources;

namespace SketchNow.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private ISnackbarMessageQueue _messageQueue;

    public MainWindowViewModel(ISnackbarMessageQueue messageQueue)
    {
        ArgumentNullException.ThrowIfNull(messageQueue);
        MessageQueue = messageQueue;

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

    [ObservableProperty] private ObservableCollection<Color> _colorList =
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

    partial void OnSelectedColorChanged(Color value) => CurrentDrawingAttributes.Color = value;
    [ObservableProperty] private ObservableCollection<double> _strokeSizeList = [5, 7, 9, 11, 13, 20];
    [ObservableProperty] private double _selectedStrokeSize;

    partial void OnSelectedStrokeSizeChanged(double value) =>
        CurrentDrawingAttributes.Width = CurrentDrawingAttributes.Height = value;

    [ObservableProperty] private DrawingAttributes _currentDrawingAttributes = new()
    {
        Color = Colors.Black,
        IgnorePressure = Settings.Default.IgnorePressure,
        FitToCurve = Settings.Default.FitToCurve,
        Height = 5,
        Width = 5,
        IsHighlighter = false
    };

    [ObservableProperty] private InkCanvasEditingMode _selectedEditingMode = InkCanvasEditingMode.None;
    
    [ObservableProperty] private int _selectedToolIndex;
    [ObservableProperty] private WhiteBoardMode _selectedWhiteBoardMode = WhiteBoardMode.Screen;
    [ObservableProperty] private bool _isMultiPageMode;
    [ObservableProperty] private bool _useEraseByStroke = Settings.Default.EraseByStroke;

    partial void OnUseEraseByStrokeChanged(bool value)
    {
        Settings.Default.EraseByStroke = value;
        Settings.Default.Save();
        OnSelectedToolIndexChanged(SelectedToolIndex);
    }

    [ObservableProperty] private ObservableCollection<Brush> _backGroundBrushes =
    [
        new SolidColorBrush(Colors.White),
        new SolidColorBrush(Color.FromRgb(255, 253, 209)),
        new SolidColorBrush(Color.FromRgb(1, 71, 54)),
        new SolidColorBrush(Color.FromRgb(25, 25, 25)),
        new DrawingBrush
        {
            Viewport = new Rect(0, 0, 80, 80),
            ViewportUnits = BrushMappingMode.Absolute,
            TileMode = TileMode.Tile,
            Drawing = new DrawingGroup
            {
                Children =
                {
                    new GeometryDrawing
                    {
                        Brush = Brushes.White, Geometry = new RectangleGeometry(new Rect(0, 0, 1, 1))
                    },
                    new GeometryDrawing
                    {
                        Brush = new SolidColorBrush(Color.FromArgb(76, 0, 0, 0)),
                        Geometry = Geometry.Parse("M 0,0 L 0,1 0.1,1 0.1,0.1 1,0.1 1,0 Z")
                    }
                }
            }
        }
    ];

    [ObservableProperty] private Brush _selectedBrush = new SolidColorBrush(Colors.White);
    [ObservableProperty] private Brush _windowBackgroundBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

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
    private static void Close()
    {
        Application.Current.Shutdown();
    }

    [RelayCommand]
    private void ToggleMultiPageMode(bool value) => SelectedWhiteBoardMode =
        value ? WhiteBoardMode.MultiPages : WhiteBoardMode.Screen;


    
    [ObservableProperty] private Progress _progress = new();
    readonly UpdateManager _mgr = new(new GithubSource(@"https://github.com/SketchNow/SketchNow.WPF", null, false));

    [RelayCommand]
    private async Task CheckForUpdates()
    {
        UpdateInfo? newVersion = null;

        try
        {
            newVersion = await _mgr.CheckForUpdatesAsync();
        }
        catch (Exception e)
        {
            MessageQueue.Enqueue(e.Message);
        }
        if (newVersion != null)
            MessageQueue.Enqueue(SketchNow.Properties.Resource.NewVersionFound, SketchNow.Properties.Resource.InstallAndUpdate,
                _ => UpdateAppCommand.ExecuteAsync(newVersion),
                null,
                false,
                true,
                TimeSpan.FromSeconds(10));
        else
            MessageQueue.Enqueue(SketchNow.Properties.Resource.UpdatesAreNotAvailable);
    }

    [RelayCommand]
    private async Task UpdateApp(UpdateInfo newVersion)
    {
        Progress.IsVisible = true;
        Progress.IsIndeterminate = true;

        MessageQueue.Enqueue(SketchNow.Properties.Resource.DownloadingUpdatesPleaseWait);

        await Task.Delay(5);

        Progress.IsIndeterminate = false;

        await _mgr.DownloadUpdatesAsync(newVersion, i => Progress.Value = i);
        MessageQueue.Enqueue(SketchNow.Properties.Resource.DownloadedUpdatesPleaseWait);
        Progress.IsIndeterminate = true;
        _mgr.ApplyUpdatesAndRestart(newVersion);
    }
}