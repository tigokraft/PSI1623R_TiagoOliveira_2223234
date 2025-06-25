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
        /// <summary>
        /// Renders a panel with income/expense lines that auto-scale,
        /// zoom/pan, rotate labels, and show default tooltips.
        /// </summary>
        public Guna2Panel SetupChart(double[] incValues, double[] expValues, string[] xLabels)
        {
            // 1) Container styling
            var container = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(16, 20, 20),
                BorderColor = Color.FromArgb(60, 60, 60),
                BorderThickness = 1,
                BorderRadius = 6
            };

            // 2) Chart control
            var chart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(16, 20, 20),
                // enable both X and Y zoom/pan
                ZoomMode = ZoomAndPanMode.X | ZoomAndPanMode.Y
            };
            container.Controls.Add(chart);

            // 3) calculate axis limits (10% padding)
            double yMax = new[] { incValues.Max(), expValues.Max() }.Max() * 1.1;
            double xMax = xLabels.Length - 1;

            // 4) series definitions
            chart.Series = new ISeries[]
            {
                new LineSeries<double>(incValues)
                {
                    Fill        = new SolidColorPaint(new SKColor(50,220,180,60)),
                    Stroke      = new SolidColorPaint(new SKColor(50,220,180)) { StrokeThickness = 2 },
                    GeometrySize = 0,
                    ZIndex      = 0
                },
                new LineSeries<double>(expValues)
                {
                    Fill        = new SolidColorPaint(new SKColor(220,100,90,60)),
                    Stroke      = new SolidColorPaint(new SKColor(220,100,90)) { StrokeThickness = 2 },
                    GeometrySize = 0,
                    ZIndex      = 0
                }
            };

            // 5) X-axis: categories, rotated labels, subtle baseline ticks
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
                    SeparatorsPaint = null, // no vertical grid
                    TicksPaint      = new SolidColorPaint(new SKColor(255,255,255,80)) { StrokeThickness = 1 },
                    MinStep         = 1
                }
            };

            // 6) Y-axis: dollar labels + horizontal grid
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
                    TicksPaint      = null // no vertical tick marks
                }
            };

            return container;
        }
    }
}
