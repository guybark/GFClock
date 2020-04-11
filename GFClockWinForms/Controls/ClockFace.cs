using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Automation;

namespace GFClockWinForms.Controls
{
    public class ClockFace : UserControl, IAutomationLiveRegion
    {
        // Barker Todo: Enable resizing of the app.
        private Size clockFaceSize = new Size(400, 400);

        private int xOffsetBigHand = 186;
        private int yOffsetBigHand = 44;
        private int xOffsetSmallHand = 185;
        private int yOffsetSmallHand = 93;

        private Label TimeStatus;

        private Image imageClockFace;
        private Image imageBigHand;
        private Image imageSmallHand;

        private double angleMinute;
        private double angleHour;

        private string currentTime;
        public string CurrentTime { get => currentTime; set => currentTime = value; }

        // Barker Note: Don't use PictureBoxes. Can't get them out of the UIA tree,
        // and don't need them anyway. If needed, I would have lived with the Pane,
        // and not to the IREP* thing.

        public ClockFace()
        {
            this.Dock = DockStyle.Top;
            this.Width = clockFaceSize.Width;

            // Note that setting IsAccessible seems to have not effect on the 
            // accessibility of the controls once exposed through UIA.

            this.AutoSize = true;

            this.Paint += ClockFace_Paint;

            this.TimeStatus = new System.Windows.Forms.Label();

            // AutoSize enables the TextBlock to grow in height when multiple
            // lines are required to show the text. Note that the width of the
            // TextBlock is no under the control of WinForms, and setting it
            // explicitly in code-behind will have no effect.
            this.TimeStatus.AutoSize = true;

            // If instead, we'd want the TimeStatus label to be a single row
            // with an ellipsis if required, leave AutoSize false and set 
            // AutoEllipsis true.
            //this.TimeStatus.AutoEllipsis = true;

            this.TimeStatus.Location = new System.Drawing.Point(0, clockFaceSize.Height);
            this.TimeStatus.MaximumSize = new System.Drawing.Size(clockFaceSize.Width, 0);
            this.TimeStatus.Name = "TimeStatus";

            // Set some arbitrary padding.
            this.TimeStatus.Padding = new System.Windows.Forms.Padding(40, 12, 40, 12);

            // Note: Don't increase the font size of the whole form,
            // as this can impact the size of controls whose role is
            // other than showing text. So only apply the text scaling
            // to the label controls in the app.

            var factor = NativeMethods.GetTextScaling();
            Debug.WriteLine("Barker: Current \"Make Text Bigger\" setting is " + factor);

            TimeStatus.Font = new Font(
                TimeStatus.Font.FontFamily,
                (float)factor * TimeStatus.Font.Size);

            this.TimeStatus.Width = clockFaceSize.Width;

            using (Graphics g = this.CreateGraphics())
            {
                var points = this.TimeStatus.Font.SizeInPoints;
                this.TimeStatus.Height = 
                    (int)(points * g.DpiY / 72) +
                    this.TimeStatus.Padding.Top + this.TimeStatus.Padding.Bottom;
            }

            this.TimeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.TimeStatus.SizeChanged += TimeStatus_SizeChanged;
            
            this.Controls.Add(TimeStatus);

            UseThemeColors();

            // Update the clock visuals every 1 second. This level of accuracy
            // is fine for this demo app.
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // Provide custom accessibility through our own AccessibleObject.
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ClockAccessibleObject(this);
        }

        public AutomationLiveSetting LiveSetting
        {
            get
            {
                // Make this an Aseretive LiveRegion.
                return AutomationLiveSetting.Assertive;
            }
            set
            {
                // For this demo code, the setting is fixed.
            }
        }

