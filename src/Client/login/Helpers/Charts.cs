using System.Drawing;
using System.Linq;
using Guna.UI2.WinForms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace login.Helpers
{
    internal class Charts
    {
        /// <summary>
        /// Renders a panel with your Income/Expenses chart.
        /// </summary>
        /// <param name="incValues">Income amounts aligned by index.</param>
        /// <param name="expValues">Expense amounts aligned by index.</param>
        /// <param name="xLabels">Category labels for X-axis (same length).</param>
        public Guna2Panel SetupChart(double[] incValues, double[] expValues, string[] xLabels)
        {
            // 1) Container with border
            var container = new Guna2Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                FillColor = Color.FromArgb(16, 20, 20),
                BorderColor = Color.FromArgb(60, 60, 60),
                BorderThickness = 1,
                BorderRadius = 6
            };

            // 2) PlotView host
            var plotView = new PlotView
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = Color.FromArgb(16, 20, 20)
            };
            container.Controls.Add(plotView);

            // 3) Build model
            var model = new PlotModel
            {
                Background = OxyColors.Transparent,
                PlotAreaBackground = OxyColors.Transparent,
                IsLegendVisible = false
            };

            // 4) Convert arrays to DataPoints
            var incPts = incValues
                .Select((val, idx) => new DataPoint(idx, val))
                .ToList();
            var expPts = expValues
                .Select((val, idx) => new DataPoint(idx, val))
                .ToList();

            // 5) Income area fill
            var incArea = new AreaSeries
            {
                Color = OxyColors.Transparent,
                Fill = OxyColor.FromArgb(60, 50, 220, 180)
            };
            incArea.Points.AddRange(incPts);
            incArea.Points2.AddRange(incPts.Select(p => new DataPoint(p.X, 0)));
            model.Series.Add(incArea);

            // 6) Income outline
            var incLine = new LineSeries
            {
                Color = OxyColor.FromRgb(50, 220, 180),
                StrokeThickness = 2,
                MarkerType = MarkerType.None
            };
            incLine.Points.AddRange(incPts);
            model.Series.Add(incLine);

            // 7) Expense area fill
            var expArea = new AreaSeries
            {
                Color = OxyColors.Transparent,
                Fill = OxyColor.FromArgb(60, 220, 100, 90)
            };
            expArea.Points.AddRange(expPts);
            expArea.Points2.AddRange(expPts.Select(p => new DataPoint(p.X, 0)));
            model.Series.Add(expArea);

            // 8) Expense outline
            var expLine = new LineSeries
            {
                Color = OxyColor.FromRgb(220, 100, 90),
                StrokeThickness = 2,
                MarkerType = MarkerType.None
            };
            expLine.Points.AddRange(expPts);
            model.Series.Add(expLine);

            // 9) X-axis with subtle baseline
            var xAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                TextColor = OxyColors.White,
                FontSize = 10,
                AxislineColor = OxyColor.FromArgb(80, 255, 255, 255),
                AxislineThickness = 1,
                MajorGridlineStyle = LineStyle.None,
                MinorGridlineStyle = LineStyle.None,
                TickStyle = OxyPlot.Axes.TickStyle.None
            };
            foreach (var lbl in xLabels)
                xAxis.Labels.Add(lbl);
            model.Axes.Add(xAxis);

            // 10) Y-axis: horizontal grid + $ formatter
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                TextColor = OxyColors.White,
                FontSize = 10,
                AxislineColor = OxyColors.Transparent,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromArgb(30, 255, 255, 255),
                MinorGridlineStyle = LineStyle.None,
                LabelFormatter = v => $"${v:0}"
            };
            model.Axes.Add(yAxis);

            // 11) Attach
            plotView.Model = model;
            return container;
        }
    }
}
