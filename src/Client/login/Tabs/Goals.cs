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
        // DTO matching GET /api/goal
        private class GoalDto
        {
            public int GoalId { get; set; }
            public string Name { get; set; }
            public decimal TargetAmount { get; set; }
            public decimal CurrentSaved { get; set; }
            public DateTime Deadline { get; set; }
        }

        private readonly HttpClient _http;

        // Colors for your progress bars
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

            // Wire up your toolbar buttons
            AddBtn.Click += async (s, e) => await Overlays.GoalOverlay(this, _http);
            closeapp.Click += (s, e) => Application.Exit();

            // Whenever the form resizes, re‐layout the cards
            this.Resize += (s, e) => _ = ListLoader();

            // Initial load
            _ = ListLoader();
        }

        /// <summary>
        /// Fetches goals and renders each as its own panel on the form.
        /// </summary>
        public async Task ListLoader()
        {
            // 1) Remove any old goal panels
            var oldCards = Controls
                .OfType<Guna2Panel>()
                .Where(p => p.Tag as string == "goalCard")
                .ToList();
            foreach (var old in oldCards)
            {
                Controls.Remove(old);
                old.Dispose();
            }

            // 2) Call your API
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
                MessageBox.Show($"API error: {resp.StatusCode}");
                return;
            }

            var json = await resp.Content.ReadAsStringAsync();
            List<GoalDto> goals;
            try
            {
                goals = JsonSerializer.Deserialize<List<GoalDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"JSON parse error: {ex.Message}");
                return;
            }

            // 3) Render each goal as a panel
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
        /// Creates one Guna2Panel representing a goal.
        /// </summary>
        private Guna2Panel CreateGoalCard(GoalDto g, int index)
        {
            // pick a color and compute %
            Color color = _progressColors[index % _progressColors.Length];
            int pct = (g.TargetAmount > 0)
                ? (int)Math.Round((double)g.CurrentSaved / (double)g.TargetAmount * 100)
                : 0;
            pct = Math.Max(0, Math.Min(100, pct));

            const int baseHeight = 120;
            var card = new Guna2Panel
            {
                Tag = "goalCard",
                Size = new Size(ClientSize.Width - 40, baseHeight),
                FillColor = Color.FromArgb(32, 34, 37),
                BorderColor = Color.FromArgb(50, 50, 50),
                BorderThickness = 1,
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(20)
            };

            // Title label
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

            // Expand/collapse “Add Money” UI
            Action toggle = null;
            toggle = async () =>
            {
                var existing = card.Controls.Find("addMoneyContainer", false).FirstOrDefault();
                if (existing != null)
                {
                    card.Controls.Remove(existing);
                    card.Height = baseHeight;
                    return;
                }

                // build the “Add Money” row
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
                    var json2 = JsonSerializer.Serialize(dto);
                    using (var content2 = new StringContent(json2, Encoding.UTF8, "application/json"))
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

            // Attach toggle to card and all its children
            card.Click += (s, e) => toggle();
            foreach (Control c in card.Controls)
                c.Click += (s, e) => toggle();

            return card;
        }
    }
}
