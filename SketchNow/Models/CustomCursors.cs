using System.Windows;
using System.Windows.Input;

namespace SketchNow.Models;

public class CustomCursors
{
    public Cursor Remove { get; } = new(Application.GetResourceStream(new Uri(@"Resources\Remove.cur", UriKind.Relative))?.Stream ?? throw new CursorNotFoundException(@"Resources\Remove.cur"));
    public Cursor Circle { get; } = new(Application.GetResourceStream(new Uri(@"Resources\Circle.cur", UriKind.Relative))?.Stream ?? throw new InvalidOperationException(@"Resources\Circle.cur"));
    public Cursor Select { get; } = new(Application.GetResourceStream(new Uri(@"Resources\Select.cur", UriKind.Relative))?.Stream ?? throw new InvalidOperationException(@"Resources\Select.cur"));
}
public class CursorNotFoundException(string message) : Exception(message);
