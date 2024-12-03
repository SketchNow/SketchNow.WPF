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

using SingleInstanceCore;

using Velopack;
using Velopack.Sources;

namespace SketchNow;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, ISingleInstance
{
#if !DEBUG
    public App()
    {
        this.Startup += Application_Startup;
        this.Exit += Application_Exit;
    }

    void Application_Startup(object sender, StartupEventArgs e)
    {
        //UI Thread
        this.DispatcherUnhandledException += AppDispatcherUnhandledException;
        //Task Thread
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
        //Non-UI Thread
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        
        bool isFirstInstance = this.InitializeAsFirstInstance("SketchNow");
        if (!isFirstInstance)
        {
            //If it's not the first instance, arguments are automatically passed to the first instance
            //OnInstanceInvoked will be raised on the first instance
            //You may shut down the current instance
            Current.Shutdown();
        }
        
    }

    static void Application_Exit(object sender, ExitEventArgs e)
    {
        //Do not forget to clean up
        SingleInstance.Cleanup();
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
#endif
        MainAsync(args).GetAwaiter().GetResult();
    }

    private static async Task MainAsync(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        //Use ConfigureAwait(true)
        //to suppress the CA2007 warning https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2007
        await host.StartAsync().ConfigureAwait(true);

        App app = new();
        app.InitializeComponent();
        app.MainWindow = host.Services.GetRequiredService<MainWindow>();
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();
        //Use ConfigureAwait(true)
        //to suppress the CA2007 warning https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2007
        await host.StopAsync().ConfigureAwait(true);
    }
#if !DEBUG
    private static async Task UpdateMyApp()
    {
        var mgr = new UpdateManager(new GithubSource(@"https://github.com/SketchNow/SketchNow.WPF", null, false));

        // check for new version
        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
            return; // no update available

        // download new version
        await mgr.DownloadUpdatesAsync(newVersion);

        // install new version and restart app
        mgr.ApplyUpdatesAndRestart(newVersion);
    }
#endif
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

    public void OnInstanceInvoked(string[] args)
    {
    }
}