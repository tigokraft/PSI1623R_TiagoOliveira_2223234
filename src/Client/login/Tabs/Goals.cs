// Goals.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using login.Helpers;
using login.Properties;

namespace login.Tabs
{
    public partial class Goals : Form
    {
        private class GoalDto
        {
            public int GoalId { get; set; }
            public string Name { get; set; }
            public decimal TargetAmount { get; set; }
            public decimal CurrentSaved { get; set; }
            public DateTime Deadline { get; set; }
        }

        private readonly HttpClient _http;
        private FlowLayoutPanel cardPanel;

        public Goals(HttpClient http)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            _http = http;

            // -- Leave your AddBtn and closeapp setup alone! --

            // Place cards panel below your buttons. Adjust the Y as needed if you have more controls.
            int panelY = Math.Max(AddBtn.Bottom, closeapp.Bottom) + 20;
            cardPanel = new FlowLayoutPanel
            {
                Location = new Point(0, panelY),
                Size = new Size(ClientSize.Width, ClientSize.Height - panelY),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(15, 10, 15, 10),
                BackColor = Color.Transparent,
            };
            Controls.Add(cardPanel);
            cardPanel.BringToFront();

            this.Resize += (s, e) =>
            {
                cardPanel.Width = ClientSize.Width;
                cardPanel.Height = ClientSize.Height - cardPanel.Top;
            };

            AddBtn.Click += AddBtn_Click;
            closeapp.Click += closeapp_Click;

            _ = ListLoader();
        }

        private async void AddBtn_Click(object sender, EventArgs e)
        {
            await Overlays.GoalOverlay(this, _http);
            await ListLoader();
        }

