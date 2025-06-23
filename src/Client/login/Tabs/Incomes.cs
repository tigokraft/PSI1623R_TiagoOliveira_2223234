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
    public partial class Incomes : Form
    {
        public class Income
        {
            public int IncomeId { get; set; }
            public decimal Amount { get; set; }
            public string Descr { get; set; }
            public DateTime Date { get; set; }
            public int CategoryId { get; set; }
            public bool? Recurring { get; set; } // optional support
        }

        public class IncomeResponse
        {
            [JsonPropertyName("incomes")]
            public List<Income> Incomes { get; set; }
        }

        private readonly HttpClient _http;
        private FlowLayoutPanel incomePanel;
        private List<CategoriesList.Category> _categories;
        private List<Income> _rawIncomes;

        private int column1Width = 110;
        private int column2Width = 150;
        private int column3Width = 240;
        private int column4Width = 90;
        private int column5Width = 90; // recurring column

        public Incomes(HttpClient http)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            _http = http;

            SetupMonths();
            cmbMonths.SelectedIndexChanged += (s, e) => RefreshList();
            cmbCat.SelectedIndexChanged += (s, e) => RefreshList();
            AddBtn.Click += async (s, e) => await Overlays.IncomeOverlay(this, _http);
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

            if (incomePanel != null)
            {
                this.Controls.Remove(incomePanel.Parent);
                incomePanel.Dispose();
                incomePanel = null;
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

            incomePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            tableContainerPanel.Controls.Add(incomePanel);
            Controls.Add(tableContainerPanel);
            tableContainerPanel.BringToFront();

            await LoadIncomes();
        }

        private async Task LoadIncomes()
        {
            _categories = await CategoriesList.GetCategoriesAsync(_http);
            var resp = await _http.GetAsync("api/income/summary");
            if (!resp.IsSuccessStatusCode)
            {
                MessageBox.Show($"Failed loading incomes: {resp.StatusCode}");
                return;
            }
            var json = await resp.Content.ReadAsStringAsync();
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var wrapper = JsonSerializer.Deserialize<IncomeResponse>(json, opts);
            _rawIncomes = wrapper?.Incomes ?? new List<Income>();

            SetupCategories();
            RefreshList();
        }

        public void RefreshList()
        {
            incomePanel.Controls.Clear();
            int totalWidth = column1Width + column2Width + column3Width + column4Width + column5Width;

            incomePanel.Controls.Add(CreateTableRow(
                "Date", "Category", "Description", "Amount", "Recurring",
                isHeader: true,
                totalWidth: totalWidth
            ));

            var filtered = _rawIncomes;
            if (cmbMonths.SelectedIndex > 0)
            {
                var sel = cmbMonths.SelectedItem.ToString();
                if (DateTime.TryParseExact(sel, "MMMM yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                    filtered = filtered.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).ToList();
            }

            if (cmbCat.SelectedIndex > 0 && _categories != null)
            {
                string selectedCat = cmbCat.SelectedItem.ToString();
                var cat = _categories.FirstOrDefault(c => c.CategoryName == selectedCat);
                if (cat != null)
                    filtered = filtered.Where(x => x.CategoryId == cat.CategoryId).ToList();
            }

            foreach (var inc in filtered)
            {
                var cat = _categories.FirstOrDefault(c => c.CategoryId == inc.CategoryId);
                var catName = cat?.CategoryName ?? "Unknown";
                var catColor = cat != null ? ColorTranslator.FromHtml(cat.Color) : Color.Gray;

                var recurringText = inc.Recurring == true ? "Yes" : "No";

                var row = CreateTableRow(
                    inc.Date.ToString("MMM d, yyyy"),
                    catName,
                    inc.Descr,
                    $"${inc.Amount:N2}",
                    recurringText,
                    isHeader: false,
                    categoryColor: catColor,
                    totalWidth: totalWidth,
                    income: inc
                );
                incomePanel.Controls.Add(row);
            }
        }

        private Guna2Panel CreateTableRow(
            string dateText, string categoryText, string descriptionText, string amountText, string recurringText,
            bool isHeader = false, Color? categoryColor = null, int totalWidth = 600, Income income = null)
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
            dateColumnPanel.Controls.Add(new Label
            {
                Text = dateText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column1Width - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleLeft,
            });
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
                categoryColumnPanel.Controls.Add(new Label
                {
                    Text = categoryText,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.LightGray,
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    Size = new Size(column2Width - internalPadding, rowPanel.Height),
                    Location = new Point(internalPadding, 0),
                    TextAlign = ContentAlignment.MiddleLeft,
                });
            }
            innerFlowPanel.Controls.Add(categoryColumnPanel);

            // Description
            var descriptionColumnPanel = createColumnPanel(column3Width);
            descriptionColumnPanel.Controls.Add(new Label
            {
                Text = descriptionText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column3Width - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleLeft,
            });
            innerFlowPanel.Controls.Add(descriptionColumnPanel);

            // Amount
            var amountColumnPanel = createColumnPanel(column4Width);
            amountColumnPanel.Controls.Add(new Label
            {
                Text = amountText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column4Width - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleRight,
            });
            innerFlowPanel.Controls.Add(amountColumnPanel);

            // Recurring
            var recurringColumnPanel = createColumnPanel(column5Width);
            recurringColumnPanel.Controls.Add(new Label
            {
                Text = recurringText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(column5Width - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleCenter,
            });
            innerFlowPanel.Controls.Add(recurringColumnPanel);

            if (!isHeader && income != null)
            {
                var contextMenu = new ContextMenuStrip();
                var editItem = new ToolStripMenuItem("Edit Income");
                var deleteItem = new ToolStripMenuItem("Delete Income");
                contextMenu.Items.AddRange(new[] { editItem, deleteItem });

                editItem.Click += async (s, e) =>
                {
                    await Overlays.IncomeOverlay(this, _http, income);
                };

                deleteItem.Click += async (s, e) =>
                {
                    var confirm = Cards.Show("Delete Income", "Are you sure you want to delete this income?", "OK");
                    if (confirm == DialogResult.OK)
                    {
                        var resp = await _http.DeleteAsync($"api/income/{income.IncomeId}");
                        if (resp.IsSuccessStatusCode)
                        {
                            RefreshList();
                        }
                        else
                        {
                            Cards.Show("Error", "Failed to delete income.", "OK");
                        }
                    }
                };

                void AttachContextMenu(Control control)
                {
                    control.MouseUp += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Right)
                            contextMenu.Show(rowPanel, rowPanel.PointToClient(Control.MousePosition));
                    };
                    foreach (Control child in control.Controls)
                        AttachContextMenu(child);
                }

                AttachContextMenu(rowPanel);
            }

            return rowPanel;
        }

        private void closeapp_Click(object sender, EventArgs e) => Application.Exit();
        private void Incomes_Load(object sender, EventArgs e) { }
    }
}
