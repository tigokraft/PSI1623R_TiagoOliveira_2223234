using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;

namespace login.Helpers
{
    internal class Charts
    {
        public Guna2Panel SetupIncomeChart(double[] incValues, string[] xLabels)
        {
            // 1) Container
            var container = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(16, 20, 20),
                BorderColor = Color.FromArgb(60, 60, 60),
                BorderThickness = 1,
                BorderRadius = 6
            };

            // 2) Chart
            var chart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(16, 20, 20),
                ZoomMode = ZoomAndPanMode.X | ZoomAndPanMode.Y
            };
            container.Controls.Add(chart);

            // 3) Axis limits
            double yMax = incValues.Length == 0 ? 1 : incValues.Max() * 1.1;
            double xMax = xLabels.Length - 1;

            // 4) Single series: Income
            chart.Series = new ISeries[]
            {
                new LineSeries<double>(incValues)
                {
                    Name          = "Income",
                    Fill          = new SolidColorPaint(new SKColor(50,220,180,60)),
                    Stroke        = new SolidColorPaint(new SKColor(50,220,180)) { StrokeThickness = 2 },
                    GeometrySize  = 0,
                    ZIndex        = 0
                }
            };

            // 5) X-axis
            chart.XAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit        = 0,
                    MaxLimit        = xMax,
                    Labels          = xLabels,
                    TextSize        = 10,
                    LabelsPaint     = new SolidColorPaint(SKColors.White),
                    LabelsRotation  = 45,
                    SeparatorsPaint = null,
                    TicksPaint      = new SolidColorPaint(new SKColor(255,255,255,80)) { StrokeThickness = 1 },
                    MinStep         = 1
                }
            };

            // 6) Y-axis
            chart.YAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit        = 0,
                    MaxLimit        = yMax,
                    TextSize        = 10,
                    LabelsPaint     = new SolidColorPaint(SKColors.White),
                    Labeler         = value => $"${value:0}",
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255,255,255,30)) { StrokeThickness = 1 },
                    TicksPaint      = null
                }
            };

            return container;
        }

        public Guna2Panel SetupExpenseChart(double[] expValues, string[] xLabels)
        {
            // 1) Container
            var container = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(16, 20, 20),
                BorderColor = Color.FromArgb(60, 60, 60),
                BorderThickness = 1,
                BorderRadius = 6
            };

            // 2) Chart
            var chart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(16, 20, 20),
                ZoomMode = ZoomAndPanMode.X | ZoomAndPanMode.Y
            };
            container.Controls.Add(chart);

            // 3) Axis limits
            double yMax = expValues.Length == 0 ? 1 : expValues.Max() * 1.1;
            double xMax = xLabels.Length - 1;

            // 4) Single series: Expense
            chart.Series = new ISeries[]
            {
                new LineSeries<double>(expValues)
                {
                    Name          = "Expenses",
                    Fill          = new SolidColorPaint(new SKColor(220,100,90,60)),
                    Stroke        = new SolidColorPaint(new SKColor(220,100,90)) { StrokeThickness = 2 },
                    GeometrySize  = 0,
                    ZIndex        = 0
                }
            };

            // 5) X-axis
            chart.XAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit        = 0,
                    MaxLimit        = xMax,
                    Labels          = xLabels,
                    TextSize        = 10,
                    LabelsPaint     = new SolidColorPaint(SKColors.White),
                    LabelsRotation  = 45,
                    SeparatorsPaint = null,
                    TicksPaint      = new SolidColorPaint(new SKColor(255,255,255,80)) { StrokeThickness = 1 },
                    MinStep         = 1
                }
            };

            // 6) Y-axis
            chart.YAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit        = 0,
                    MaxLimit        = yMax,
                    TextSize        = 10,
                    LabelsPaint     = new SolidColorPaint(SKColors.White),
                    Labeler         = value => $"${value:0}",
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255,255,255,30)) { StrokeThickness = 1 },
                    TicksPaint      = null
                }
            };

            return container;
        }
    }
}
