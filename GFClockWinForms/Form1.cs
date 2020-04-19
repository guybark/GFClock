using GFClockWinForms.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace GFClockWinForms
{
    public partial class Form1 : Form
    {
        private ClockFace clockFace = null;

        public Form1()
        {
            InitializeComponent();

            // Support the clock face being rather small.
            this.MinimumSize = new Size(200, 200);

            clockFace = new ClockFace();
            this.Controls.Add(clockFace);

            clockFace.SetClockHands();

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
            clockFace.UseThemeColors();
        }
    }
}
