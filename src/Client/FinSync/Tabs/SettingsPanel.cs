using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using login.Helpers;

namespace login.Tabs
{
    public class SettingsPanel : UserControl
    {
        // colour palette
        readonly Color BackgroundColor = Color.FromArgb(18, 20, 20);
        readonly Color CardColor = Color.FromArgb(32, 34, 35);
        readonly Color TextPrimary = Color.White;
        readonly Color TextSecondary = Color.FromArgb(180, 180, 180);
        readonly Color AccentSave = Color.FromArgb(60, 180, 100);
        readonly Color AccentLogout = Color.FromArgb(220, 60, 60);

        readonly HttpClient _http;
        readonly ClientSettings _settings;

        Guna2TextBox _baseUrlBox;
        Guna2Button _saveBtn;
        Guna2Button _logoutBtn;

        public SettingsPanel(HttpClient httpClient)
        {
            _http = httpClient;
            _settings = ClientSettings.Load();

            DoubleBuffered = true;
            BackColor = BackgroundColor;
            Dock = DockStyle.Fill;

            InitializeComponent();
            _baseUrlBox.Text = _settings.ApiBaseUrl;
        }

        private void InitializeComponent()
        {
            // — Header —
            var header = new Label
            {
                Text = "Settings",
                Font = new Font(new FontFamily("Segoe UI"), 16f, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };
            Controls.Add(header);

            // — Card container —
            var card = new Guna2Panel
            {
                Location = new Point(20, 60),
                Size = new Size(560, 200),
                FillColor = CardColor,
                BorderRadius = 16
            };
            Controls.Add(card);

            // — API URL label —
            var lbl = new Label
            {
                Text = "API Base URL",
                Font = new Font(new FontFamily("Segoe UI"), 9f, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lbl);

            // — URL text-box —
            _baseUrlBox = new Guna2TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(520, 36),
                BorderRadius = 8,
                FillColor = BackgroundColor,
                ForeColor = TextPrimary,
                PlaceholderText = "https://api.yourservice.com"
            };
            card.Controls.Add(_baseUrlBox);

            // — Save button —
            _saveBtn = new Guna2Button
            {
                Text = "Save",
                Font = new Font(new FontFamily("Segoe UI"), 9f, FontStyle.Bold),
                Location = new Point(20, 105),
                Size = new Size(100, 36),
                BorderRadius = 8,
                FillColor = AccentSave,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            _saveBtn.Click += SaveBtn_Click;
            card.Controls.Add(_saveBtn);

            // — Logout button —
            _logoutBtn = new Guna2Button
            {
                Text = "Logout",
                Font = new Font(new FontFamily("Segoe UI"), 9f, FontStyle.Bold),
                Location = new Point(140, 105),
                Size = new Size(100, 36),
                BorderRadius = 8,
                FillColor = AccentLogout,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            _logoutBtn.Click += LogoutBtn_Click;
            card.Controls.Add(_logoutBtn);
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var newUrl = _baseUrlBox.Text.Trim();
            _settings.ApiBaseUrl = newUrl;
            _settings.Save();  // synchronous void

            if (Uri.TryCreate(newUrl, UriKind.Absolute, out var uri))
            {
                _http.BaseAddress = uri;
                MessageBox.Show(
                    "Settings saved.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show(
                    "Invalid URL.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LogoutBtn_Click(object sender, EventArgs e)
        {
            var tokenFile = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "auth.token"
            );
            if (File.Exists(tokenFile))
                File.Delete(tokenFile);

            Application.Exit();
        }
    }
}
