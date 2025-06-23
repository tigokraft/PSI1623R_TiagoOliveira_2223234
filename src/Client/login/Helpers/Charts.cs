using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
        /// Returns a Guna2Panel containing a PlotView with Income/Expenses
        /// area fills + lines, a faint baseline, and a 1px panel border.
        /// </summary>
        public Guna2Panel SetupChart()
        {
            // 1) Container with border
            var container = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(16, 20, 20),
                BorderColor = Color.FromArgb(60, 60, 60),
                BorderThickness = 1,
                BorderRadius = 6
            };

            // 2) PlotView host
            var plotView = new PlotView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(16, 20, 20)
            };
            container.Controls.Add(plotView);

            // 3) Model
            var model = new PlotModel
            {
                Background = OxyColors.Transparent,
                PlotAreaBackground = OxyColors.Transparent,
                IsLegendVisible = false
            };

            // 4) Data + labels
            double[] incomeRaw = { 0, 90, 130, 160, 180, 200, 190, 210, 205, 220 };
            double[] expenseRaw = { 0, 30, 50, 80, 95, 110, 100, 120, 115, 130 };
            string[] xLabels = { "Apr 1", "", "", "", "", "24", "", "", "" };

            // 5) Smooth lightly
            var incomePts = Smooth(incomeRaw, 1).ToList();
            var expensePts = Smooth(expenseRaw, 1).ToList();

            // 6) Income area fill
            var incArea = new AreaSeries
            {
                Color = OxyColors.Transparent,
                Fill = OxyColor.FromArgb(60, 50, 220, 180), // 60/255 opacity
            };
            incArea.Points.AddRange(incomePts);
            incArea.Points2.AddRange(incomePts.Select(p => new DataPoint(p.X, 0)));
            model.Series.Add(incArea);

            // 7) Income outline
            var incLine = new LineSeries
            {
                Color = OxyColor.FromRgb(50, 220, 180),
                StrokeThickness = 2,
                MarkerType = MarkerType.None
            };
            incLine.Points.AddRange(incomePts);
            model.Series.Add(incLine);

            // 8) Expenses area fill
            var expArea = new AreaSeries
            {
                Color = OxyColors.Transparent,
                Fill = OxyColor.FromArgb(60, 220, 100, 90),
            };
            expArea.Points.AddRange(expensePts);
            expArea.Points2.AddRange(expensePts.Select(p => new DataPoint(p.X, 0)));
            model.Series.Add(expArea);

            // 9) Expenses outline
            var expLine = new LineSeries
            {
                Color = OxyColor.FromRgb(220, 100, 90),
                StrokeThickness = 2,
                MarkerType = MarkerType.None
            };
            expLine.Points.AddRange(expensePts);
            model.Series.Add(expLine);

            // 10) X-axis with subtle baseline
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
            foreach (var lbl in xLabels) xAxis.Labels.Add(lbl);
            model.Axes.Add(xAxis);

            // 11) Y-axis with gridlines
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

            // 12) Attach and return
            plotView.Model = model;
            return container;
        }

        // moving-average smoother
        private static IEnumerable<DataPoint> Smooth(double[] data, int w)
        {
            int n = data.Length;
            for (int i = 0; i < n; i++)
            {
                int s = i - w < 0 ? 0 : i - w;
                int e = i + w >= n ? n - 1 : i + w;
                double sum = 0;
                for (int j = s; j <= e; j++) sum += data[j];
                yield return new DataPoint(i, sum / (e - s + 1));
            }
        }
    }
}
