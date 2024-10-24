using System.Windows.Controls;
using System.Windows.Input;

using SketchNow.ViewModels;

namespace SketchNow.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }
        
        private void TestInkCanvas_OnTouchDown(object? sender, TouchEventArgs e)
        {
            var touchPoint = e.GetTouchPoint(TestInkCanvas);
            TestInkCanvasDebugInfo.Text = $"X: {touchPoint.Position.X}, Y: {touchPoint.Position.Y}";
            TestInkCanvasDebugInfo.Text += ($"Height: {touchPoint.Bounds.Height}, Width: {touchPoint.Bounds.Width}");
        }
    }
}