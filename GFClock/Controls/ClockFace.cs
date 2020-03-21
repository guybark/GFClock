using System;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Timers;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Media;

namespace GFClock.Controls
{
    // The ClockFace contains everything going on in the clock area.
    public class ClockFace : UserControl
    {
        private ClockPartImage bigHand;
        private ClockPartImage smallHand;
        private TextBlock timeStatus;

        // These offsets and rotation center value account for where the 
        // rotation origin appears to be in the images of the clock hands.
        // These rough values are fine for this demo app.

        // Barker Todo: Account for the app window and clock elements
        // being resizable in the future.

        private int xOffsetBigHand = 11;
        private int yOffsetBigHand = 49;
        private int xOffsetSmallHand = 11;
        private int yOffsetSmallHand = 83;
        private int xRotateCenterBigHand = 13;
        private int yRotateCenterBigHand = 153;
        private int xRotateCenterSmallHand = 13;
        private int yRotateCenterSmallHand = 119;

        public delegate void SetClockHandsUIDelegate();

        private string currentTime;
        public string CurrentTime { get => currentTime; set => currentTime = value; }

        public ClockFace()
        {
            this.Loaded += ClockFace_Loaded;
        }

        private void ClockFace_Loaded(object sender, RoutedEventArgs e)
        {
            bigHand = (ClockPartImage)GetDescendantFromName(this, "BigHand");
            smallHand = (ClockPartImage)GetDescendantFromName(this, "SmallHand");
            timeStatus = (TextBlock)GetDescendantFromName(this, "TimeStatus");

            // Move the hands so that they appears to be attached at the 
            // center of the clock face.
            var xOffsetWindow = this.Width / 2;
            bigHand.Margin = new Thickness(xOffsetWindow - xOffsetBigHand, yOffsetBigHand, 0, 0);
            smallHand.Margin = new Thickness(xOffsetWindow - xOffsetSmallHand, yOffsetSmallHand, 0, 0);

            // Set the hands to the current time.
            SetClockHands();

            // Update the clock visuals every 1 second. This level of accuracy
            // is fine for this demo app.
            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        // Helper function to get the clock hands from the clock face control.
        private FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            FrameworkElement element = null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                element = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (element != null)
                {
                    if (element.Name == name)
                    {
                        return element;
                    }

                    element = GetDescendantFromName(element, name);
                    if (element != null)
                    {
                        return element;
                    }
                }
            }

            return element;
        }

        // Update the clock hands to reflect the current time.
        void SetClockHands()
        {
            int iMinutes = DateTime.Now.Minute;
            int iHours = DateTime.Now.Hour;
            int iSeconds = DateTime.Now.Second;

            double angleMinute = (iMinutes * 6) + ((double)iSeconds / 10.0);
            double angleHour = ((iHours % 12) * 30) + ((double)iMinutes / 2.0);

            // Rotate the hands.
            bigHand.RenderTransform = new RotateTransform(angleMinute, xRotateCenterBigHand, yRotateCenterBigHand);
            smallHand.RenderTransform = new RotateTransform(angleHour, xRotateCenterSmallHand, yRotateCenterSmallHand);

            // Has the hour:minute time value changed?
            var newTimeValue = iHours.ToString("D2") + ":" + iMinutes.ToString("D2");
            if (CurrentTime != newTimeValue)
            {
                // Let assistive technologies like screen readers know 
                // that the programmatic value of the clock has changed.

                var oldTimeValue = CurrentTime;
                CurrentTime = newTimeValue;

                var peer = FrameworkElementAutomationPeer.FromElement(this);
                if (peer != null)
                {
                    Debug.WriteLine("Barker: Raise ValuePropertyChanged event");

                    // Note that as things stand today, Narrator doesn't make an 
                    // announcement in response to this event, but another screen 
                    // reader does.
                    peer.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty,
                        oldTimeValue, CurrentTime);
                }

                // Now update the textual time-related status.
                timeStatus.Text = newTimeValue;

                // And while we're here, simulate a reminder at 13:00pm.
                if (CurrentTime == "13:00")
                {
                    timeStatus.Text = "It's one o'clock and time for lunch.";

                    peer = FrameworkElementAutomationPeer.FromElement(timeStatus);
                    if (peer != null)
                    {
                        Debug.WriteLine("Barker: Raise LiveRegionChanged event");

                        // Always update the text value on The TextBlock before raising the event.
                        peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);

                        // If LiveRegions aren't available to the app, (for example, the 
                        // app is running on a machine where only .NET 4.6 Framework is 
                        // available), and an event for a screen reader must be announced, 
                        // consider using the NotificationTextBlock.
                        //
                        //timeStatus.RaiseNotificationEvent(
                        //  displayTimeStatus,
                        //   "2112" // Some abitrary number for this demo.
                        // );
                    }
                }
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetClockHandsUIDelegate fn = new SetClockHandsUIDelegate(SetClockHands);
            Dispatcher.BeginInvoke(fn, null);
        }

        // Customize the programmatic accessibility of this control 
        // by using a custom AutomationPeer.
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ClockFaceAutomationPeer(this);
        }
    }

    public class ClockFaceAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        private ClockFace clockFace;

        public ClockFaceAutomationPeer(ClockFace clockFace)
            : base(clockFace)
        {
            this.clockFace = clockFace;
        }

        // Give the control a localized, accurate, concise name which
        // uniquely identifies the clock element.
        protected override string GetNameCore()
        {
            ResourceManager rm = new ResourceManager(
                "GFClock.Resource1", Assembly.GetExecutingAssembly());
            return rm.GetString("Grandfather");
        }

        // Give the control a localized and accurate control type which 
        // conveys the type of the element.
        protected override string GetLocalizedControlTypeCore()
        {
            ResourceManager rm = new ResourceManager(
                "GFClock.Resource1", Assembly.GetExecutingAssembly());
            return rm.GetString("Clock");
        }

        // Add support the UIA Value pattern now in order for the current time
        // to be exposed as a value directly from this element.
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public string Value
        {
            get
            {
                Debug.WriteLine("Barker: Current Value is \"" + clockFace.CurrentTime + "\"");

                return clockFace.CurrentTime;
            }
        }

        public void SetValue(string strValue)
        {
            // This clock has a read-only value so can't be set here.
        }
    }
}
