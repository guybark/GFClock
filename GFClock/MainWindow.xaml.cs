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

            this.SourceInitialized += MainWindow_SourceInitialized;

            // Always position the clock near the top right on the primary monitor.

            // Barker Todo: Only do this on first run. After that, remember 
            // where the customer moved the window to.
            int iOffset = 32;

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width  - iOffset;
            this.Top  = iOffset;

            // Automatically resize height and width relative to content.
            this.SizeToContent = SizeToContent.WidthAndHeight;

            ClockFace.Focus();
        }

        // Add the Minimize button back onto the app caption bar after it
        // got removed through my use of ResizeMode="NoResize" in the XAML.
        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            _windowHandle = new WindowInteropHelper(this).Handle;

            EnableMinimizeButton();
        }

        protected void EnableMinimizeButton()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                var style = NativeWin32.GetWindowLong(
                    _windowHandle,
                    NativeWin32.GWL_STYLE);

                NativeWin32.SetWindowLong(
                    _windowHandle,
                    NativeWin32.GWL_STYLE,
                    style | NativeWin32.WS_MINIMIZEBOX);
            }
        }

        private void ClockFace_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Enable a drag of the clock from anywhere on the clock face.
            DragMove();
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
