using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;

namespace SketchNow.Input.StylusPlugIns;

public class CustomStylusPlugin : StylusPlugIn
{
    protected override void OnStylusDown(RawStylusInput rawStylusInput)
    {
        // Call the base class before modifying the data.
        base.OnStylusDown(rawStylusInput);

        rawStylusInput.SetStylusPoints(rawStylusInput.GetStylusPoints());
    }

    protected override void OnStylusMove(RawStylusInput rawStylusInput)
    {
        // Call the base class before modifying the data.
        base.OnStylusMove(rawStylusInput);

        rawStylusInput.SetStylusPoints(rawStylusInput.GetStylusPoints());
    }

    protected override void OnStylusUp(RawStylusInput rawStylusInput)
    {
        // Call the base class before modifying the data.
        base.OnStylusUp(rawStylusInput);

        rawStylusInput.SetStylusPoints(rawStylusInput.GetStylusPoints());
    }
}