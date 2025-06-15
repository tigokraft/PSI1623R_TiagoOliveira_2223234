using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices; // You already have this
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace login.Tabs
{
    public partial class Expenses_list : Form
    {
        // Theme Colors
        private readonly Color BackgroundColor = Color.FromArgb(16, 20, 20);
        private readonly Color CardColor = Color.FromArgb(28, 28, 28);
        private readonly Color TextPrimary = Color.White;
        private readonly Color TextSecondary = Color.LightGray;

        // --- WINAPI STUFF ---
        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

        private const int SB_VERT = 1; // Vertical scrollbar

        public class Expense
        {
            public int ExpenseId { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public string Tags { get; set; }
            public string CategoryName { get; set; }
        }

        public class ExpenseResponse
        {
            public decimal TotalMonthlySpent { get; set; }
            public decimal TotalAllTimeSpent { get; set; }
            public List<Expense> Expenses { get; set; }
        }

        private readonly HttpClient _http;
        private readonly FlowLayoutPanel _expenseList;
        private readonly Label _totalBalanceLabel;
        private readonly Label _monthlySpentLabel;


        public Expenses_list(HttpClient httpClient, int w, int h)
        {
            _http = httpClient;

            Text = "Expenses Viewer";
            Width = w;
            Height = h;
            BackColor = BackgroundColor;


            // Top Summary Panel
            var summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 130,
                Padding = new Padding(20),
                BackColor = BackgroundColor
            };

            var totalCard = CreateSummaryCard("Total Balance", out _totalBalanceLabel);
            totalCard.Location = new Point(10, 10);

            var monthCard = CreateSummaryCard("Spent This Month", out _monthlySpentLabel);
            monthCard.Location = new Point(240, 10);

            summaryPanel.Controls.Add(totalCard);
            summaryPanel.Controls.Add(monthCard);

            // Expense List
            _expenseList = new NoScrollFlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = BackgroundColor,
                Padding = new Padding(20),
            };

            Controls.Add(_expenseList);
            //Controls.Add(summaryPanel); // Make sure you uncomment this if you want the summary panel to show up
            // or change the order of adding controls if you want summaryPanel on top

            load();

            // Hide the scrollbar after the form has been created
            this.Load += (s, e) => ShowScrollBar(_expenseList.Handle, SB_VERT, 0);
        }

        private async void load()
        {
            await LoadExpensesAsync();
        }

        private Guna2Panel CreateSummaryCard(string title, out Label valueLabel)
        {
            var card = new Guna2Panel
            {
                BorderRadius = 20,
                FillColor = CardColor,
                Size = new Size(200, 100),
                ShadowDecoration = { Enabled = true }
            };

            var titleLabel = new Label
            {
                Text = title,
                ForeColor = TextSecondary,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 10),
                AutoSize = true
            };

            valueLabel = new Label
            {
                Text = "$0.00",
                ForeColor = TextPrimary,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 40),
                AutoSize = true
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);
            return card;
        }

        private async System.Threading.Tasks.Task LoadExpensesAsync()
        {
            var response = await _http.GetAsync("api/expense/summary");
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Failed to fetch expenses.");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<ExpenseResponse>(json, options);

            _totalBalanceLabel.Text = $"${data.TotalAllTimeSpent:0.00}";
            _monthlySpentLabel.Text = $"${data.TotalMonthlySpent:0.00}";

            _expenseList.Controls.Clear();

            foreach (var exp in data.Expenses)
            {
                var panel = new Guna2Panel
                {
                    BorderRadius = 10,
                    FillColor = CardColor,
                    Size = new Size(_expenseList.Width - 60, 80), // Adjusted width to account for padding
                    Margin = new Padding(10),
                };

                var title = new Label
                {
                    Text = $"{exp.Description} - {exp.Amount:C}",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = TextPrimary,
                    BackColor = Color.Transparent,
                    AutoSize = true,
                    Location = new Point(15, 10)
                };

                var info = new Label
                {
                    Text = $"Date: {exp.Date:yyyy-MM-dd HH:mm} | Tags: {exp.Tags} | Category: {exp.CategoryName}",
                    Font = new Font("Segoe UI", 9),
                    ForeColor = TextSecondary,
                    BackColor = Color.Transparent,
                    AutoSize = true,
                    Location = new Point(15, 35)
                };

                panel.Controls.Add(title);
                panel.Controls.Add(info);
                _expenseList.Controls.Add(panel);
            }
        }
    }

    public class NoScrollFlowLayoutPanel : FlowLayoutPanel
    {
        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

        private const int SB_VERT = 1; // Vertical scrollbar

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x83) // WM_NCCALCSIZE
            {
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ShowScrollBar(this.Handle, SB_VERT, 0);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x00200000; // WS_VSCROLL
                return cp;
            }
        }
    }

}