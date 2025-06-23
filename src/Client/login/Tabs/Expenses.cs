using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using login.Helpers;

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
            public int CategoryId { get; set; }
            public bool IsRecurringSource { get; set; } // Optional: backend must support
            public string RecurrenceType { get; set; } // Optional
        }

        public class ExpenseResponse
        {
            public List<Expense> Expenses { get; set; }
        }

        private readonly HttpClient _http;
        private FlowLayoutPanel expensePanel;
        private List<CategoriesList.Category> _categories;
        private List<Expense> _rawExpenses;

        private int column1Width = 110;
        private int column2Width = 150;
        private int column3Width = 240;
        private int column4Width = 90;
        private int column5Width = 90; // Recurring

        public Expenses(HttpClient http)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            _http = http;

            SetupMonths();
            cmbMonths.SelectedIndexChanged += (s, e) => RefreshList();
            cmbCat.SelectedIndexChanged += (s, e) => RefreshList();

            AddBtn.Click += async (s, e) => await Overlays.ExpenseOverlay(this, _http);

            ListLoader();
        }

        private void SetupMonths()
        {
            cmbMonths.Items.Clear();
            cmbMonths.Items.Add("All Months");
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year - 5, now.Month, 1);
            var months = new List<string>();
            while (start <= now)
            {
                months.Add(start.ToString("MMMM yyyy"));
                start = start.AddMonths(1);
            }
            months.Reverse();
            cmbMonths.Items.AddRange(months.ToArray());
            cmbMonths.SelectedIndex = 0;
        }

        private void SetupCategories()
        {
            cmbCat.Items.Clear();
            cmbCat.Items.Add("All Categories");
            if (_categories != null)
            {
                foreach (var cat in _categories)
                    cmbCat.Items.Add(cat.CategoryName);
            }
            cmbCat.SelectedIndex = 0;
        }

        public async void ListLoader()
        {
            int tablePadding = 20;
            int desiredTotalContainerWidth = 900;
            int desiredStartY = 130;

            int availableContentWidth = desiredTotalContainerWidth - (tablePadding * 2);
            int fixedColumnsSumWidth = column1Width + column2Width + column4Width + column5Width;
            column3Width = availableContentWidth - fixedColumnsSumWidth;

            // Only one panel (expensePanel) at a time!
            if (expensePanel != null)
            {
                this.Controls.Remove(expensePanel.Parent);
                expensePanel.Dispose();
                expensePanel = null;
            }

            var tableContainerPanel = new Guna2Panel
            {
                Size = new Size(desiredTotalContainerWidth, 200 + (tablePadding * 2)),
                Location = new Point(20, desiredStartY),
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(40, 40, 40),
                FillColor = Color.FromArgb(24, 26, 27),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
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

            await LoadExpenses();
        }

        private async Task LoadExpenses()
        {
            _categories = await CategoriesList.GetCategoriesAsync(_http);

            var resp = await _http.GetAsync("api/expense/summary");
            if (!resp.IsSuccessStatusCode)
            {
                MessageBox.Show($"Failed loading expenses: {resp.StatusCode}");
                return;
            }
            var json = await resp.Content.ReadAsStringAsync();
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var wrapper = JsonSerializer.Deserialize<ExpenseResponse>(json, opts);
            _rawExpenses = wrapper?.Expenses ?? new List<Expense>();

            SetupCategories();
            RefreshList();
        }

        public void RefreshList()
        {
            expensePanel.Controls.Clear();
            int totalWidth = column1Width + column2Width + column3Width + column4Width + column5Width;

            // Header
            expensePanel.Controls.Add(CreateTableRow(
                "Date", "Category", "Description", "Amount", "Recurring",
                isHeader: true,
                totalWidth: totalWidth
            ));

            // Month filtering
            var filtered = _rawExpenses;
            if (cmbMonths.SelectedIndex > 0)
            {
                var sel = cmbMonths.SelectedItem.ToString();
                if (DateTime.TryParseExact(sel, "MMMM yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                    filtered = filtered.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).ToList();
            }

            // Category filtering
            if (cmbCat.SelectedIndex > 0 && _categories != null)
            {
                string selectedCat = cmbCat.SelectedItem.ToString();
                var cat = _categories.FirstOrDefault(c => c.CategoryName == selectedCat);
                if (cat != null)
                    filtered = filtered.Where(x => x.CategoryId == cat.CategoryId).ToList();
            }

            foreach (var exp in filtered)
            {
                var cat = _categories.FirstOrDefault(c => c.CategoryId == exp.CategoryId);
                var catName = cat?.CategoryName ?? "Unknown";
                var catColor = cat != null ? ColorTranslator.FromHtml(cat.Color) : Color.Gray;

                var row = CreateTableRow(
                    exp.Date.ToString("MMM d, yyyy"),
                    catName,
                    exp.Description,
                    $"${exp.Amount:N2}",
                    exp.IsRecurringSource ? exp.RecurrenceType ?? "Yes" : "No",
                    isHeader: false,
                    categoryColor: catColor,
                    totalWidth: totalWidth
                );

                // Right-click context menu on each row (except header)
                row.MouseUp += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        var context = new ContextMenuStrip();
                        var editItem = new ToolStripMenuItem("Edit Expense");
                        var deleteItem = new ToolStripMenuItem("Delete Expense");
                        context.Items.Add(editItem);
                        context.Items.Add(deleteItem);

                        // Pass exp as closure
                        editItem.Click += async (se, ev) =>
                        {
                            await Overlays.EditExpenseOverlay(this, _http, exp);
                        };

                        deleteItem.Click += async (se, ev) =>
                        {
                            var confirm = Cards.Show("Delete Expense", "This will delete the expense. Continue?", "OK");
                            if (confirm == DialogResult.OK)
                            {
                                var resp = await _http.DeleteAsync($"api/expense/{exp.ExpenseId}");
                                if (resp.IsSuccessStatusCode)
                                {
                                    Cards.Show("Success", "Expense deleted.", "OK");
                                    await LoadExpenses();
                                }
                                else
                                {
                                    Cards.Show("Error", "Failed to delete expense.", "OK");
                                }
                            }
                        };
                        context.Show(row, e.Location);
                    }
                };
                expensePanel.Controls.Add(row);
            }
        }

        private Guna2Panel CreateTableRow(
            string dateText, string categoryText, string descriptionText, string amountText, string recurringText,
            bool isHeader = false, Color? categoryColor = null, int totalWidth = 600)
        {
            var rowPanel = new Guna2Panel
            {
                Size = new Size(totalWidth, 30),
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

            // Date
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

            // Category
            var categoryColumnPanel = createColumnPanel(column2Width);
            if (!isHeader)
            {
                var categoryTagPanel = new Guna2Panel
                {
                    FillColor = Color.FromArgb(35, 38, 39),
                    BorderRadius = 5,
                    Padding = new Padding(5, 0, 8, 0),
                    Location = new Point(internalPadding, (rowPanel.Height - 24) / 2),
                    MinimumSize = new Size(0, 20),
                    Size = new Size(column2Width, 24)
                };

                var dot = new Guna2Panel
                {
                    BorderRadius = 5,
                    FillColor = categoryColor ?? Color.Gray,
                    Size = new Size(12, 12),
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

            // Description
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

            // Amount
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

            // Recurring
            var recurringColumnPanel = createColumnPanel(column5Width);
            var recurringLabel = new Label
            {
                Text = recurringText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column5Width - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            recurringColumnPanel.Controls.Add(recurringLabel);
            innerFlowPanel.Controls.Add(recurringColumnPanel);

            return rowPanel;
        }

        private void closeapp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
