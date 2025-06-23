using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using login.Helpers;
using OxyPlot.WindowsForms;

namespace login.Tabs
{
    public partial class Overview : Form
    {
        private readonly HttpClient _http;

        private class MonthlyBalance
        {
            public decimal Income { get; set; }
            public decimal Expenses { get; set; }
            public decimal Available { get; set; }
        }

        public Overview(HttpClient httpClient)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            _http = httpClient;

            // match your dark theme
            ChartPanel.BackColor = Color.FromArgb(16, 20, 20);
            ExpensesPanel.BackColor = Color.FromArgb(16, 20, 20);

            Loader();
        }

        private async void Loader()
        {
            // 1) Fetch data
            var balance = await GetBalanceAsync();
            var monthly = await GetMonthlyBalanceAsync();

            // 2) Update labels
            BalanceTxt.Text = $"{balance:C0}";
            lblSpent.Text = monthly != null ? $"{monthly.Expenses:C0}" : "$0";

            // 3) Expenses list on the right
            var expList = new Expenses_list(_http) { Dock = DockStyle.Fill };
            ExpensesPanel.Controls.Clear();
            ExpensesPanel.Controls.Add(expList);

            // 4) Chart in the ChartPanel
            var chart = new Charts().SetupChart();
            chart.Dock = DockStyle.Fill;
            ChartPanel.Controls.Clear();
            ChartPanel.Controls.Add(chart);
        }

        private async Task<decimal> GetBalanceAsync()
        {
            var token = LoadToken();
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("No saved token. Please log in.");
                return 0;
            }

            try
            {
                var resp = await _http.GetAsync("api/balance");
                if (resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    if (decimal.TryParse(body, out var bal))
                        return bal;
                }
                MessageBox.Show("Failed to fetch or parse balance.");
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return 0;
            }
        }

        private async Task<MonthlyBalance> GetMonthlyBalanceAsync()
        {
            try
            {
                var resp = await _http.GetAsync("api/balance/monthly");
                if (!resp.IsSuccessStatusCode) return null;
                var json = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MonthlyBalance>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return null;
            }
        }

        public static string LoadToken()
        {
            const string path = "auth.token";
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }

        private void closeapp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void aiButton_Click(object sender, EventArgs e)
        {
            var monthly = await GetMonthlyBalanceAsync();
            if (monthly == null)
            {
                MessageBox.Show("Unable to fetch monthly balance.");
                return;
            }
            var summary = await AiHelper.GenerateOverviewAsync(monthly.Income, monthly.Expenses);
            MessageBox.Show(summary, "AI Overview");
        }
    }
}
