using System;
using System.Windows;
using System.Windows.Data;

namespace GFClock
{
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

            // If the background for text with this hgh contrast theme is light,
            // select the dark-on-light clock face image. Otherwise select the
            // light-on-dark version.
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
