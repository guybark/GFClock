using GFClock.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GFClock
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private ClockFace clockFace;

        public SettingsWindow(ClockFace clockFace)
        {
            InitializeComponent();

            this.clockFace = clockFace;

            this.ShowInTaskbar = false;

            ObservableCollection<ClockData> clockData = new ObservableCollection<ClockData>();
            clockData.Add(new ClockData(false, "Sundials", "2000 years ago", "Sundials"));
            clockData.Add(new ClockData(true, "Big Ben", "1859", "Big Ben"));
            clockData.Add(new ClockData(false, "Dad's Grandfather Clock", "1777", "GFClock"));
            clockData.Add(new ClockData(true, "Deep Space Atomic Clock", "2019", "NASA"));

            DetailsDataGrid.DataContext = clockData;

            this.Loaded += SettingsWindow_Loaded;

            Keyboard.AddKeyboardInputProviderAcquireFocusHandler(this, CheckRadioButton);

            this.Closing += SettingsWindow_Closing;
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Set focus to the first control in the window after the 
            // UI's been fully loaded.
            IntroLink.Focus();
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.clockFace.VisibleHandCount = ((bool)ShowBothHandsRadioButton.IsChecked ? 2 : 1);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CheckRadioButton(object sender, KeyboardInputProviderAcquireFocusEventArgs e)
        {
            if (e.FocusAcquired)
            {
                var radioButton = e.Source as RadioButton;
                if (radioButton != null)
                {
                    radioButton.IsChecked = true;
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {

        }

        private class ClockData
        {
            public ClockData(bool Status, string Name, string Date, string Details)
            {
                this.Status = Status;
                this.Name = Name;
                this.Date = Date;
                this.Details = Details;
            }

            public bool Status { get; set; }
            public string Name { get; set; }
            public string Date { get; set; }
            public string Details { get; set; }
        }
    }

    public class StatusToAccessibleNameConverter : IValueConverter​
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = (bool)value;
            return status ? "Running" : "Not running";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToImageConverter : IValueConverter​
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = (bool)value;
            return status ?
                new BitmapImage(new Uri(@"Assets/ClockFace.png", UriKind.Relative)) :
                new BitmapImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
