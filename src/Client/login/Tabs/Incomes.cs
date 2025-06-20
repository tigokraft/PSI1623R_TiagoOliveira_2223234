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
using Guna.UI2.WinForms;
using login.Helpers;

namespace login.Tabs
{
    public partial class Incomes : Form
    {
        private readonly HttpClient _http;
        private FlowLayoutPanel _incomePanel;

        private int _colDateWidth = 150;
        private int _colDescWidth = 350;
        private int _colAmountWidth = 150;

        public Incomes(HttpClient http)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            _http = http;
            ListLoader();
        }

        private async void ListLoader()
        {
            int padding = 20;
            int containerWidth = 700;
            int startY = 80;

            int availableWidth = containerWidth - (padding * 2);
            int fixedWidth = _colDateWidth + _colAmountWidth;
            _colDescWidth = availableWidth - fixedWidth;

            var tableContainer = new Guna2Panel
            {
                Size = new Size(containerWidth, 400 + (padding * 2)),
                Location = new Point(20, startY),
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(40, 40, 40),
                FillColor = Color.FromArgb(24, 26, 27),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Padding = new Padding(padding)
            };

            _incomePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            tableContainer.Controls.Add(_incomePanel);
            Controls.Add(tableContainer);
            tableContainer.BringToFront();

            await GetIncomes();
        }

        public class Income
        {
            public int IncomeId { get; set; }
            public decimal Amount { get; set; }
            public string Descr { get; set; }
            public DateTime Date { get; set; }
        }

        public class IncomeSummary
        {
            public decimal TotalIncome { get; set; }
            public List<Income> Incomes { get; set; }
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
                Location = new Point((this.ClientSize.Width - 500) / 2, 50),
                Anchor = AnchorStyles.Top,
                BackColor = Color.Transparent,
                Name = "OverlayCard",
                //Font = new Font("Segoe UI", 9, FontStyle.Regular),
            };

            var descr = new Guna2TextBox
            {
                PlaceholderText = "Description",
                Size = new Size(300, 40),
                Location = new Point(25, 130),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                //Font = new Font("Segoe UI", 9, FontStyle.Regular),
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
                TabIndex = 0,
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
                TabIndex = 1,
            };

            var recurring = new Guna2CheckBox
            {
                Text = "Recurring",
                Size = new Size(300, 30),
                Location = new Point(25, 190),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                CheckedState =
                {
                    FillColor = Color.FromArgb(67, 79, 82),
                    BorderColor = Color.FromArgb(67, 79, 82),
                },
                UncheckedState =
                {
                    FillColor = Color.FromArgb(125, 137, 149),
                    BorderColor = Color.FromArgb(67, 79, 82),
                },
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
            };

            var recurrence = new Guna2ComboBox
            {
                Size = new Size(300, 40),
                Location = new Point(25, 230),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                BorderRadius = 10,
                TabIndex = 2,

                Items =
                {
                    "Weekly",
                    "Monthly",
                    "Yearly"
                },
            };

            var endDate = new Guna2DateTimePicker
            {
                Size = new Size(300, 40),
                Location = new Point(25, 280),
                BorderColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(18, 20, 20),
                FillColor = Color.FromArgb(18, 20, 20),
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                BorderRadius = 10,
                TabIndex = 3,
            };



            var CreateBtn = new Guna2Button
            {
                FillColor = Color.FromArgb(20, 24, 26),
                BorderColor = Color.FromArgb(39, 42, 44),
                BackColor = Color.FromArgb(18, 20, 20),
                BorderRadius = 10,
                BorderThickness = 1,
                Text = "Add Income",
                Size = new Size(300, 50),
                Location = new Point(25, 380),
                HoverState =
                {
                    BorderColor = Color.FromArgb(160, 160, 160),
                },
                Font = new Font("Segoe UI", 9),
            };


