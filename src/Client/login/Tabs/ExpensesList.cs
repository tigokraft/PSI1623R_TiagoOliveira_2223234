using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace login.Tabs
{
    public partial class Expenses_list : UserControl
    {
        // Colors
        private readonly Color BackgroundColor = Color.FromArgb(18, 20, 20);
        private readonly Color CardColor = Color.FromArgb(32, 34, 35);
        private readonly Color TextPrimary = Color.White;
        private readonly Color TextSecondary = Color.FromArgb(180, 180, 180);
        private readonly Color AmountExpense = Color.FromArgb(220, 60, 60);
        private readonly Color AmountIncome = Color.FromArgb(60, 180, 100);

        private readonly HttpClient _http;
        private List<Category> _categories = new List<Category>();

        // Scrolling infra
        private Guna2Panel viewportPanel;
        private Panel contentPanel;
        private Guna2VScrollBar gunaScroll;

        public class Category
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string Color { get; set; }
        }

        public class Transaction
        {
            public int Id { get; set; }
            public decimal Amount { get; set; }

            // map incomes' "descr" field into our Description
            [JsonPropertyName("descr")]
            public string Description { get; set; }

            public DateTime Date { get; set; }
            public int CategoryId { get; set; }
            public string Tags { get; set; }
            public bool IsExpense { get; set; }
        }

        public Expenses_list(HttpClient httpClient)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.Size = new Size(440, 345);
            this.BackColor = BackgroundColor;

            var lblSub = new Label
            {
                Text = "Latest activity",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(8, 28),
                BackColor = Color.Transparent
            };
            Controls.Add(lblSub);

            // Viewport panel
            viewportPanel = new Guna2Panel
            {
                Location = new Point(0, 48),
                Size = new Size(425, 260),
                BorderRadius = 16,
                FillColor = BackgroundColor,
                BorderThickness = 0
            };
            Controls.Add(viewportPanel);

            // Content panel
            contentPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(425, 0),
                BackColor = BackgroundColor,
                AutoScroll = false
            };
            viewportPanel.Controls.Add(contentPanel);

            // Custom scrollbar
            gunaScroll = new Guna2VScrollBar
            {
                Location = new Point(432, 48),
                Size = new Size(8, 260),
                FillColor = Color.FromArgb(40, 40, 40),
                ThumbColor = Color.FromArgb(120, 120, 120),
                BorderRadius = 4,
                LargeChange = 40,
                Minimum = 0,
                Maximum = 0,
                Value = 0
            };
            gunaScroll.Scroll += (s, e) =>
            {
                contentPanel.Top = -gunaScroll.Value;
            };
            Controls.Add(gunaScroll);

            // Mouse‐wheel scrolling
            viewportPanel.MouseWheel += MouseWheelScrollHandler;
            contentPanel.MouseWheel += MouseWheelScrollHandler;
            this.MouseWheel += MouseWheelScrollHandler;

            _http = httpClient;
            LoadAll();
        }

        private void MouseWheelScrollHandler(object sender, MouseEventArgs e)
        {
            int newVal = gunaScroll.Value - e.Delta / 3;
            if (newVal < gunaScroll.Minimum) newVal = gunaScroll.Minimum;
            if (newVal > gunaScroll.Maximum) newVal = gunaScroll.Maximum;

            if (newVal != gunaScroll.Value)
            {
                gunaScroll.Value = newVal;
                contentPanel.Top = -newVal;
            }
        }

        private void UpdateScrollBar()
        {
            int contentHeight = contentPanel.Controls
                .Cast<Control>()
                .Sum(c => c.Height + c.Margin.Top + c.Margin.Bottom);

            contentPanel.Height = contentHeight;
            int visibleHeight = viewportPanel.Height;
            int max = Math.Max(0, contentHeight - visibleHeight);

            gunaScroll.Maximum = max;
            gunaScroll.LargeChange = visibleHeight;
            gunaScroll.Enabled = max > 0;

            if (gunaScroll.Value > max)
                gunaScroll.Value = max;

            contentPanel.Top = -gunaScroll.Value;
        }

        private async void LoadAll()
        {
            await LoadCategories();
            await LoadExpensesAndIncomes();
        }

        private async Task LoadCategories()
        {
            var resp = await _http.GetAsync("api/category");
            if (!resp.IsSuccessStatusCode) return;

            var json = await resp.Content.ReadAsStringAsync();
            _categories = JsonSerializer.Deserialize<List<Category>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<Category>();
        }

        private async Task LoadExpensesAndIncomes()
        {
            var expenses = new List<Transaction>();
            var expResp = await _http.GetAsync("api/expense/summary");
            if (expResp.IsSuccessStatusCode)
            {
                string j = await expResp.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(j))
                {
                    if (doc.RootElement.TryGetProperty("expenses", out var arr))
                    {
                        expenses = JsonSerializer.Deserialize<List<Transaction>>(
                            arr.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        ) ?? new List<Transaction>();
                        expenses.ForEach(t => t.IsExpense = true);
                    }
                }
            }

            var incomes = new List<Transaction>();
            var incResp = await _http.GetAsync("api/income/summary");
            if (incResp.IsSuccessStatusCode)
            {
                string j = await incResp.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(j))
                {
                    if (doc.RootElement.TryGetProperty("incomes", out var arr))
                    {
                        incomes = JsonSerializer.Deserialize<List<Transaction>>(
                            arr.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        ) ?? new List<Transaction>();
                        incomes.ForEach(t => t.IsExpense = false);
                    }
                }
            }

            var all = expenses
                .Concat(incomes)
                .OrderByDescending(t => t.Date)
                .ToList();

            contentPanel.Controls.Clear();

            if (!all.Any())
            {
                var lbl = new Label
                {
                    Text = "No transactions.",
                    Font = new Font("Segoe UI", 10, FontStyle.Italic),
                    ForeColor = TextSecondary,
                    Size = new Size(viewportPanel.Width - 20, 35),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = BackgroundColor
                };
                contentPanel.Controls.Add(lbl);
                UpdateScrollBar();
                return;
            }

            int y = 0;
            foreach (var t in all)
            {
                var card = CreateCard(t);
                card.Location = new Point(6, y);
                contentPanel.Controls.Add(card);
                y += card.Height + card.Margin.Bottom;
                card.MouseWheel += MouseWheelScrollHandler;
            }
            UpdateScrollBar();
        }

        private Guna2Panel CreateCard(Transaction t)
        {
            // Lookup category
            var cat = _categories.FirstOrDefault(c => c.CategoryId == t.CategoryId);
            Color dotColor = Color.Gray;
            string catName = "Unknown";
            if (cat != null)
            {
                catName = cat.CategoryName;
                try { dotColor = ColorTranslator.FromHtml(cat.Color); }
                catch { }
            }

            // Build icon
            var iconBmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(iconBmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (SolidBrush brush = new SolidBrush(Color.White))
                    g.FillEllipse(brush, 0, 0, 24, 24);
                using (Pen pen = new Pen(t.IsExpense ? AmountExpense : AmountIncome, 3))
                {
                    g.DrawLine(pen, 6, 12, 18, 12);
                    if (!t.IsExpense)
                        g.DrawLine(pen, 12, 6, 12, 18);
                }
            }

            var panel = new Guna2Panel
            {
                BorderRadius = 10,
                FillColor = CardColor,
                Size = new Size(405, 46),
                Margin = new Padding(0, 0, 0, 10)
            };

            // Icon
            var pic = new PictureBox
            {
                Image = iconBmp,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Size = new Size(30, 30),
                Location = new Point(9, 8),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(pic);

            // Description
            var descr = new Label
            {
                Text = t.Description,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(48, 6),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(descr);

            int w = panel.ClientSize.Width;
            int h = panel.ClientSize.Height;

            // Category dot
            var catDot = new Panel
            {
                Size = new Size(9, 9),
                BackColor = Color.Transparent,
                Location = new Point(48, h - 9 - 6)
            };
            catDot.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(dotColor), 0, 0, 9, 9);
            };
            panel.Controls.Add(catDot);

            // Category label
            var catLbl = new Label
            {
                Text = catName,
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(48 + 9 + 4, h - new Font("Segoe UI", 8).Height - 6),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(catLbl);

            // Amount
            string amtText = (t.IsExpense ? "-" : "+") + $"{Math.Abs(t.Amount):0.00}";
            var amtLbl = new Label
            {
                Text = amtText,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = t.IsExpense ? AmountExpense : AmountIncome,
                AutoSize = true,
                Padding = new Padding(0, 0, 5, 0),
                BackColor = Color.Transparent
            };
            int amtWidth = TextRenderer.MeasureText(amtText, amtLbl.Font).Width + 5;
            amtLbl.Location = new Point(w - amtWidth - 12, 6);
            panel.Controls.Add(amtLbl);

            // Date
            string dtStr = (t.Date.Year == DateTime.Now.Year)
                ? t.Date.ToString("MMM d")
                : t.Date.ToString("MMM d, yyyy");
            var dateLbl = new Label
            {
                Text = dtStr,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = TextSecondary,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            int dateW = TextRenderer.MeasureText(dtStr, dateLbl.Font).Width;
            dateLbl.Location = new Point(w - dateW - 12, h - dateLbl.PreferredHeight - 6);
            panel.Controls.Add(dateLbl);

            return panel;
        }
    }
}
