using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using login.Helpers;

namespace login.Tabs
{
    public class SettingsPanel : Form
    {
        // Same palette as Expenses_list
        private readonly Color BackgroundColor = Color.FromArgb(18, 20, 20);
        private readonly Color CardColor = Color.FromArgb(32, 34, 35);
        private readonly Color TextPrimary = Color.White;
        private readonly Color TextSecondary = Color.FromArgb(180, 180, 180);
        private readonly Color AccentSave = Color.FromArgb(60, 180, 100);
        private readonly Color AccentLogout = Color.FromArgb(220, 60, 60);

        private readonly HttpClient _http;
        private readonly ClientSettings _settings;

        private Guna2TextBox baseUrlBox;
        private Guna2Button saveBtn;
        private Guna2Button logoutBtn;

        public SettingsPanel(HttpClient httpClient)
        {
            _http = httpClient;
            _settings = ClientSettings.Load();
            InitializeComponent();
            baseUrlBox.Text = _settings.ApiBaseUrl;
        }

        private void InitializeComponent()
        {
            // Form setup
            Text = "Settings";
            BackColor = BackgroundColor;
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(600, 200);

            // Outer panel for rounded-corner card look
            var container = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = CardColor,
                BorderRadius = 16
            };
            Controls.Add(container);

            // Label
            var lbl = new Label
            {
                Text = "API Base URL",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = TextSecondary,
                Location = new Point(30, 30),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            container.Controls.Add(lbl);

            // Text box
            baseUrlBox = new Guna2TextBox
            {
                Location = new Point(30, 55),
                Size = new Size(540, 36),
                BorderRadius = 8,
                FillColor = BackgroundColor,
                ForeColor = TextPrimary,
                PlaceholderText = "https://api.yourservice.com"
            };
            container.Controls.Add(baseUrlBox);

            // Save button
            saveBtn = new Guna2Button
            {
                Text = "Save",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(30, 110),
                Size = new Size(100, 36),
                BorderRadius = 8,
                FillColor = AccentSave,
                ForeColor = Color.White
            };
            saveBtn.Click += SaveBtn_Click;
            container.Controls.Add(saveBtn);

            // Logout button
            logoutBtn = new Guna2Button
            {
                Text = "Logout",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(150, 110),
                Size = new Size(100, 36),
                BorderRadius = 8,
                FillColor = AccentLogout,
                ForeColor = Color.White
            };
            logoutBtn.Click += LogoutBtn_Click;
            container.Controls.Add(logoutBtn);
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var newUrl = baseUrlBox.Text.Trim();
            _settings.ApiBaseUrl = newUrl;
            _settings.Save();

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
            var tokenFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.token");
            if (File.Exists(tokenFile))
                File.Delete(tokenFile);

            Application.Exit();
        }
    }
}
