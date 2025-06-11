using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using login.Helpers;
using Guna.UI2.WinForms;
using System.Text.Json;

namespace login.Tabs
{
    public partial class Expenses : Form
    {
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


        public class ExpenseRequest
        {
            public decimal Amount { get; set; }
            public string Tags { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public int CategoryId { get; set; }
        }
        private readonly HttpClient _http;
        private FlowLayoutPanel expensePanel;

        private int column1Width = 110;
        private int column2Width = 150;
        private int column3Width = 240; // This will be dynamically updated in ListLoader
        private int column4Width = 90;

        public Expenses(HttpClient http)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            _http = http;
            ListLoader();
        }

        public async void ListLoader()
        {
            int tablePadding = 20;
            int desiredTotalContainerWidth = 800; // Desired overall width of the table container
            int desiredStartY = 80;    // Desired Y start position

            // Calculate available width for columns inside the padded container
            int availableContentWidth = desiredTotalContainerWidth - (tablePadding * 2);

            // Sum of fixed column widths (Date, Category, Amount)
            int fixedColumnsSumWidth = column1Width + column2Width + column4Width;

            // Dynamically adjust column3Width (Description) to fill the remaining space
            column3Width = availableContentWidth - fixedColumnsSumWidth;

            var tableContainerPanel = new Guna2Panel
            {
                Size = new Size(desiredTotalContainerWidth, 400 + (tablePadding * 2)),
                Location = new Point(20, desiredStartY), // Set desired Y start position
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(40, 40, 40),
                FillColor = Color.FromArgb(24, 26, 27),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Padding = new Padding(tablePadding)
            };

            expensePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            tableContainerPanel.Controls.Add(expensePanel);
            Controls.Add(tableContainerPanel);
            tableContainerPanel.BringToFront();

            await GetExpenses();
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            var overlay = new Guna2Panel
            {
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(40, 40, 40),
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.Transparent,
                Size = new Size(350, 450),
                Location = new Point((this.ClientSize.Width - 300) / 2, 50),
                Anchor = AnchorStyles.Top,
                BackColor = Color.Transparent,
                Name = "OverlayCard",
            };

            var amount = new Guna2TextBox
            {
                PlaceholderText = "Amount",
                Size = new Size(300, 40),
                Location = new Point(25, 70),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                FocusedState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                BorderRadius = 10,
            };

            var descr = new Guna2TextBox
            {
                PlaceholderText = "Description",
                Size = new Size(300, 40),
                Location = new Point(25, 130),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                FocusedState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                BorderRadius = 10,
            };

            var Tag = new Guna2TextBox
            {
                PlaceholderText = "Tag",
                Size = new Size (300, 40),
                Location = new Point(25, 190),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                FocusedState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                BorderRadius = 10,
            };

            var CreateBtn = new Guna2Button
            {
                FillColor = Color.FromArgb(20, 24, 26),
                BorderColor = Color.FromArgb(39, 42, 44),
                BackColor = Color.FromArgb(18, 20, 20),
                BorderRadius = 10,
                BorderThickness = 1,
                Text = "Add Expense",
                Size = new Size (300, 50),
                Location = new Point(25, 380),
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                Font = new Font("Segoe UI", 9)
            };


            var label = new Label
            {
                Text = "Add Expense",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 20)
            };

            var closeBtn = new Guna2ImageButton
            {
                Image = Properties.Resources.close,
                Size = new Size(30, 30),
                Location = new Point(overlay.Width - 40, 10),
                ForeColor = Color.Transparent,
            };

            closeBtn.Click += (s, ev) => { this.Controls.Remove(overlay); };
            CreateBtn.Click += async (s, ev) =>
            {
                await PostExpense(descr.Text, Convert.ToDecimal(amount.Text), Tag.Text);
                this.Controls.Remove(overlay);
            };
            overlay.Controls.Add(label);
            overlay.Controls.Add(closeBtn);
            overlay.Controls.Add(descr);
            overlay.Controls.Add(amount);
            overlay.Controls.Add(Tag);
            overlay.Controls.Add(CreateBtn);

