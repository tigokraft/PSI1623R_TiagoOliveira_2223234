using System;
using System.Windows;
using System.Windows.Controls;

namespace login.Helpers
{
    public partial class GunaStyleDatePicker : UserControl
    {
        public GunaStyleDatePicker()
        {
            InitializeComponent();
        }

        private void DateButton_Click(object sender, RoutedEventArgs e)
        {
            CalendarPopup.IsOpen = true;
        }

        private void PopupCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PopupCalendar.SelectedDate.HasValue)
            {
                DateButton.Content = PopupCalendar.SelectedDate.Value.ToString("dd/MM/yyyy");
                CalendarPopup.IsOpen = false;
            }
        }
    }
}
