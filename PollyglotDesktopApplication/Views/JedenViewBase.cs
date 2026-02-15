using System.Windows;
using System.Windows.Controls;

namespace PollyglotDesktopApp.Views
{
    public class JedenViewBase : UserControl
    {
        static JedenViewBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(JedenViewBase),
                new FrameworkPropertyMetadata(typeof(JedenViewBase))
            );
        }
    }
}
