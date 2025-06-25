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
            public bool IsRecurringSource { get; set; }    // Optional: backend must support
            public string RecurrenceType { get; set; }     // Optional
        }

        public class ExpenseResponse
        {
            [JsonPropertyName("expenses")]
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
            SetupCategories();
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
            int desiredTotalContainerWidth = 800;
            int desiredStartY = 130;

            int availableContentWidth = desiredTotalContainerWidth - (tablePadding * 2);
            int fixedColumnsSumWidth = column1Width + column2Width + column4Width + column5Width;
            column3Width = availableContentWidth - fixedColumnsSumWidth;

            if (expensePanel != null)
            {
                this.Controls.Remove(expensePanel.Parent);
                expensePanel.Dispose();
                expensePanel = null;
            }

            var tableContainerPanel = new Guna2Panel
            {
                Size = new Size(desiredTotalContainerWidth, 400 + (tablePadding * 2)),
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

            var resp = await _http.GetAsync("api/expense");
            if (!resp.IsSuccessStatusCode)
            {
                MessageBox.Show($"Failed loading expenses: {resp.StatusCode}");
                return;
            }

            var json = await resp.Content.ReadAsStringAsync();
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _rawExpenses = JsonSerializer.Deserialize<List<Expense>>(json, opts) ?? new List<Expense>();

            SetupCategories();
            RefreshList();
        }

        public void RefreshList()
        {
            expensePanel.Controls.Clear();
            int totalWidth = column1Width + column2Width + column3Width + column4Width + column5Width;

            // Header row
            expensePanel.Controls.Add(CreateTableRow(
                "Date", "Category", "Description", "Amount", "Recurring",
                isHeader: true,
                totalWidth: totalWidth
            ));

            // Apply month filter
            var filtered = _rawExpenses;
            if (cmbMonths.SelectedIndex > 0)
            {
                var sel = cmbMonths.SelectedItem.ToString();
                if (DateTime.TryParseExact(sel, "MMMM yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                    filtered = filtered.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).ToList();
            }

            // Apply category filter
            if (cmbCat.SelectedIndex > 0 && _categories != null)
            {
                var selCat = cmbCat.SelectedItem.ToString();
                var cat = _categories.FirstOrDefault(c => c.CategoryName == selCat);
                if (cat != null)
                    filtered = filtered.Where(x => x.CategoryId == cat.CategoryId).ToList();
            }

            // Data rows
            foreach (var exp in filtered)
            {
                var cat = _categories.FirstOrDefault(c => c.CategoryId == exp.CategoryId);
                var catName = cat?.CategoryName ?? "Unknown";
                var catColor = cat != null ? ColorTranslator.FromHtml(cat.Color) : Color.Gray;
                var recurringText = exp.IsRecurringSource
                    ? (string.IsNullOrEmpty(exp.RecurrenceType) ? "Yes" : exp.RecurrenceType)
                    : "No";

                var row = CreateTableRow(
                    exp.Date.ToString("MMM d, yyyy"),
                    catName,
                    exp.Description,
                    $"${exp.Amount:N2}",
                    recurringText,
                    isHeader: false,
                    categoryColor: catColor,
                    totalWidth: totalWidth,
                    expense: exp
                );

                expensePanel.Controls.Add(row);
            }
        }

        private Guna2Panel CreateTableRow(
            string dateText,
            string categoryText,
            string descriptionText,
            string amountText,
            string recurringText,
            bool isHeader = false,
            Color? categoryColor = null,
            int totalWidth = 600,
            Expense expense = null      // <<< expense passed in for context menu
        )
        {
            var rowPanel = new Guna2Panel
            {
                Size = new Size(totalWidth, 30),
                Margin = new Padding(0),
                FillColor = Color.FromArgb(24, 26, 27),
                BorderColor = Color.FromArgb(35, 38, 39),
                BorderThickness = 1
            };

            var inner = new FlowLayoutPanel
            {
                Size = new Size(totalWidth, rowPanel.Height),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            rowPanel.Controls.Add(inner);

            int pad = 10;
            Func<int, Control> makeCol = width => new Guna2Panel
            {
                Size = new Size(width, rowPanel.Height),
                FillColor = Color.Transparent,
                Margin = new Padding(0)
            };

            // Date column
            var colDate = makeCol(column1Width);
            colDate.Controls.Add(new Label
            {
                Text = dateText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column1Width - pad, rowPanel.Height),
                Location = new Point(pad, 0),
                TextAlign = ContentAlignment.MiddleLeft
            });
            inner.Controls.Add(colDate);

            // Category column
            var colCat = makeCol(column2Width);
            if (!isHeader)
            {
                var tag = new Guna2Panel
                {
                    FillColor = Color.FromArgb(35, 38, 39),
                    BorderRadius = 5,
                    Padding = new Padding(5, 0, 8, 0),
                    Location = new Point(pad, (rowPanel.Height - 24) / 2),
                    MinimumSize = new Size(0, 20),
                    Size = new Size(column2Width, 24)
                };
                var dot = new Guna2Panel
                {
                    BorderRadius = 5,
                    FillColor = categoryColor ?? Color.Gray,
                    Size = new Size(12, 12),
                    Location = new Point(5, (tag.Height - 5) / 2)
                };
                tag.Controls.Add(dot);

                var lblCatText = new Label
                {
                    Text = categoryText,
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    AutoSize = true,
                    Location = new Point(dot.Right + 5, (tag.Height - 12) / 2)
                };
                tag.Controls.Add(lblCatText);
                tag.Width = lblCatText.Right + 5;
                colCat.Controls.Add(tag);
            }
            else
            {
                colCat.Controls.Add(new Label
                {
                    Text = categoryText,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.LightGray,
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    Size = new Size(column2Width - pad, rowPanel.Height),
                    Location = new Point(pad, 0),
                    TextAlign = ContentAlignment.MiddleLeft
                });
            }
            inner.Controls.Add(colCat);

            // Description column
            var colDesc = makeCol(column3Width);
            colDesc.Controls.Add(new Label
            {
                Text = descriptionText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column3Width - pad, rowPanel.Height),
                Location = new Point(pad, 0),
                TextAlign = ContentAlignment.MiddleLeft
            });
            inner.Controls.Add(colDesc);

            // Amount column
            var colAmt = makeCol(column4Width);
            colAmt.Controls.Add(new Label
            {
                Text = amountText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column4Width - pad, rowPanel.Height),
                Location = new Point(pad, 0),
                TextAlign = ContentAlignment.MiddleRight
            });
            inner.Controls.Add(colAmt);

            // Recurring column
            var colRec = makeCol(column5Width);
            colRec.Controls.Add(new Label
            {
                Text = recurringText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column5Width - pad, rowPanel.Height),
                Location = new Point(pad, 0),
                TextAlign = ContentAlignment.MiddleCenter
            });
            inner.Controls.Add(colRec);

            // Right-click context menu for data rows
            if (!isHeader && expense != null)
            {
                var menu = new ContextMenuStrip();
                var editItem = new ToolStripMenuItem("Edit Expense");
                var deleteItem = new ToolStripMenuItem("Delete Expense");
                menu.Items.AddRange(new[] { editItem, deleteItem });

                editItem.Click += async (s, e) =>
                {
                    await Overlays.EditExpenseOverlay(this, _http, expense);
                };
                deleteItem.Click += async (s, e) =>
                {
                    var confirm = Cards.Show(
                        "Delete Expense",
                        "This will delete the expense. Continue?",
                        "OK"
                    );
                    if (confirm == DialogResult.OK)
                    {
                        var resp = await _http.DeleteAsync($"api/expense/{expense.ExpenseId}");
                        if (resp.IsSuccessStatusCode)
                        {
                            Cards.Show("Success", "Expense deleted.", "OK");
                            await LoadExpenses();
                        }
                        else
                        {
                            Cards.Show("Error", "Failed to delete expense.", "OK");
                        }

                        RefreshList();
                    }
                };

                void AttachMenu(Control c)
                {
                    c.MouseUp += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Right)
                            menu.Show(rowPanel, rowPanel.PointToClient(Control.MousePosition));
                    };
                    foreach (Control child in c.Controls)
                        AttachMenu(child);
                }
                AttachMenu(rowPanel);
            }

            return rowPanel;
        }

        private void closeapp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
