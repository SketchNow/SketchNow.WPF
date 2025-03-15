using SketchNow.Models;

namespace SketchNow.Services;

public class SketchNowConfigurationService : ISketchNowConfigurationService
{

    public SketchNowConfigurationService()
    {
        Settings = new SketchNowSettings
        {
            IsFitToCurve = Properties.Settings.Default.IsFitToCurve,
            IsIgnorePressure = Properties.Settings.Default.IsIgnorePressure,
            IsEraseByStroke = Properties.Settings.Default.IsEraseByStroke,
            SelectedBackground = Properties.Settings.Default.SelectedBackground
        };

        Settings.PropertyChanged += (_, _) => SaveSettings();
    }

    public SketchNowSettings Settings { get; set; }

    private void SaveSettings()
    {
        Properties.Settings.Default.IsFitToCurve = Settings.IsFitToCurve;
        Properties.Settings.Default.IsIgnorePressure = Settings.IsIgnorePressure;
        Properties.Settings.Default.IsEraseByStroke = Settings.IsEraseByStroke;
        Properties.Settings.Default.SelectedBackground = Settings.SelectedBackground;
        Properties.Settings.Default.Save();
    }
}

public interface ISketchNowConfigurationService
{
    public SketchNowSettings Settings { get; set; }
}
