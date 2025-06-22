using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using Velopack;
using MaterialDesignThemes.Wpf;
using SketchNow.Models;
using Velopack.Sources;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using SketchNow.Services;

namespace SketchNow.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial ObservableCollection<Brush> BackGroundBrushes { get; set; } =
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

    public SettingsViewModel(
        IMessenger messenger,
        ISnackbarMessageQueue messageQueue,
        ISketchNowConfigurationService sketchNowConfigurationService
        )
    {
        MessageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
        IsActive = true;
        _sketchNowConfiguration = sketchNowConfigurationService;
        Settings = _sketchNowConfiguration.Settings;

        Settings.PropertyChanged += (_, _) =>
        {
            _sketchNowConfiguration.Settings = Settings;
            messenger.Send(
                new ValueChangedMessage<SketchNowSettings>(Settings)
            );
        };
    }

    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    public partial SketchNowSettings Settings { get ; set ;}

    [ObservableProperty] public partial ISnackbarMessageQueue MessageQueue { get; set; }

    [RelayCommand]
    private static void Close()
    {
        Application.Current.Shutdown();
    }

    [ObservableProperty] public partial Progress Progress { get; set; } = new();
    readonly UpdateManager _mgr = new(new GithubSource("https://github.com/SketchNow/SketchNow.WPF", null, false));
    private readonly ISketchNowConfigurationService _sketchNowConfiguration;

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
        {
            MessageQueue.Enqueue(Properties.Resource.NewVersionFound, Properties.Resource.InstallAndUpdate,
                        _ => UpdateAppCommand.ExecuteAsync(newVersion),
                        null,
                        false,
                        true,
                        TimeSpan.FromSeconds(10));
        }
        else
        {
            MessageQueue.Enqueue(Properties.Resource.UpdatesAreNotAvailable);
        }
    }

    [RelayCommand]
    private async Task UpdateApp(UpdateInfo newVersion)
    {
        Progress.IsVisible = true;
        Progress.IsIndeterminate = true;
        MessageQueue.Enqueue(Properties.Resource.DownloadingUpdatesPleaseWait);

        await Task.Delay(5);

        Progress.IsIndeterminate = false;

        await _mgr.DownloadUpdatesAsync(newVersion, i => Progress.Value = i);
        MessageQueue.Enqueue(Properties.Resource.DownloadedUpdatesPleaseWait);
        Progress.IsIndeterminate = true;
        _mgr.ApplyUpdatesAndRestart(newVersion);
    }
}