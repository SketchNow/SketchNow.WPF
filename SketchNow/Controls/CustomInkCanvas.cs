using System.Windows.Controls;

using SketchNow.Input.StylusPlugIns;

namespace SketchNow.Controls;

public sealed class CustomInkCanvas : InkCanvas
{
    private readonly CustomStylusPlugin _customStylusPlugin = new() { PressureFactor = 0.1f };

    public CustomInkCanvas()
    {
        StylusPlugIns.Add(_customStylusPlugin);
    }

    /// <summary>
    /// Changes the pressure factor.
    /// </summary>
    /// <param name="pressureFactor">The pressure factor.</param>
    /// <exception cref="ArgumentException">Pressure factor must be between 0 and 1</exception>
    public void ChangePressureFactor(float pressureFactor)
    {
        if (pressureFactor < 0.0 || pressureFactor > 1.0)
        {
            throw new ArgumentException("Pressure factor must be between 0 and 1");
        }

        StylusPlugIns.Remove(_customStylusPlugin);
        _customStylusPlugin.PressureFactor = pressureFactor;
        StylusPlugIns.Add(_customStylusPlugin);
    }
}