        private void ClockFace_Paint(object sender, PaintEventArgs e)
        {
            // Barker Todo: Suppress the refresh until all painting's done.
            var gfx = e.Graphics;

            gfx.DrawImage(imageClockFace, 0, 0, 400, 400);

            var rotatedBigHandImage = RotateImage(imageBigHand, angleMinute, xOffsetBigHand, yOffsetBigHand, 0.6f);
            gfx.DrawImage(rotatedBigHandImage, 0, 0);

            var rotatedSmallHandImage = RotateImage(imageSmallHand, angleHour, xOffsetSmallHand, yOffsetSmallHand, 0.4f);
            gfx.DrawImage(rotatedSmallHandImage, 0, 0);
        }

        public Image RotateImage(
            Image imgHand,
            double rotationAngle,
            int xOffset,
            int yOffset,
            float scale)
        {
            // Barker Todo: Tidy this up to avoid the use of two bitmaps here.

            Bitmap bmpMove = new Bitmap(clockFaceSize.Width, clockFaceSize.Height);
            Graphics gfxMove = Graphics.FromImage(bmpMove);

            // Move and scale to put the hand at the center of the clock face.
            gfxMove.TranslateTransform(xOffset, yOffset);
            gfxMove.ScaleTransform(scale, scale);
            gfxMove.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfxMove.DrawImage(imgHand, new Point(0, 0));
            gfxMove.Dispose();

            Bitmap bmpRotate = new Bitmap(clockFaceSize.Width, clockFaceSize.Height);
            Graphics gfxRotate = Graphics.FromImage(bmpRotate);

            // Now rotate the image around its pin.
            gfxRotate.TranslateTransform(clockFaceSize.Width / 2, clockFaceSize.Height / 2);
            gfxRotate.RotateTransform((float)rotationAngle);
            gfxRotate.TranslateTransform(-clockFaceSize.Width / 2, -clockFaceSize.Height / 2);
            gfxRotate.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfxRotate.DrawImage(bmpMove, new Point(0, 0));
            gfxRotate.Dispose();

            return bmpRotate;
        }

        private void TimeStatus_SizeChanged(object sender, EventArgs e)
        {
            // Center the TimeStatus label in the Clock control. Trying to do this
            // through docking does seem possible given the need to AutoSize the
            // Clock control.
            var label = (sender as Label);
            label.Left = (this.Width - label.Width) / 2;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SetClockHands();
        }

        // Update the clock hands to reflect the current time.
        public void SetClockHands()
        {
            int iMinutes = DateTime.Now.Minute;
            int iHours = DateTime.Now.Hour;
            int iSeconds = DateTime.Now.Second;

            // Has the hour:minute time value changed?
            var newTimeValue = iHours.ToString("D2") + ":" + iMinutes.ToString("D2");
            if (CurrentTime != newTimeValue)
            {
                angleMinute = (iMinutes * 6) + ((double)iSeconds / 10.0);
                angleHour = ((iHours % 12) * 30) + ((double)iMinutes / 2.0);

                // Paint the clock hands in their new position.
                this.Refresh();

                // Let assistive technologies like screen readers know 
                // that the programmatic value of the clock has changed.

                var oldTimeValue = CurrentTime;
                CurrentTime = newTimeValue;

                // Raise a ValuePropertyChanged event. Note that a screen reader may choose
                // not to react to the event, but AIWin can still be used to verify that the
                // event is being raised as it should be.

                NativeMethods.NotifyWinEvent(
                    0x800E, // EVENT_OBJECT_VALUECHANGE
                    this.Handle,
                    NativeMethods.OBJID_CLIENT,
                    NativeMethods.CHILDID_SELF);

                // Now update the textual time-related status.
                TimeStatus.Text = newTimeValue;

                // And while we're here, simulate a reminder at 13:00pm.
                if (CurrentTime == "13:00")
                {
                    TimeStatus.Text = "It's one o'clock and time for lunch.";

                    // Raise a LiveRegionChanged event. Note that WinForms also 
                    // makes it straightforward to raise a Notification event.
                    this.AccessibilityObject.RaiseLiveRegionChanged();
                }
            }
        }