            this.Controls.Add(overlay);
            overlay.BringToFront();

            

        }
        private async Task PostExpense(string desc, decimal amt, string tags)
        {
            var payload = new ExpenseRequest
            {
                Amount = amt,
                Description = desc,
                Tags = tags,
                CategoryId = 1,
                Date = DateTime.Now
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("api/expense/", content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("expense added");
                await GetExpenses();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Failed: {response.StatusCode} - {error}");
            }

        }

        private async System.Threading.Tasks.Task GetExpenses()
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

            expensePanel.Controls.Clear();

            // totalWidth will now correctly reflect the sum of adjusted column widths
            int totalWidth = column1Width + column2Width + column3Width + column4Width;

            Guna2Panel headerRowPanel = CreateTableRow(
                "Date",
                "Category",
                "Description",
                "Amount",
                isHeader: true,
                totalWidth: totalWidth
            );
            expensePanel.Controls.Add(headerRowPanel);

            foreach (var exp in data.Expenses)
            {
                Guna2Panel expenseRowPanel = CreateTableRow(
                    exp.Date.ToString("MM/dd/yyyy"),
                    exp.CategoryName,
                    exp.Description,
                    $"${exp.Amount:N2}",
                    categoryColor: GetCategoryColor(exp.CategoryName),
                    totalWidth: totalWidth
                );
                expensePanel.Controls.Add(expenseRowPanel);
            }
        }

        private Guna2Panel CreateTableRow(string dateText, string categoryText, string descriptionText, string amountText, bool isHeader = false, Color? categoryColor = null, int totalWidth = 600)
        {
            var rowPanel = new Guna2Panel
            {
                Size = new Size(totalWidth, 40),
                Margin = new Padding(0),
                FillColor = Color.FromArgb(24, 26, 27),
                BorderColor = Color.FromArgb(35, 38, 39),
                BorderThickness = 1,
            };

            var innerFlowPanel = new FlowLayoutPanel
            {
                Size = new Size(totalWidth, rowPanel.Height),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
            rowPanel.Controls.Add(innerFlowPanel);

            int internalPadding = 10;

            Func<int, Control> createColumnPanel = (width) => new Guna2Panel
            {
                Size = new Size(width, rowPanel.Height),
                FillColor = Color.Transparent,
                Margin = new Padding(0),
            };

            var dateColumnPanel = createColumnPanel(column1Width);
            var dateLabel = new Label
            {
                Text = dateText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column1Width - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleLeft,
            };
            dateColumnPanel.Controls.Add(dateLabel);
            innerFlowPanel.Controls.Add(dateColumnPanel);

            var categoryColumnPanel = createColumnPanel(column2Width);
            if (!isHeader)
            {
                var categoryTagPanel = new Guna2Panel
                {
                    FillColor = Color.FromArgb(35, 38, 39),
                    BorderRadius = 5,
                    Padding = new Padding(5, 0, 8, 0),
                    Location = new Point(internalPadding, (rowPanel.Height - 30) / 2),
                    MinimumSize = new Size(0, 20),
                    Size = new Size (column2Width, 30)
                };

                var dot = new Guna2Panel
                {
                    BorderRadius = 8,
                    FillColor = categoryColor ?? Color.Gray,
                    Size = new Size(16, 16),
                    Location = new Point(5, (categoryTagPanel.MinimumSize.Height - 5) / 2)
                };
                categoryTagPanel.Controls.Add(dot);

                var catTextLabel = new Label
                {
                    Text = categoryText,
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    AutoSize = true,
                    Location = new Point(dot.Right + 5, (categoryTagPanel.MinimumSize.Height - 5) / 2)
                };
                categoryTagPanel.Controls.Add(catTextLabel);

                categoryTagPanel.Width = catTextLabel.Right + 5;

                categoryColumnPanel.Controls.Add(categoryTagPanel);

            }
            else
            {
                var catHeaderLabel = new Label
                {
                    Text = categoryText,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.LightGray,
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    Size = new Size(column2Width - internalPadding, rowPanel.Height),
                    Location = new Point(internalPadding, 0),
                    TextAlign = ContentAlignment.MiddleLeft,
                };
                categoryColumnPanel.Controls.Add(catHeaderLabel);
            }
            innerFlowPanel.Controls.Add(categoryColumnPanel);

            var descriptionColumnPanel = createColumnPanel(column3Width);
            var descriptionLabel = new Label
            {
                Text = descriptionText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column3Width - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleLeft,
            };
            descriptionColumnPanel.Controls.Add(descriptionLabel);
            innerFlowPanel.Controls.Add(descriptionColumnPanel);

            var amountColumnPanel = createColumnPanel(column4Width);
            var amountLabel = new Label
            {
                Text = amountText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column4Width - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleRight,
            };
            amountColumnPanel.Controls.Add(amountLabel);
            innerFlowPanel.Controls.Add(amountColumnPanel);

            return rowPanel;
        }

        private Color GetCategoryColor(string categoryName)
        {
            switch (categoryName.ToLower())
            {
                case "groceries":
                    return Color.FromArgb(70, 190, 140);
                case "transport":
                    return Color.FromArgb(70, 150, 220);
                case "dining":
                    return Color.FromArgb(255, 165, 0);
                case "alimentação":
                    return Color.FromArgb(120, 120, 120);
                default:
                    return Color.FromArgb(120, 120, 120);
            }
        }
    }
}