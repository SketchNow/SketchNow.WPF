using SketchNow.ViewModels;

namespace SketchNow.Tests;

//This attribute generates tests for MainWindowViewModel that
//asserts all constructor arguments are checked for null
[ConstructorTests(typeof(MainWindowViewModel))]
public partial class MainWindowViewModelTests
{
    /// <summary>Specifies the editing mode for the <see cref="T:System.Windows.Controls.InkCanvas" />.</summary>
    public enum InkCanvasEditingMode
    {
        /// <summary>Indicates that no action is taken when the pen sends data to the <see cref="T:System.Windows.Controls.InkCanvas" />.</summary>
        None,

        /// <summary>Indicates that ink appears on the <see cref="T:System.Windows.Controls.InkCanvas" /> when the pen sends data to it.</summary>
        Ink,

        /// <summary>Indicates that the <see cref="T:System.Windows.Controls.InkCanvas" /> responds to gestures, and does not receive ink.</summary>
        GestureOnly,

        /// <summary>Indicates that the <see cref="T:System.Windows.Controls.InkCanvas" /> responds to gestures, and receives ink.</summary>
        InkAndGesture,

        /// <summary>Indicates that the pen selects strokes and elements on the <see cref="T:System.Windows.Controls.InkCanvas" />.</summary>
        Select,

        /// <summary>Indicates that the pen erases part of a stroke when the pen intersects the stroke.</summary>
        EraseByPoint,

        /// <summary>Indicates that the pen erases an entire stroke when the pen intersects the stroke.</summary>
        EraseByStroke,
    }


    [Fact]
    public void AddPageCommand_Execute_IncrementsCount()
    {
        //Arrange
        AutoMocker mocker = new();

        MainWindowViewModel viewModel = mocker.CreateInstance<MainWindowViewModel>();

        int initialCount = viewModel.CanvasPages.Pages.Count;

        //Act
        viewModel.CanvasPages.AddPageCommand.Execute(null);

        //Assert
        Assert.Equal(initialCount + 1, viewModel.CanvasPages.Pages.Count);
    }
    
    
    [Fact]
    public void NextCommand_CanExecute_When_At_The_End_Of_The_Pages()
    {
        //Arrange
        AutoMocker mocker = new();

        MainWindowViewModel viewModel = mocker.CreateInstance<MainWindowViewModel>();

        //Act
        bool canExecute = viewModel.CanvasPages.NextCommand.CanExecute(null);

        //Assert
        Assert.False(canExecute);
    }
    
    [Fact]
    public void PreviousCommand_CanExecute_When_At_The_Head_Of_The_Pages()
    {
        //Arrange
        AutoMocker mocker = new();

        MainWindowViewModel viewModel = mocker.CreateInstance<MainWindowViewModel>();

        //Act
        bool canExecute = viewModel.CanvasPages.NextCommand.CanExecute(null);

        //Assert
        Assert.False(canExecute);
    }


    [Theory]
    [InlineData(0, (int)InkCanvasEditingMode.None)]
    [InlineData(1, (int)InkCanvasEditingMode.Ink)]
    [InlineData(2, (int)InkCanvasEditingMode.EraseByStroke)]
    [InlineData(3, (int)InkCanvasEditingMode.Select)]
    [InlineData(4, (int)InkCanvasEditingMode.None)]
    [InlineData(5, (int)InkCanvasEditingMode.None)]
    public void SelectedToolIndexRelatedSelectedEditingModeIsRight(int index, int expected)
    {
        //Arrange
        AutoMocker mocker = new();

        MainWindowViewModel viewModel = mocker.CreateInstance<MainWindowViewModel>();

        viewModel.SelectedToolIndex = index;

        //Act
        var actual = (int)viewModel.SelectedEditingMode;

        //Assert
        Assert.Equal(expected, actual);
    }
}