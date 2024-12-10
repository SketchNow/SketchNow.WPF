using System.Windows.Controls;

using SketchNow.Input.StylusPlugIns;

namespace SketchNow.Controls;

public class CustomInkCanvas : InkCanvas
{
    CustomStylusPlugin _customStylusPlugin = new();

    public CustomInkCanvas()
    {
        this.StylusPlugIns.Add(_customStylusPlugin);
    }
}