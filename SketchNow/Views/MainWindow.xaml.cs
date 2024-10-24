using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using SketchNow.ViewModels;

namespace SketchNow.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
        
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, OnClose));
    }

    private void OnClose(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox box)
        {
            return;
        }

        switch (box.SelectedIndex)
        {
            case 0:
                Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                inkCanvas.UseCustomCursor = false;
                break;
            case 1:
                Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                inkCanvas.UseCustomCursor = true;
                inkCanvas.Cursor = Cursors.Pen;
                break;
            default:
                Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                inkCanvas.UseCustomCursor = false;
                break;
        }
    }
}
