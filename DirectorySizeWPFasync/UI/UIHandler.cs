using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace DirectorySizeWPFasync.UI
{
    internal static class UIHandler
    {
        public static SolidColorBrush PaleGreen;
        public static SolidColorBrush PaleRed;

        public static void Execute(Action action)
        {
            //all interface operations must be executed on the UI thread
            Application.Current.Dispatcher.Invoke(action, DispatcherPriority.Background);
        }

        //method must be executed on the UI thread
        public static void SetBrushes()
        {
            PaleGreen = new SolidColorBrush(Color.FromRgb(167, 214, 167));
            PaleRed = new SolidColorBrush(Color.FromRgb(214, 167, 167));
        }
    }
}