using System;
using System.Drawing;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using WpfAxis = LiveCharts.Wpf.Axis;
using WpfSeparator = LiveCharts.Wpf.Separator;
using WpfBrushes = System.Windows.Media.Brushes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;

namespace login
{
    public partial class Overview : Form
    {
        private readonly HttpClient _http;
        public Overview(HttpClient httpClient)
        {
            InitializeComponent();
            //SetupChart();
            _http = httpClient;
            Loader();
            
        }

        private async void Loader()
        {
            var balance = await GetBalanceAsync();
            BalanceTxt.Text = $"${balance:C2}";
        }

        private void SetupChart()
        {
            var cartesianChart = new LiveCharts.WinForms.CartesianChart
            {
                Size = new Size(300, 150),
                Location = new Point(450, 50)
            };

            cartesianChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Income",
                    Values = new ChartValues<double> { 0, 90, 130, 160, 180, 200 },
                    Stroke = WpfBrushes.MediumSpringGreen,
                    Fill = WpfBrushes.Transparent,
                    PointGeometry = null,
                    StrokeThickness = 2
                },
                new LineSeries
                {
                    Title = "Expenses",
                    Values = new ChartValues<double> { 0, 30, 50, 80, 95, 110 },
                    Stroke = WpfBrushes.IndianRed,
                    Fill = WpfBrushes.Transparent,
                    PointGeometry = null,
                    StrokeThickness = 2
                }
            };

            cartesianChart.AxisX.Add(new WpfAxis
            {
                Labels = new[] { "Apr 1", "6", "12", "18", "24", "30" },
                Foreground = WpfBrushes.Gray,
                Separator = new WpfSeparator { Stroke = WpfBrushes.DimGray }
            });

            cartesianChart.AxisY.Add(new WpfAxis
            {
                LabelFormatter = value => $"${value}",
                Foreground = WpfBrushes.Gray,
                Separator = new WpfSeparator { Stroke = WpfBrushes.DimGray }
            });

            this.Controls.Add(cartesianChart);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ExpensesBtn.FillColor = Color.FromArgb(100, 27, 43, 48);
            OvBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            budgetBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            GoalsBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
        }

        private void OvBtn_Click(object sender, EventArgs e)
        {
            OvBtn.FillColor = Color.FromArgb(100, 27, 43, 48);
            ExpensesBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            budgetBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            GoalsBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
        }

        private void budgetBtn_Click(object sender, EventArgs e)
        {
            budgetBtn.FillColor = Color.FromArgb(100, 27, 43, 48);
            ExpensesBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            OvBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            GoalsBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
        }

        private void GoalsBtn_Click(object sender, EventArgs e)
        {
            GoalsBtn.FillColor = Color.FromArgb(100, 27, 43, 48);
            ExpensesBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            OvBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            budgetBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("auth.token"))
            {
                File.Delete("auth.token");
                Application.Exit();
            }
        }

        private void BalanceTxt_Click(object sender, EventArgs e)
        {

        }


        public async Task<decimal> GetBalanceAsync()
        {
            var token = LoadToken();
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("No saved token. Please log in.");
                return 0;
            }

            _http.BaseAddress = new Uri("http://localhost:5034/");

            // Set headers if not already set globally
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (!_http.DefaultRequestHeaders.Contains("x-api-key"))
                _http.DefaultRequestHeaders.Add("x-api-key", "12345-abcdef-67890");

            try
            {
                var response = await _http.GetAsync("api/balance");
                if (response.IsSuccessStatusCode)
                {
                    var raw = await response.Content.ReadAsStringAsync();

                    if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var balance))
                        return balance;

                    MessageBox.Show("Invalid format received: " + raw);
                }
                else
                {
                    MessageBox.Show($"Balance request failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching balance: " + ex.Message);
            }

            return 0;
        }



        public static string LoadToken()
        {
            string tokenPath = "auth.token";

            if (File.Exists(tokenPath))
                return File.ReadAllText(tokenPath);

            return null;
        }
    }
}
