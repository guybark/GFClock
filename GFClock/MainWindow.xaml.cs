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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

// Barker: Enable sizing of the app, and so remove all hard-code sizes and margins.

namespace GFClock
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Always position the clock near the top right on the primary monitor.
 
            // Barker: Only do this on first run. After that, remember where the 
            // customer moved the window to.
            int iOffset = 32;

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width  - iOffset;
            this.Top  = iOffset;

            ClockFace.Focus();
        }

        private void ClockFace_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Enable a drag of the clock from anywhere on the clock face.
            DragMove();
        }
    }

    // The ClockFace control everything going on in the clock area.
    public class ClockFace : UserControl
    {
        private ClockHand bigHand;
        private ClockHand smallHand;
        public delegate void SetClockHandsUIDelegate();

        private string currentTime;
        public string CurrentTime { get => currentTime; set => currentTime = value; }

        public ClockFace()
        {
            this.Loaded += ClockFace_Loaded;
        }

        private void ClockFace_Loaded(object sender, RoutedEventArgs e)
        {
            bigHand = (ClockHand)GetDescendantFromName(this, "BigHand");
            smallHand = (ClockHand)GetDescendantFromName(this, "SmallHand");

            // Move the hands so they appear to be attached at the 
            // center of the clock face.
            bigHand.Margin = new Thickness(182, 29, 0, 0);
            smallHand.Margin = new Thickness(182, 63, 0, 0);

            SetClockHands();

            // Update the clock visuals every 1 second. This level of accuracy
            // is fine for this demo app.
            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

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

        void SetClockHands()
        {
            int iMinutes = DateTime.Now.Minute;
            int iHours = DateTime.Now.Hour % 24;

            // Move the hands to reflect the current time.
            int iSeconds = DateTime.Now.Second;
            double angleMinute = (iMinutes * 6) + ((double)iSeconds / 10.0);
            double angleHour = ((iHours % 12) * 30) + ((double)iMinutes / 2.0);

            bigHand.RenderTransform = new RotateTransform(angleMinute, 13, 153);
            smallHand.RenderTransform = new RotateTransform(angleHour, 13, 119);

            // Has the time value changed?
            var newTimeValue = iHours.ToString() + ":" + iMinutes.ToString();
            if (CurrentTime != newTimeValue)
            {
                var oldTimeValue = CurrentTime;
                CurrentTime = newTimeValue;

                // Let assistive technologies like screen readers know 
                // that the programmatic value of the clock has changed.
                var peer = FrameworkElementAutomationPeer.FromElement(this);
                if (peer != null)
                {
                    Debug.WriteLine("Barker: Raise ValuePropertyChanged event");

                    // Note that as things stand today, Narrator doesn't make an 
                    // announcement in response to this event, but NVDA does.
                    peer.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty,
                        oldTimeValue, CurrentTime);
                }
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetClockHandsUIDelegate fn = new SetClockHandsUIDelegate(SetClockHands);
            Dispatcher.BeginInvoke(fn, null);
        }

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

        protected override string GetNameCore()
        {
            ResourceManager rm = new ResourceManager("GFClock.Resource1", Assembly.GetExecutingAssembly());
            return rm.GetString("Grandfather");
        }

        // Semantically, the control is not an image, so override the 
        // ControlType to be custom given that there's no known UIA 
        // control type that's a good match. Note that today, it seems
        // that Narrator doesn't announce the Value associated with a
        // custom control, but NVDA does. So I'll be testing this all
        // out with NVDA.

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        // Give the control helpful localized control type to be announced
        // by a screen reader.

        protected override string GetLocalizedControlTypeCore()
        {
            ResourceManager rm = new ResourceManager("GFClock.Resource1", Assembly.GetExecutingAssembly());
            return rm.GetString("Clock");
        }

        // Add support the UIA Value pattern now.
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
        }
    }

    // A custom class for the clock hands.
    public class ClockHand : Image
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ClockHandAutomationPeer(this);
        }
    }

    public class ClockHandAutomationPeer : ImageAutomationPeer
    {
        public ClockHandAutomationPeer(Image owner)
            : base(owner)
        {
        }

        // Remove the UIA elements representing the clock hands from the 
        // UIA views which are meant to contain things of interest to the 
        // customer. The customer doesn't need to encounter the hands,
        // because the full time is accessible through the clock element.
        protected override bool IsControlElementCore()
        {
            return false;
        }

        protected override bool IsContentElementCore()
        {
            return false;
        }
    }

    // Converters to select the appropriate light-on-dark or dark-on-light images
    // when a high contrast theme is active.
    public class ClockFaceImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // To be here, we know a high contrast theme is active. So get the color for window backgrounds.
            var windowBackgroundBrush = SystemColors.WindowBrush;

            System.Drawing.Color windowBackgroundColor = System.Drawing.Color.FromArgb(
                0, windowBackgroundBrush.Color.R, windowBackgroundBrush.Color.G, windowBackgroundBrush.Color.B);

            string result;

            if (windowBackgroundColor.GetBrightness() > 0.5)
            {
                result = "/GFClock;component/Assets/ClockFace_DarkOnLight.png";
            }
            else
            {
                result = "/GFClock;component/Assets/ClockFace_LightOnDark.png";
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ClockHandImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // To be here, we know a high contrast theme is active. So get the color for window backgrounds.
            var windowBackgroundBrush = SystemColors.WindowBrush;

            System.Drawing.Color windowBackgroundColor = System.Drawing.Color.FromArgb(
                0, windowBackgroundBrush.Color.R, windowBackgroundBrush.Color.G, windowBackgroundBrush.Color.B);

            string result;

            if (windowBackgroundColor.GetBrightness() > 0.5)
            {
                result = ((string)parameter == "1" ? 
                    "/GFClock;component/Assets/BigHand_DarkOnLight.png" :
                    "/GFClock;component/Assets/SmallHand_DarkOnLight.png");
            }
            else
            {
                result = ((string)parameter == "1" ?
                    "/GFClock;component/Assets/BigHand_LightOnDark.png" :
                    "/GFClock;component/Assets/SmallHand_LightOnDark.png");
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
