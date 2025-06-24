using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
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

        // Progress‐bar colors to cycle through
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

            // Wire your header buttons
            AddBtn.Click += AddBtn_Click;
            closeapp.Click += closeapp_Click;

            // Start loading
            _ = ListLoader();
        }

        private async void AddBtn_Click(object sender, EventArgs e)
        {
            await Overlays.GoalOverlay(this, _http);
        }

        private void closeapp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Fetches /api/goal and renders each goal as its own panel on the form.
        /// </summary>
        public async Task ListLoader()
        {
            // Remove old cards
            foreach (var old in Controls.OfType<Guna2Panel>()
                                       .Where(p => p.Tag as string == "goalCard")
                                       .ToList())
            {
                Controls.Remove(old);
                old.Dispose();
            }

            // Fetch from API
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

            // Render each goal
            int y = closeapp.Bottom + 20, idx = 0;
            foreach (var g in goals)
            {
                var card = CreateGoalCard(g, idx++);
                card.Tag = "goalCard";
                card.Location = new Point(20, y);
                Controls.Add(card);
                y += card.Height + 20;
            }
        }

        /// <summary>
        /// Builds one goal‐card styled like your mockup.
        /// </summary>
        private Guna2Panel CreateGoalCard(GoalDto g, int index)
        {
            // Pick a color
            var color = _progressColors[index % _progressColors.Length];

            // Compute percentage
            int pct = 0;
            if (g.TargetAmount > 0m)
                pct = (int)Math.Round((double)(g.CurrentSaved / g.TargetAmount) * 100);
            pct = Math.Min(100, Math.Max(0, pct));

            // Container panel
            var card = new Guna2Panel
            {
                Size = new Size(ClientSize.Width - 40, 120),
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

            // Horizontal progress bar
            var progress = new Guna2ProgressBar
            {
                Height = 12,
                Location = new Point(20, lblTitle.Bottom + 10),
                FillColor = Color.FromArgb(50, 50, 50),
                ProgressColor = color,
                BorderRadius = 6,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Value = pct
            };
            progress.Width = card.Width - 160;
            card.Controls.Add(progress);

            // Saved text
            var lblSaved = new Label
            {
                Text = $"Saved ${g.CurrentSaved:N0} of ${g.TargetAmount:N0}",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.LightGray,
                Location = new Point(20, progress.Bottom + 8),
                AutoSize = true
            };
            card.Controls.Add(lblSaved);

            // Circular percent indicator
            var circle = new Guna2CircleProgressBar
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
            circle.Location = new Point(
                card.Width - circle.Width - 20,
                (card.Height - circle.Height) / 2
            );
            card.Controls.Add(circle);

            return card;
        }
    }
}
