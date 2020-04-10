using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GFClockWinForms.Controls
{
    public class ClockFace : UserControl
    {
        private string currentTime;
        public string CurrentTime { get => currentTime; set => currentTime = value; }

        private Label TimeStatus;

        private int xOffsetBigHand = 186;
        private int yOffsetBigHand = 44;
        private int xOffsetSmallHand = 185;
        private int yOffsetSmallHand = 93;

        private Size clockFaceSize = new Size(400, 400);

        private double angleMinute;
        private double angleHour;

        private Image imageClockFace;
        private Image imageBigHand;
        private Image imageSmallHand;

        // Barker Note: Don't use PictureBoxes. Can't get them out of the UIA tree,
        // and don't need them anyway. If needed, I would have lived with the Pane,
        // and not to the IREP* thing.

        public ClockFace()
        {
            // Note that setting IsAccessible seems to have not effect on the 
            // accessibility of the controls once exposed through UIA.

            this.AutoSize = true;

            this.Paint += ClockFace_Paint;

            this.TimeStatus = new System.Windows.Forms.Label();
            this.TimeStatus.AutoSize = true;
            this.TimeStatus.Location = new System.Drawing.Point(0, 400);
            this.TimeStatus.Margin = new System.Windows.Forms.Padding(0);
            this.TimeStatus.MaximumSize = new System.Drawing.Size(800, 0);
            this.TimeStatus.Name = "TimeStatus";
            this.TimeStatus.Padding = new System.Windows.Forms.Padding(6);
            this.TimeStatus.Size = new System.Drawing.Size(73, 37);
            this.TimeStatus.TabIndex = 0;
            this.TimeStatus.Text = "";
            this.TimeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.Controls.Add(TimeStatus);

            // Barker Todo; demo ellipis...
            //labelStatus.MaximumSize = new Size(0, 0);

            this.Width = clockFaceSize.Width;
            TimeStatus.Width = this.Width;

            var factor = NativeMethods.GetTextScaling();
            Debug.WriteLine("Barker: Current \"Make Text Bigger\" setting is " + factor);

            // Note: Don't increase the font size of the whole form,
            // as this can impact the size of controls whose role is
            // other than showing text. So only apply the text scaling
            // to the label controls in the app.

            TimeStatus.Font = new Font(
                TimeStatus.Font.FontFamily,
                (float)factor * TimeStatus.Font.Size);

            UseThemeColors();

            // Update the clock visuals every 1 second. This level of accuracy
            // is fine for this demo app.
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void ClockFace_Paint(object sender, PaintEventArgs e)
        {
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

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ClockAccessibleObject(this);
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
                angleHour = ((iHours % 12) * 30) + ((double) iMinutes / 2.0);

                this.Refresh();

                // Let assistive technologies like screen readers know 
                // that the programmatic value of the clock has changed.

                var oldTimeValue = CurrentTime;
                CurrentTime = newTimeValue;

                // Raise a ValuePropertyChanged event. Note that a screen reader may choose
                // not to react to the event, but AIWin can still be used to verify that the
                // event is being raised as it should be.

                // Note that the following did not get a UIA Value property changed event raised.
                //NativeMethods.NotifyWinEvent(
                //    NativeMethods.UIA_ValueValuePropertyId,
                //    this.Handle, // 0, 0);
                //    NativeMethods.OBJID_CLIENT,
                //    NativeMethods.CHILDID_SELF);

                // Note that the following did not get a UIA Value property changed event raised.
                //this.AccessibilityNotifyClients(
                //    AccessibleEvents.ValueChange,
                //    NativeMethods.CHILDID_SELF);

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

        // High Contrast related.

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
        }

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
                    return "Clock";
                }
            }

            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.accessibleobject?view=netframework-4.8

            public override AccessibleRole Role
            {
                get
                {
                    // Barker: This seemed handy, but in fact the control gets exposed
                    // through UIA as a Button, so this doesn't seem helpful to the 
                    // customer.
                    //return AccessibleRole.Clock;
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
            // Note, some screen readers presumably would react as expected 
            // though.
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

        // From uiautomationclient.h.
        public const int UIA_ValueValuePropertyId = 30045;
    }
}