        private void closeapp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public async Task ListLoader()
        {
            cardPanel.SuspendLayout();
            cardPanel.Controls.Clear();

            HttpResponseMessage resp;
            try
            {
                resp = await _http.GetAsync("api/goal");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching goals: {ex.Message}");
                return;
            }
            if (!resp.IsSuccessStatusCode)
            {
                MessageBox.Show($"API returned {resp.StatusCode}");
                return;
            }

            string json = await resp.Content.ReadAsStringAsync();
            List<GoalDto> goals;
            try
            {
                goals = JsonSerializer.Deserialize<List<GoalDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse JSON: {ex.Message}");
                return;
            }

            if (goals == null || goals.Count == 0)
            {
                var emptyLbl = new Label
                {
                    Text = "No goals yet. Click '+ Add Goal' to get started.",
                    ForeColor = Color.LightGray,
                    Font = new Font("Segoe UI", 12f, FontStyle.Italic),
                    AutoSize = true,
                    Padding = new Padding(20),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                cardPanel.Controls.Add(emptyLbl);
                cardPanel.ResumeLayout();
                return;
            }

            foreach (var g in goals)
            {
                var card = CreateGoalCard(g);
                cardPanel.Controls.Add(card);
            }
            cardPanel.ResumeLayout();
        }

        // Interpolates between two colors based on percentage (0–100)
        private Color InterpolateColor(Color from, Color to, int percentage)
        {
            int r = from.R + (to.R - from.R) * percentage / 100;
            int g = from.G + (to.G - from.G) * percentage / 100;
            int b = from.B + (to.B - from.B) * percentage / 100;
            return Color.FromArgb(r, g, b);
        }

        private Guna2Panel CreateGoalCard(GoalDto g)
        {
            const int BaseHeight = 120;
            int pct = (g.TargetAmount > 0m)
                ? (int)Math.Round((double)g.CurrentSaved / (double)g.TargetAmount * 100)
                : 0;
            pct = Math.Max(0, Math.Min(100, pct));

            // Calculate dynamic color from (20,24,26) → (27,43,48)
            Color color = InterpolateColor(
                Color.FromArgb(20, 24, 26),
                Color.FromArgb(27, 43, 48),
                pct
            );

            var card = new Guna2Panel
            {
                Tag = "goalCard",
                Width = cardPanel.ClientSize.Width - 32,
                Height = BaseHeight,
                FillColor = Color.FromArgb(32, 34, 37),
                BorderColor = Color.FromArgb(50, 50, 50),
                BorderThickness = 1,
                BorderRadius = 10,
                Margin = new Padding(0, 0, 0, 16),
                Padding = new Padding(20)
            };

            // Title
            var lblTitle = new Label
            {
                Text = g.Name,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            // Horizontal progress bar
            var prog = new Guna2ProgressBar
            {
                Height = 12,
                Location = new Point(20, lblTitle.Bottom + 10),
                FillColor = Color.FromArgb(20, 24, 26),
                ProgressColor = color,
                BorderRadius = 6,
                Value = pct,
                Width = card.Width - 180
            };
            card.Controls.Add(prog);

            // Saved text
            var lblSaved = new Label
            {
                Text = $"Saved ${g.CurrentSaved:N0} of ${g.TargetAmount:N0}",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.LightGray,
                Location = new Point(20, prog.Bottom + 8),
                AutoSize = true
            };
            card.Controls.Add(lblSaved);

            // Circular percent
            var circ = new Guna2CircleProgressBar
            {
                Size = new Size(60, 60),
                Minimum = 0,
                Maximum = 100,
                Value = pct,
                ProgressThickness = 8,
                FillThickness = 8,
                ProgressColor = color,
                FillColor = Color.FromArgb(20, 24, 26),
                ForeColor = Color.White,
                ShowPercentage = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            };
            circ.Location = new Point(card.Width - circ.Width - 20, (card.Height - circ.Height) / 2);
            card.Controls.Add(circ);

            // Expander button
            var expanderBtn = new Guna2Button
            {
                Image = Resources.plus_white,
                ImageAlign = HorizontalAlignment.Center,
                Size = new Size(32, 32),
                BorderRadius = 8,
                FillColor = Color.FromArgb(67, 79, 82),
                ForeColor = Color.White,
                Location = new Point(card.Width - circ.Width - 80, card.Height / 2 - 16),
                Name = "expanderBtn"
            };
            card.Controls.Add(expanderBtn);

            Panel addMoneyPanel = null;
            bool expanded = false;

            expanderBtn.Click += async (s, e) =>
            {
                if (!expanded)
                {
                    if (addMoneyPanel == null)
                    {
                        int y0 = lblSaved.Bottom + 10;
                        addMoneyPanel = new Panel
                        {
                            Name = "addMoneyPanel",
                            Size = new Size(card.Width - 40, 40),
                            Location = new Point(20, y0),
                            BackColor = Color.Transparent
                        };

                        var txt = new Guna2TextBox
                        {
                            PlaceholderText = "Amount",
                            Size = new Size(120, 30),
                            Location = new Point(0, 5),
                            BorderRadius = 6,
                            BorderColor = Color.FromArgb(67, 79, 82),
                            FillColor = Color.FromArgb(18, 20, 20),
                            ForeColor = Color.White
                        };
                        addMoneyPanel.Controls.Add(txt);

                        var btn = new Guna2Button
                        {
                            Text = "Add Money",
                            Size = new Size(100, 30),
                            Location = new Point(txt.Right + 10, 5),
                            BorderRadius = 6,
                            FillColor = Color.FromArgb(20, 24, 26),
                            ForeColor = Color.White
                        };
                        addMoneyPanel.Controls.Add(btn);

                        btn.Click += async (s2, e2) =>
                        {
                            if (!decimal.TryParse(txt.Text, out var amt) || amt <= 0)
                            {
                                Cards.Show("Validation Error", "Enter a valid amount.", "OK");
                                return;
                            }
                            var dto = new { Amount = amt };
                            var body = JsonSerializer.Serialize(dto);
                            var content2 = new StringContent(body, Encoding.UTF8, "application/json");
                            var r2 = await _http.PostAsync($"api/goal/{g.GoalId}/save", content2);
                            if (!r2.IsSuccessStatusCode)
                            {
                                Cards.Show("Error", $"Failed: {r2.StatusCode}", "OK");
                                return;
                            }
                            await ListLoader();
                        };

                        card.Controls.Add(addMoneyPanel);
                        card.Height = addMoneyPanel.Bottom + 20;
                        expanded = true;
                    }
                }
                else
                {
                    card.Controls.Remove(addMoneyPanel);
                    card.Height = BaseHeight;
                    expanded = false;
                    addMoneyPanel = null;
                }
                cardPanel.PerformLayout();
            };

            card.Resize += (s, e) =>
            {
                prog.Width = card.Width - 180;
                addMoneyPanel?.SetBounds(0, addMoneyPanel.Location.Y, card.Width - 40, 40);
                circ.Location = new Point(card.Width - circ.Width - 20, (card.Height - circ.Height) / 2);
                expanderBtn.Location = new Point(card.Width - circ.Width - 80, 25);
            };

            return card;
        }
    }
}