        // Set up the colors and images to be shown based on whethr
        // a high contrast theme is active.
        public void UseThemeColors()
        {
            var highContrastThemeActive = SystemInformation.HighContrast;

            Debug.WriteLine("Barker: UseThemeColors, high contrast is " + highContrastThemeActive);

            // Barker Todo: Add the hands to the app!

            if (highContrastThemeActive)
            {
                TimeStatus.BackColor = SystemColors.Window;
                TimeStatus.ForeColor = SystemColors.WindowText;

                // If the background for text with this hgh contrast theme is light,
                // select the dark-on-light clock face image. Otherwise select the
                // light-on-dark version.
                if (TimeStatus.BackColor.GetBrightness() > 0.5)
                {
                    imageClockFace = global::GFClockWinForms.Properties.Resources.ClockFace_DarkOnLight;
                    imageBigHand = global::GFClockWinForms.Properties.Resources.BigHand_DarkOnLight;
                    imageSmallHand = global::GFClockWinForms.Properties.Resources.SmallHand_DarkOnLight;
                }
                else
                {
                    imageClockFace = global::GFClockWinForms.Properties.Resources.ClockFace_LightOnDark;
                    imageBigHand = global::GFClockWinForms.Properties.Resources.BigHand_LightOnDark;
                    imageSmallHand = global::GFClockWinForms.Properties.Resources.SmallHand_LightOnDark;
                }
            }
            else
            {
                TimeStatus.BackColor = Color.Gold;
                TimeStatus.ForeColor = Color.DarkBlue;

                imageClockFace = global::GFClockWinForms.Properties.Resources.ClockFace;
                imageBigHand = global::GFClockWinForms.Properties.Resources.BigHand;
                imageSmallHand = global::GFClockWinForms.Properties.Resources.SmallHand;
            }

            // Given that the TimeStatus lable is autosizing, its width won't necessarily 
            // be as wide at the Clock control, and as such, the background of the Clock
            // Control may be visible outside of the label. So set the background color 
            // of the Clock control to be the same as that of the label.
            this.BackColor = TimeStatus.BackColor;
        }

        // A custom AccessibleObject for the Clock control.
        public class ClockAccessibleObject : ControlAccessibleObject
        {
            ClockFace clockFace;

            public ClockAccessibleObject(ClockFace ctrl) : base(ctrl)
            {
                clockFace = ctrl;
            }

            public override string Name
            {
                get
                {
                    // Barker Todo: Localize this!
                    return "Grandfather Clock";
                }
            }

            // Some related documentation is at:
            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.accessibleobject?view=netframework-4.8

            public override AccessibleRole Role
            {
                get
                {
                    // Setting AccessibleRole.Clock seemed like a good idea, but
                    // in fact by doing this the control gets exposed through UIA
                    // as a Button, so this doesn't seem helpful to the customer.
                    // So expose this as an image instead.

                    return AccessibleRole.Graphic;
                }
            }

            // Note that this gets exposed through UIA's Value pattern
            // as not being read-only.

            public override string Value
            {
                get
                {
                    return clockFace.CurrentTime;
                }
            }

            // Have GetChildCount return zero does not impact the UIA tree.
            // Note: Despite this, some screen readers presumably may react 
            // as expected.
        }
    }

    internal class NativeMethods
    {
        [DllImport("GFClockWinFormsNativeGetTextScaling.dll", EntryPoint = "GetTextScaling")]
        public static extern double GetTextScaling();

        [DllImport("user32.dll")]
        public static extern void NotifyWinEvent(int winEvent, IntPtr hwnd, int objType, int objID);

        // From winuser.h.
        public const int CHILDID_SELF = 0;
        public const int OBJID_CLIENT = (unchecked((int)0xFFFFFFFC));
        public const int EVENT_OBJECT_VALUECHANGE = 0x800E;
    }
}
