using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;

namespace SketchNow.Input.StylusPlugIns;

public sealed class CustomStylusPlugin : StylusPlugIn
{

    /// <summary>
    /// Gets or sets a value between 0 and 1 that reflects the amount of pressure the stylus applies to the digitizer's surface when the <see cref="StylusPoint" /> is created.
    /// </summary>
    /// <value>
    /// The pressure factor.
    /// </value>
    /// <returns>Value between 0 and 1 indicating the amount of pressure the stylus applies to the digitizer's surface as the <see cref="StylusPoint" /> is created.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The <see cref="StylusPoint.PressureFactor" /> property is set to a value less than 0 or greater than 1.</exception>
    public float PressureFactor
    {
        get
        {
            return field > 1.0 ? 1f : field < 0.0 ? 0.0f : field;
        }
        set
        {
            field = value >= 0.0 && value <= 1.0
                ? value
                : throw new ArgumentOutOfRangeException(nameof(PressureFactor),
                    Properties.Resource.InvalidPressureValue);
        }
    }
    protected override void OnStylusDown(RawStylusInput rawStylusInput)
    {
        // Call the base class before modifying the data.
        base.OnStylusDown(rawStylusInput);

        Filter(rawStylusInput);
    }

    protected override void OnStylusMove(RawStylusInput rawStylusInput)
    {
        // Call the base class before modifying the data.
        base.OnStylusMove(rawStylusInput);

        Filter(rawStylusInput);
    }

    protected override void OnStylusUp(RawStylusInput rawStylusInput)
    {
        // Call the base class before modifying the data.
        base.OnStylusUp(rawStylusInput);

        Filter(rawStylusInput);
    }

    private void Filter(RawStylusInput rawStylusInput)
    {
        // Get the StylusPoints that have come in.
        StylusPointCollection stylusPoints = rawStylusInput.GetStylusPoints();

        // Modify the (X,Y) data
        for (int i = 0; i < stylusPoints.Count; i++)
        {
            StylusPoint sp = stylusPoints[i];
            sp.PressureFactor = PressureFactor;
            stylusPoints[i] = sp;
        }

        // Copy the modified StylusPoints back to the RawStylusInput.
        rawStylusInput.SetStylusPoints(stylusPoints);
    }
}