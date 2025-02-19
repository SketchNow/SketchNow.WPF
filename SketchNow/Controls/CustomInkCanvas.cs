using System.Windows.Controls;

using SketchNow.Input.StylusPlugIns;

namespace SketchNow.Controls;

public class CustomInkCanvas : InkCanvas
{
    private readonly CustomStylusPlugin _customStylusPlugin = new();

    public CustomInkCanvas()
    {
        StylusPlugIns.Add(_customStylusPlugin);
    }
}