            var label = new Label
            {
                Text = "Add Income",
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

            recurrence.Visible = false;
            endDate.Visible = false;

            bool isRecurring = false;
            recurring.CheckedChanged += (s, ev) =>
            {
                if (recurring.Checked)
                {
                    recurrence.Visible = true;
                    endDate.Visible = true;
                    isRecurring = true;
                    recurrence.SelectedIndex = -1;
                }
                else
                {
                    recurrence.Visible = false;
                    endDate.Visible = false;
                }
            };


            closeBtn.Click += (s, ev) => { this.Controls.Remove(overlay); };
            CreateBtn.Click += async (s, ev) =>
            {
                if (isRecurring)
                {
                    if (recurrence.SelectedIndex == -1)
                    {
                        Cards.Show("Error", "Select recurrence", "OK");
                        return;
                    }

                    if (endDate.Value < DateTime.Now)
                    {
                        MessageBox.Show("End date cannot be in the past.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    recurrence.SelectedIndex = -1;
                    endDate.Value = DateTime.Now;
                }

                // Validate amount
                if (!decimal.TryParse(amount.Text, out var parsedAmount))
                {
                    MessageBox.Show("Enter a valid amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Call the method
                await Tasks.PostIncome(parsedAmount, descr.Text, isRecurring, recurrence.SelectedItem?.ToString() ?? "", endDate.Value.ToString("yyyy-MM-dd"), _http);
                await GetIncomes();
                this.Controls.Remove(overlay);
            };
            overlay.Controls.Add(label);
            overlay.Controls.Add(closeBtn);
            overlay.Controls.Add(descr);
            overlay.Controls.Add(amount);
            overlay.Controls.Add(recurring);
            overlay.Controls.Add(recurrence);
            overlay.Controls.Add(endDate);
            overlay.Controls.Add(CreateBtn);

            this.Controls.Add(overlay);
            overlay.BringToFront();
        }

        private async Task GetIncomes()
        {
            var response = await _http.GetAsync("api/income/summary");
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Failed to fetch incomes.");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<IncomeSummary>(json, options);

            _incomePanel.Controls.Clear();

            int totalWidth = _colDateWidth + _colDescWidth + _colAmountWidth;

            var header = CreateTableRow("Date", "Description", "Amount", true, totalWidth);
            _incomePanel.Controls.Add(header);

            foreach (var inc in data.Incomes)
            {
                var row = CreateTableRow(
                    inc.Date.ToString("MM/dd/yyyy"),
                    inc.Descr,
                    $"${inc.Amount:N2}",
                    false,
                    totalWidth);
                _incomePanel.Controls.Add(row);
            }
        }

        private Guna2Panel CreateTableRow(string dateText, string descriptionText, string amountText, bool isHeader, int totalWidth)
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
            rowPanel.HorizontalScroll.Maximum = 0;
            rowPanel.AutoScroll = false;
            rowPanel.VerticalScroll.Visible = false;
            rowPanel.AutoScroll = true;

            int internalPadding = 10;

            Func<int, Control> createColumnPanel = (width) => new Guna2Panel
            {
                Size = new Size(width, rowPanel.Height),
                FillColor = Color.Transparent,
                Margin = new Padding(0),
            };

            var datePanel = createColumnPanel(_colDateWidth);
            var dateLabel = new Label
            {
                Text = dateText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(_colDateWidth - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleLeft,
            };
            datePanel.Controls.Add(dateLabel);
            innerFlowPanel.Controls.Add(datePanel);

            var descPanel = createColumnPanel(_colDescWidth);
            var descLabel = new Label
            {
                Text = descriptionText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(_colDescWidth - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleLeft,
            };
            descPanel.Controls.Add(descLabel);
            innerFlowPanel.Controls.Add(descPanel);

            var amountPanel = createColumnPanel(_colAmountWidth);
            var amountLabel = new Label
            {
                Text = amountText,
                Font = new Font("Segoe UI", 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.LightGray : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(_colAmountWidth - internalPadding, rowPanel.Height),
                Location = new Point(internalPadding, 0),
                TextAlign = ContentAlignment.MiddleRight,
            };
            amountPanel.Controls.Add(amountLabel);
            innerFlowPanel.Controls.Add(amountPanel);

            return rowPanel;
        }
    }
}
