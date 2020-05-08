using GFClockWinForms.Controls;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace GFClockWinForms
{
    public partial class Form1 : Form
    {
        private ClockFace clockFace = null;
        private float makeEverythingBiggerScaling;

        public Form1()
        {
            InitializeComponent();

            // Support the clock face being rather small.
            this.MinimumSize = new Size(200, 200);

            var hMon = NativeMethods.MonitorFromWindow(this.Handle, NativeMethods.MONITOR_DEFAULTTONEAREST);
            NativeMethods.MONITOR_DPI_TYPE monDpiType = NativeMethods.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI;

            int dpiX;
            int dpiY;
            NativeMethods.GetDpiForMonitor(hMon, monDpiType, out dpiX, out dpiY);
            makeEverythingBiggerScaling = ((float)dpiY / DeviceDpi);

            // The Clock control has a Dock of Fill, as that makes it easy 
            // for it to be autosized as the window size changes. But that
            // also means the control occupies the same space as the menu.
            // So always offset the Clock visuals to be below the menu.
            this.clockFace = new ClockFace(this.menuStrip1.Bottom, makeEverythingBiggerScaling);
            this.Controls.Add(clockFace);

            this.clockFace.SetClockHands();

            Microsoft.Win32.SystemEvents.UserPreferenceChanged
                   += new Microsoft.Win32.UserPreferenceChangedEventHandler(
                   this.UserPreferenceChanged);
        }

        public void UserPreferenceChanged(
            object sender,
            Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            // Note that not only do we end up here even when switching 
            // between the active state of high contrast, but also when
            // switching between high contrast themes, which is nice.
            this.clockFace.UseThemeColors();
        }

        private void closeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void settingsToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            var settingsForm = new SettingsForm(this.clockFace, this.makeEverythingBiggerScaling);
            settingsForm.ShowDialog(this);

            this.clockFace.RedrawClockFace(true, this.clockFace.CreateGraphics());
        }
    }
}
