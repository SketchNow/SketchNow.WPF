using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MaterialDesignThemes.Wpf;

namespace SketchNow.Controls
{
    /// <summary>
    /// IconWithText.xaml 的交互逻辑
    /// </summary>
    public partial class IconWithText : UserControl
    {
        public IconWithText()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register("Kind", typeof(PackIconKind), typeof(IconWithText), new PropertyMetadata(PackIconKind.None));

        public PackIconKind Kind
        {
            get { return (PackIconKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(IconWithText), new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static new readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register("ToolTiip", typeof(string), typeof(IconWithText), new PropertyMetadata(string.Empty));
        public new string ToolTip
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
