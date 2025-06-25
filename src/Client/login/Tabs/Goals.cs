using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using login.Helpers;

namespace login.Tabs
{
    public partial class Goals : Form
    {
        // DTO matching your GET /api/goal response
        private class GoalDto
        {
            public int GoalId { get; set; }
            public string Name { get; set; }
            public decimal TargetAmount { get; set; }
            public decimal CurrentSaved { get; set; }
            public DateTime Deadline { get; set; }
        }

        private readonly HttpClient _http;

        // Colors for the progress bars
        private readonly Color[] _progressColors = new[]
        {
            Color.FromArgb(52, 152, 219),
            Color.FromArgb(241, 196, 15),
            Color.FromArgb(46, 204, 113),
            Color.FromArgb(231, 76,  60),
            Color.FromArgb(155, 89,  182)
        };

        public Goals(HttpClient http)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            _http = http;

            // Re-render on resize so panels fill width
            this.Resize += (s, e) => _ = ListLoader();

            // Initial load
            _ = ListLoader();
        }

        // Designer‐wired: when the “+ Add Goal” button is clicked
        private async void AddBtn_Click(object sender, EventArgs e)
        {
            await Overlays.GoalOverlay(this, _http);
        }

        // Designer‐wired: when the “X” close button is clicked
        private void closeapp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Fetches goals from the API and renders each as its own panel.
        /// </summary>
        public async Task ListLoader()
        {
            // Remove old goal panels
            var oldCards = Controls
                .OfType<Guna2Panel>()
                .Where(p => p.Tag as string == "goalCard")
                .ToList();
            foreach (var old in oldCards)
            {
                Controls.Remove(old);
                old.Dispose();
            }

            // Call GET /api/goal
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

            // Stack them under the toolbar
            int y = Math.Max(AddBtn.Bottom, closeapp.Bottom) + 20;
            int idx = 0;
            foreach (var g in goals)
            {
                var card = CreateGoalCard(g, idx++);
                card.Location = new Point(20, y);
                Controls.Add(card);
                y += card.Height + 20;
            }
        }

        /// <summary>
        /// Builds one Guna2Panel for a single goal, with expand/collapse Add-Money UI.
        /// </summary>
        private Guna2Panel CreateGoalCard(GoalDto g, int index)
        {
            // Pick a color and compute percent
            Color color = _progressColors[index % _progressColors.Length];
            int pct = (g.TargetAmount > 0m)
                ? (int)Math.Round((double)g.CurrentSaved / (double)g.TargetAmount * 100)
                : 0;
            pct = Math.Max(0, Math.Min(100, pct));

            const int BaseHeight = 120;
            var card = new Guna2Panel
            {
                Tag = "goalCard",
                Size = new Size(ClientSize.Width - 40, BaseHeight),
                FillColor = Color.FromArgb(32, 34, 37),
                BorderColor = Color.FromArgb(50, 50, 50),
                BorderThickness = 1,
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
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

            // Horizontal progress
            var prog = new Guna2ProgressBar
            {
                Height = 12,
                Location = new Point(20, lblTitle.Bottom + 10),
                FillColor = Color.FromArgb(50, 50, 50),
                ProgressColor = color,
                BorderRadius = 6,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Value = pct
            };
            prog.Width = card.Width - 160;
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

            // Circular percent indicator
            var circ = new Guna2CircleProgressBar
            {
                Size = new Size(60, 60),
                Minimum = 0,
                Maximum = 100,
                Value = pct,
                ProgressThickness = 8,
                FillThickness = 8,
                ProgressColor = color,
                FillColor = Color.FromArgb(32, 34, 37),
                ShowPercentage = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            circ.Location = new Point(
                card.Width - circ.Width - 20,
                (card.Height - circ.Height) / 2
            );
            card.Controls.Add(circ);

            // Expand / collapse "Add Money" UI
            Action toggle = null;
            toggle = async () =>
            {
                var old = card.Controls
                    .Find("addMoneyContainer", false)
                    .FirstOrDefault();
                if (old != null)
                {
                    card.Controls.Remove(old);
                    card.Height = BaseHeight;
                    return;
                }

                int y0 = lblSaved.Bottom + 10;
                var container = new Panel
                {
                    Name = "addMoneyContainer",
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
                container.Controls.Add(txt);

                var btn = new Guna2Button
                {
                    Text = "Add Money",
                    Size = new Size(100, 30),
                    Location = new Point(txt.Right + 10, 5),
                    BorderRadius = 6,
                    FillColor = Color.FromArgb(52, 152, 219),
                    ForeColor = Color.White
                };
                container.Controls.Add(btn);

                btn.Click += async (s, e) =>
                {
                    if (!decimal.TryParse(txt.Text, out var amt) || amt <= 0)
                    {
                        Cards.Show("Validation Error", "Enter a valid amount.", "OK");
                        return;
                    }
                    var dto = new { Amount = amt };
                    var body = JsonSerializer.Serialize(dto);
                    using (var content2 = new StringContent(body, Encoding.UTF8, "application/json"))
                    {
                        var r2 = await _http.PostAsync($"api/goal/{g.GoalId}/save", content2);
                        if (!r2.IsSuccessStatusCode)
                        {
                            Cards.Show("Error", $"Failed: {r2.StatusCode}", "OK");
                            return;
                        }
                    }
                    await ListLoader();
                };

                card.Controls.Add(container);
                card.Height = y0 + container.Height + 20;
            };

            // Hook the toggle on click for the card and its children
            card.Click += (s, e) => toggle();
            foreach (Control c in card.Controls)
                c.Click += (s, e) => toggle();

            return card;
        }
    }
}
