using GFClockWinForms.Controls;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GFClockWinForms
{
    public partial class Form1 : Form
    {
        ClockFace clockFace = null;

        public Form1()
        {
            InitializeComponent();

            clockFace = new ClockFace();
            clockFace.Name = "ClockFace";
            clockFace.Location = new Point(0, 0);
            this.Controls.Add(clockFace);

            clockFace.SetClockHands();

            Microsoft.Win32.SystemEvents.UserPreferenceChanged
                   += new Microsoft.Win32.UserPreferenceChangedEventHandler(
                   this.UserPreferenceChanged);

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
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


        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    this.SizeChanged += Form1_SizeChanged;
        //}

        //private void Form1_SizeChanged(object sender, EventArgs e)
        //{
        //    // This only works because I've set MaximumSize to impose a max width.
        //    // No Dock, because then it disappears behind the picture.
        //    // I set AutoSize true in the propeties now. labelStatus.AutoSize = true;

        //    // Note cold seem to center label in the using the autosize/etc setting I was doing.
        //    TimeStatus.Left = (this.ClientSize.Width - TimeStatus.Width) / 2;
        //}

    }
}
