using System.Text;

using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SketchNow.ViewModels;
using SketchNow.Views;

using System.Windows;
using System.Windows.Threading;

using Microsoft.VisualBasic;

using Velopack;
using Velopack.Sources;

namespace SketchNow;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
#if !DEBUG
    public App()
    {
        this.Startup += AppStartup;
        this.Exit += AppExit;
    }

    void AppStartup(object sender, StartupEventArgs e)
    {
        //UI Thread
        this.DispatcherUnhandledException += AppDispatcherUnhandledException;
        //Task Thread
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
        //Non-UI Thread
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
    }

    static void AppExit(object sender, ExitEventArgs e)
    {
    }

    static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            e.Handled = true;
            MessageBox.Show(
                $"UI thread meets an exception: {e.Exception.Message}{Environment.NewLine}{e.Exception.StackTrace}",
                "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"UI thread meets a fatal exception! {ex.Message}{Environment.NewLine}{ex.StackTrace}",
                "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        StringBuilder sbEx = new();
        if (e.IsTerminating)
        {
            sbEx.Append("Non-UI thread meets fatal exception.");
        }

        sbEx.Append("Non-UI thread exception: ");
        if (e.ExceptionObject is Exception exception)
        {
            sbEx.Append(exception.Message + "\n" + exception.StackTrace);
        }
        else
        {
            sbEx.Append(e.ExceptionObject);
        }

        MessageBox.Show(sbEx.ToString());
    }

    static void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        MessageBox.Show(
            $"Task thread meets exception：{e.Exception.Message}{Environment.NewLine}{e.Exception.StackTrace}",
            "Unobserved Task", MessageBoxButton.OK, MessageBoxImage.Error);
        e.SetObserved();
    }

#endif
    [STAThread]
    private static void Main(string[] args)
    {
#if !DEBUG
        VelopackApp.Build().Run();
        UpdateMyApp().GetAwaiter().GetResult();
#endif
        MainAsync(args).GetAwaiter().GetResult();
    }
#if !DEBUG
    private static async Task UpdateMyApp()
    {
        try
        {
            var mgr = new UpdateManager(new GithubSource("https://github.com/SketchNow/SketchNow.WPF", null, false));

            // check for new version
            var newVersion = await mgr.CheckForUpdatesAsync();
            if (newVersion == null)
                return; // no update available

            // download new version
            await mgr.DownloadUpdatesAsync(newVersion);

            // install new version and restart app
            mgr.ApplyUpdatesAndRestart(newVersion);
        }
        catch (Exception e)
        {
            MessageBox.Show("Updated failed: " + e.Message + Environment.NewLine + e.StackTrace, "Update Failed",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
#endif
    private static async Task MainAsync(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        await host.StartAsync().ConfigureAwait(true);

        App app = new();
        app.InitializeComponent();
        app.MainWindow = host.Services.GetRequiredService<MainWindow>();
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();

        await host.StopAsync().ConfigureAwait(true);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder)
                => configurationBuilder.AddUserSecrets(typeof(App).Assembly))
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<WeakReferenceMessenger>();
                services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider =>
                    provider.GetRequiredService<WeakReferenceMessenger>());

                services.AddSingleton(_ => Current.Dispatcher);

                services.AddTransient<ISnackbarMessageQueue>(provider =>
                {
                    Dispatcher dispatcher = provider.GetRequiredService<Dispatcher>();
                    return new SnackbarMessageQueue(TimeSpan.FromSeconds(3.0), dispatcher);
                });
            });
}