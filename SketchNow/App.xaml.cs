﻿using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using SingleInstanceCore;

using SketchNow.Services;
using SketchNow.ViewModels;
using SketchNow.Views;

using Velopack;
using Velopack.Sources;

namespace SketchNow;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, ISingleInstance
{
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

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder)
                => configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                       .AddUserSecrets(typeof(App).Assembly))
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger()));

                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<SettingsViewModel>();

                services.AddSingleton<ISketchNowConfigurationService, SketchNowConfigurationService>();

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

    public void OnInstanceInvoked(string[] args) { }
#if !DEBUG
    public App()
    {
        Startup += Application_Startup;
        Exit += Application_Exit;
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

            Log.Error(e.Exception, "UI thread meets an exception");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"UI thread meets a fatal exception! {ex.Message}{Environment.NewLine}{ex.StackTrace}",
                "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);

            Log.Error(ex, "UI thread meets a fatal exception");
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

        Log.Error(e.ExceptionObject as Exception, "Non-UI thread meets exception");
    }

    static void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        MessageBox.Show(
            $"Task thread meets exception：{e.Exception.Message}{Environment.NewLine}{e.Exception.StackTrace}",
            "Unobserved Task", MessageBoxButton.OK, MessageBoxImage.Error);

        Log.Error(e.Exception, "Task thread meets exception");
        e.SetObserved();
    }
#endif
}