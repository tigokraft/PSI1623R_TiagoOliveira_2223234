using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using LiveCharts.WinForms;
using login.Helpers;
using login.Tabs;
using System.Runtime.InteropServices;
using System.Text.Json;

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

            Loader();
            this.ChartPanel.BackColor = System.Drawing.Color.FromArgb(16, 20, 20);

            

        }
        private async void Loader()
        {
            var balance = await GetBalanceAsync();
            //MessageBox.Show(Convert.ToString(balance), "Balance", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var monthly = await GetMonthlyBalanceAsync();
            Charts chartHelper = new Charts();
            CartesianChart ovChart = chartHelper.SetupChart();

            Expenses_list expList = new Expenses_list(_http);

            BalanceTxt.Text = $"{balance}";
            if (monthly != null)
            {
                lblSpent.Text = $"{monthly.Expenses:C2}";
            }
            this.ExpensesPanel.Controls.Add(expList);
            expList.Show();
            //this.ChartPanel.Controls.Add(ovChart); // Chart added to ChartPanel
            ChartPanel.Controls.Add(ovChart); // Chart added to the main form
        }

        public async Task<decimal> GetBalanceAsync()
        {
            var token = LoadToken();
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("No saved token. Please log in.");
                return 0;
            }

            try
            {
                var response = await _http.GetAsync("api/balance");
                if (response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();

                    // Optionally log to debug or MessageBox
                    Console.WriteLine($"Raw balance response: {body}");

                    if (decimal.TryParse(body, out var balance))
                        return balance;

                    MessageBox.Show("Failed to parse balance value.");
                    return 0;
                }

                MessageBox.Show("Failed to fetch balance.");
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
                var response = await _http.GetAsync("api/balance/monthly");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        public static string LoadToken()
        {
            string tokenPath = "auth.token";

            if (File.Exists(tokenPath))
                return File.ReadAllText(tokenPath);

            return null;
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
