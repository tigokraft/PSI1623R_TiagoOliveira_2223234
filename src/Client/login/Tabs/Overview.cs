using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;                  // ← needed for LINQ
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using login.Helpers;
using login.Tabs;                   // ← for Expenses_list
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

            ChartPanel.BackColor = Color.FromArgb(16, 20, 20);
            ExpensesPanel.BackColor = Color.FromArgb(16, 20, 20);

            Loader();
        }

        private async void Loader()
        {
            // 1) Fetch balances
            decimal balance = await GetBalanceAsync();
            MonthlyBalance monthly = await GetMonthlyBalanceAsync();

            BalanceTxt.Text = $"{balance:C0}";
            lblSpent.Text = monthly != null ? $"{monthly.Expenses:C0}" : "$0";

            // 2) Expenses list
            var expList = new Expenses_list(_http) { Dock = DockStyle.Fill };
            ExpensesPanel.Controls.Clear();
            ExpensesPanel.Controls.Add(expList);

            // 3) Fetch raw transactions
            var expenses = new List<Expenses_list.Transaction>();
            var incomes = new List<Expenses_list.Transaction>();

            HttpResponseMessage expResp = await _http.GetAsync("api/expense/summary");
            if (expResp.IsSuccessStatusCode)
            {
                string j = await expResp.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(j))
                {
                    if (doc.RootElement.TryGetProperty("expenses", out var arr))
                    {
                        expenses = JsonSerializer.Deserialize<List<Expenses_list.Transaction>>(
                            arr.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        ) ?? new List<Expenses_list.Transaction>();
                        expenses.ForEach(t => t.IsExpense = true);
                    }
                }
            }

            HttpResponseMessage incResp = await _http.GetAsync("api/income/summary");
            if (incResp.IsSuccessStatusCode)
            {
                string j = await incResp.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(j))
                {
                    if (doc.RootElement.TryGetProperty("incomes", out var arr))
                    {
                        incomes = JsonSerializer.Deserialize<List<Expenses_list.Transaction>>(
                            arr.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        ) ?? new List<Expenses_list.Transaction>();
                        incomes.ForEach(t => t.IsExpense = false);
                    }
                }
            }

            // 4) Align by date
            var allDates = expenses
                .Select(tx => tx.Date.Date)
                .Concat(incomes.Select(tx => tx.Date.Date))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            // X-axis labels: show only first, middle, last
            string[] xLabels = allDates
                .Select((d, i) =>
                    i == 0
                    || i == allDates.Count / 2
                    || i == allDates.Count - 1
                        ? d.ToString("MMM d")
                        : ""
                )
                .ToArray();

            // 5) Daily totals
            double[] expVals = allDates
                .Select(d => (double)expenses
                    .Where(tx => tx.Date.Date == d)
                    .Sum(tx => tx.Amount))
                .ToArray();

            double[] incVals = allDates
                .Select(d => (double)incomes
                    .Where(tx => tx.Date.Date == d)
                    .Sum(tx => tx.Amount))
                .ToArray();

            // 6) Render the chart
            var chartPanel = new Charts().SetupChart(incVals, expVals, xLabels);
            chartPanel.Dock = DockStyle.Fill;
            ChartPanel.Controls.Clear();
            ChartPanel.Controls.Add(chartPanel);
        }

        public async Task<decimal> GetBalanceAsync()
        {
            string token = LoadToken();
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
                    string body = await resp.Content.ReadAsStringAsync();
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
                string json = await resp.Content.ReadAsStringAsync();
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

        private void closeapp_Click(object sender, EventArgs e) => Application.Exit();

        private async void aiButton_Click(object sender, EventArgs e)
        {
            var monthly = await GetMonthlyBalanceAsync();
            if (monthly == null)
            {
                MessageBox.Show("Unable to fetch monthly balance.");
                return;
            }
            string summary = await AiHelper.GenerateOverviewAsync(monthly.Income, monthly.Expenses);
            MessageBox.Show(summary, "AI Overview");
        }
    }
}
