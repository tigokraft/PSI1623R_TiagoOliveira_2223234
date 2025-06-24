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
        // DTO mapping the JSON from GET /api/goal
        private class GoalDto
        {
            public int GoalId { get; set; }
            public string Name { get; set; }
            public decimal TargetAmount { get; set; }
            public decimal CurrentSaved { get; set; }
            public DateTime Deadline { get; set; }
        }

        private readonly HttpClient _http;

        // A simple color palette for your progress bars
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

            // Wire up header buttons
            AddBtn.Click += AddBtn_Click;
            closeapp.Click += closeapp_Click;

            // Kick off loading & rendering
            _ = ListLoader();
        }

        private async void AddBtn_Click(object sender, EventArgs e)
        {
            // Show your overlay; on save it will call ListLoader() again
            await Overlays.GoalOverlay(this, _http);
        }

        private void closeapp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Fetches from GET /api/goal and renders each goal as its own panel.
        /// </summary>
        public async Task ListLoader()
        {
            // 1) Remove existing goal‐cards
            foreach (var old in Controls.OfType<Guna2Panel>()
                                       .Where(p => p.Tag?.ToString() == "goalCard")
                                       .ToList())
            {
                Controls.Remove(old);
                old.Dispose();
            }

            // 2) Fetch goals
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

            // 3) Render each goal
            int y = closeapp.Bottom + 20;  // start just below your toolbar
            int idx = 0;
            foreach (var g in goals)
            {
                var card = CreateGoalCard(g, idx++);
                card.Tag = "goalCard";       // so we can remove it next time
                card.Location = new Point(20, y);
                Controls.Add(card);
                y += card.Height + 20;
            }
        }

        /// <summary>
        /// Builds a single goal card panel matching your mockup.
        /// </summary>
        private Guna2Panel CreateGoalCard(GoalDto g, int index)
        {
            // pick a color
            var color = _progressColors[index % _progressColors.Length];

            // compute percentage
            int pct = 0;
            if (g.TargetAmount > 0)
                pct = (int)Math.Round((double)(g.CurrentSaved / g.TargetAmount) * 100);
            pct = Math.Min(100, Math.Max(0, pct));

            // container panel
            var card = new Guna2Panel
            {
                Size = new Size(this.ClientSize.Width - 40, 100),
                FillColor = Color.FromArgb(24, 26, 27),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Title
            var lblTitle = new Label
            {
                Text = g.Name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            // Horizontal progress bar
            var progress = new Guna2ProgressBar
            {
                Height = 10,
                Location = new Point(20, lblTitle.Bottom + 10),
                FillColor = Color.FromArgb(40, 40, 40),
                ProgressColor = color,
                BorderRadius = 5,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Value = pct
            };
            progress.Width = card.Width - 140;
            card.Controls.Add(progress);

            // Saved text
            var lblSaved = new Label
            {
                Text = $"Saved ${g.CurrentSaved:N0} of ${g.TargetAmount:N0}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                Location = new Point(20, progress.Bottom + 8),
                AutoSize = true
            };
            card.Controls.Add(lblSaved);

            // Circular percentage
            var circle = new Guna2CircleProgressBar
            {
                Size = new Size(60, 60),
                Minimum = 0,
                Maximum = 100,
                Value = pct,
                ProgressThickness = 8,
                FillThickness = 8,
                ProgressColor = color,
                FillColor = Color.FromArgb(24, 26, 27),
                ShowPercentage = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            circle.Location = new Point(card.Width - circle.Width - 20,
                                        (card.Height - circle.Height) / 2);
            card.Controls.Add(circle);

            return card;
        }
    }
}
