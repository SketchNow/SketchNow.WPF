using System.Diagnostics;
using System.Windows.Input;

using SketchNow.ViewModels;
using SketchNow.Views;

namespace SketchNow;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow(
        MainWindowViewModel mainViewModel,
        SettingsViewModel settingsViewModel)
    {
        InitializeComponent();
        Debug.WriteLine("Ctor=============");
        
        DataContext = mainViewModel;
        SettingsView.DataContext = settingsViewModel;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, OnClose));
    }

    private void OnClose(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
    }
}
