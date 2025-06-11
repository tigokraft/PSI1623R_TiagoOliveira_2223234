using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using System;
using System.Windows.Media;
using WpfAxis = LiveCharts.Wpf.Axis;
using WpfSeparator = LiveCharts.Wpf.Separator;
using WpfBrushes = System.Windows.Media.Brushes;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace login.Helpers
{
    internal class Charts
    {
        public LiveCharts.WinForms.CartesianChart SetupChart()
        {
            var cartesianChart = new LiveCharts.WinForms.CartesianChart
            {
                Size = new System.Drawing.Size(360, 465),
                Location = new System.Drawing.Point(0, 0),
                // Keep this as a slightly different shade if you want the chart area defined
                BackColor = System.Drawing.Color.FromArgb(20, 20, 20), // This is the chart's specific background
                LegendLocation = LiveCharts.LegendLocation.Top,
                ForeColor = System.Drawing.Color.Black,
            };

            // ... (rest of your chart setup code for gradients, series, axes) ...
            // (The colors for lines, fills, labels, and grid lines should remain as we refined them previously)

            var incomeGradient = new System.Windows.Media.LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0.5, 0),
                EndPoint = new System.Windows.Point(0.5, 1),
                GradientStops = new System.Windows.Media.GradientStopCollection
                {
                    new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromArgb(180, 50, 220, 180), 0),
                    new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromArgb(0, 40, 180, 140), 1)
                }
            };

            var expensesGradient = new System.Windows.Media.LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0.5, 0),
                EndPoint = new System.Windows.Point(0.5, 1),
                GradientStops = new System.Windows.Media.GradientStopCollection
                {
                    new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromArgb(180, 220, 100, 90), 0),
                    new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromArgb(0, 180, 80, 70), 1)
                }
            };

            cartesianChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Income",
                    Values = new ChartValues<double> { 0, 90, 130, 160, 180, 200, 190, 210, 205, 220 },
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 50, 220, 180)),
                    Fill = incomeGradient,
                    PointGeometry = null,
                    StrokeThickness = 2,
                    LineSmoothness = 1
                },
                new LineSeries
                {
                    Title = "Expenses",
                    Values = new ChartValues<double> { 0, 30, 50, 80, 95, 110, 100, 120, 115, 130 },
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 220, 100, 90)),
                    Fill = expensesGradient,
                    PointGeometry = null,
                    StrokeThickness = 2,
                    LineSmoothness = 1
                }
            };

            cartesianChart.AxisX.Add(new WpfAxis
            {
                Labels = new[] { "Apr 1", "", "", "", "", "24", "", "", "" },
                Foreground = WpfBrushes.LightGray,
                Separator = new WpfSeparator
                {
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(40, 128, 128, 128)),
                    StrokeThickness = 1,
                    IsEnabled = true
                }
            });

            cartesianChart.AxisY.Add(new WpfAxis
            {
                LabelFormatter = value => $"${value}",
                Foreground = WpfBrushes.LightGray,
                Separator = new WpfSeparator
                {
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(40, 128, 128, 128)),
                    StrokeThickness = 1,
                    IsEnabled = true
                },
                MinValue = 0
            });

            return cartesianChart;
        }
    }
}