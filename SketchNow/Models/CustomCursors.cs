using System.Windows;
using System.Windows.Input;

namespace SketchNow.Models;

public interface ICustomCursors
{
    Cursor Remove { get; }
    Cursor Circle { get; }
    Cursor Select { get; }
}

public class CustomCursors : ICustomCursors
{
    private static Cursor LoadCursor(string resourcePath)
    {
        var uri = new Uri($"/SketchNow;component/Resources/{resourcePath}", UriKind.Relative);
        var stream = Application.GetResourceStream(uri)?.Stream 
                     ?? throw new CursorNotFoundException(resourcePath);
        return new Cursor(stream);
    }

    public Cursor Remove { get; } = LoadCursor("Remove.cur");
    public Cursor Circle { get; } = LoadCursor("Circle.cur");
    public Cursor Select { get; } = LoadCursor("Select.cur");
}

public class CursorNotFoundException(string message) : Exception($"Cursor not found: {message}");
