using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace login.Helpers
{
    public partial class ShadcnDatePicker : UserControl
    {
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(ShadcnDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(ShadcnDatePicker), new PropertyMetadata("Pick a date"));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public ShadcnDatePicker()
        {
            InitializeComponent();
            UpdateText();
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ShadcnDatePicker)d).UpdateText();
        }

        private void UpdateText()
        {
            if (SelectedDate.HasValue)
            {
                DateTextBlock.Text = SelectedDate.Value.ToString("dd/MM/yyyy");
                DateTextBlock.Foreground = Brushes.White;
            }
            else
            {
                DateTextBlock.Text = Placeholder;
                DateTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A1A1AA"));
            }
        }

        private void DateButton_Click(object sender, RoutedEventArgs e)
        {
            CalendarPopup.IsOpen = true;
        }

        private void PopupCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PopupCalendar.SelectedDate.HasValue)
            {
                SelectedDate = PopupCalendar.SelectedDate.Value;
                CalendarPopup.IsOpen = false;
            }
        }
    }
}
