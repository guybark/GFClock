using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

// Barker: Enable sizing of the app, and so remove all hard-code sizes and margins.

namespace GFClock
{
    public partial class MainWindow : Window
    {
        private IntPtr _windowHandle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClockFace_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Enable a drag of the clock from anywhere on the clock face.
            DragMove();
        }

        private void ClockFace_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClockFace.ResizeHands();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(ClockFace);
            settingsWindow.ShowDialog();

            BigHand.Visibility = (ClockFace.VisibleHandCount > 1 ?
                Visibility.Visible : Visibility.Collapsed);
        }
    }

    public class NativeWin32
    {
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public const int GWL_STYLE = -16;
        public const int WS_MINIMIZEBOX = 0x20000;
    }
}
