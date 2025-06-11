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

namespace login
{
    public partial class Overview : Form
    {
        private readonly HttpClient _http;

        public Overview(HttpClient httpClient)
        {
            InitializeComponent();
            _http = httpClient;
            Loader();
            this.BackColor = System.Drawing.Color.FromArgb(16, 20, 20);

            this.ChartPanel.BackColor = System.Drawing.Color.FromArgb(16, 20, 20);
        }

        private async void Loader()
        {
            var balance = await GetBalanceAsync();
            Charts chartHelper = new Charts();
            CartesianChart ovChart = chartHelper.SetupChart();

            int PWidth = ExpensesPanel.Width;
            int PHeight = ExpensesPanel.Height;
            Expenses_list expList = new Expenses_list(_http, PWidth, PHeight);

            expList.TopLevel = false;
            expList.TopMost = true;
            expList.FormBorderStyle = FormBorderStyle.None;

            BalanceTxt.Text = $"{balance:C2}";
            this.ExpensesPanel.Controls.Add(expList);
            expList.Show();
            //this.ChartPanel.Controls.Add(ovChart); // Chart added to ChartPanel
            ChartPanel.Controls.Add(ovChart); // Chart added to the main form
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ExpensesBtn.FillColor = Color.FromArgb(100, 27, 43, 48);
            OvBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            budgetBtn.FillColor = Color.FromArgb(100, 14, 18, 18);
            GoalsBtn.FillColor = Color.FromArgb(100, 14, 18, 18);

            new Expenses(_http).Show();
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




        public static string LoadToken()
        {
            string tokenPath = "auth.token";

            if (File.Exists(tokenPath))
                return File.ReadAllText(tokenPath);

            return null;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
