using System.Drawing;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace login.Helpers
{
    internal class Charts
    {
        /// <summary>
        /// Returns a PlotView with two line series (Income/Expenses), dark theme,
        /// light horizontal gridlines, category X-axis, and dollar-formatted Y-axis.
        /// </summary>
        public PlotView SetupChart()
        {
            // 1) WinForms host
            var plotView = new PlotView
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = Color.FromArgb(20, 20, 20)
            };

            // 2) PlotModel
            var model = new PlotModel
            {
                Background = OxyColor.FromRgb(18, 20, 20),
                PlotAreaBackground = OxyColor.FromRgb(20, 20, 20),
                IsLegendVisible = true
            };

            // 3) Sample data + labels
            double[] incomeData = { 0, 90, 130, 160, 180, 200, 190, 210, 205, 220 };
            double[] expenseData = { 0, 30, 50, 80, 95, 110, 100, 120, 115, 130 };
            string[] xLabels = { "Apr 1", "", "", "", "", "24", "", "", "" };

            // 4) Income line
            var incomeSeries = new LineSeries
            {
                Title = "Income",
                Color = OxyColor.FromRgb(50, 220, 180),
                StrokeThickness = 2,
                MarkerType = MarkerType.None
            };
            for (int i = 0; i < incomeData.Length; i++)
                incomeSeries.Points.Add(new DataPoint(i, incomeData[i]));
            model.Series.Add(incomeSeries);

            // 5) Expenses line
            var expenseSeries = new LineSeries
            {
                Title = "Expenses",
                Color = OxyColor.FromRgb(220, 100, 90),
                StrokeThickness = 2,
                MarkerType = MarkerType.None
            };
            for (int i = 0; i < expenseData.Length; i++)
                expenseSeries.Points.Add(new DataPoint(i, expenseData[i]));
            model.Series.Add(expenseSeries);

            // 6) X-axis: categories
            var xAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                TextColor = OxyColors.LightGray,
                AxislineColor = OxyColors.Transparent,
                MajorGridlineStyle = LineStyle.None,
                MinorGridlineStyle = LineStyle.None,
                TickStyle = OxyPlot.Axes.TickStyle.None
            };
            foreach (var lbl in xLabels)
                xAxis.Labels.Add(lbl);
            model.Axes.Add(xAxis);

            // 7) Y-axis: $ formatter + horizontal gridlines
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                TextColor = OxyColors.LightGray,
                AxislineColor = OxyColors.Transparent,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromArgb(40, 128, 128, 128),
                MinorGridlineStyle = LineStyle.None,
                LabelFormatter = v => $"${v:0}"
            };
            model.Axes.Add(yAxis);

            // 8) Attach and return
            plotView.Model = model;
            return plotView;
        }
    }
}
