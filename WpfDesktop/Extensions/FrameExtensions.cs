using System.Windows.Controls;
using System.Windows;

namespace WpfDesktop.Extensions
{
    public static class FrameExtensions
    {
        public static object GetDataContext(this Frame frame)
        {
            if (frame.Content is FrameworkElement element)
            {
                return element.DataContext;
            }

            return null;
        }

        public static void CleanNavigation(this Frame frame)
        {
            while (frame.CanGoBack)
            {
                frame.RemoveBackEntry();
            }
        }
    }
}
