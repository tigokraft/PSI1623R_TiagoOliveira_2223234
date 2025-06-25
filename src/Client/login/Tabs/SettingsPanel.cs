using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using login.Helpers;

namespace login.Tabs
{
    public class SettingsPanel : Form
    {
        private readonly HttpClient _http;
        private readonly ClientSettings _settings;
        private Guna2TextBox baseUrlBox;

        public SettingsPanel(HttpClient http)
        {
            _http = http;
            _settings = ClientSettings.Load();
            InitializeComponent();
            baseUrlBox.Text = _settings.ApiBaseUrl;
        }

        private void InitializeComponent()
        {
            // ── Form ────────────────────────────────────────────────────────────────
            this.Text = "Settings";
            this.BackColor = Color.FromArgb(18, 20, 20);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(900, 650);

            // ── Container ───────────────────────────────────────────────────────────
            var container = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                FillColor = Color.FromArgb(24, 26, 27),
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(40, 40, 40),
            };
            this.Controls.Add(container);

            // ── Header ──────────────────────────────────────────────────────────────
            var header = new Label
            {
                Text = "Settings",
                Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft
            };
            container.Controls.Add(header);

            // ── Two-column table for label + textbox ────────────────────────────────
            var table = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Top,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 20)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            container.Controls.Add(table);

            // Label
            var lbl = new Label
            {
                Text = "API Base URL",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(lbl, 0, 0);

            // TextBox
            baseUrlBox = new Guna2TextBox
            {
                PlaceholderText = "https://api.example.com",
                BorderRadius = 8,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(67, 79, 82),
                FillColor = Color.FromArgb(18, 20, 20),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 0, 0, 0),
                Height = 36
            };
            table.Controls.Add(baseUrlBox, 1, 0);

            // ── Button row ─────────────────────────────────────────────────────────
            var buttons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                AutoSize = true,
                WrapContents = false
            };
            container.Controls.Add(buttons);

            // Logout
            var logoutBtn = new Guna2Button
            {
                Text = "Logout",
                Size = new Size(100, 36),
                FillColor = Color.FromArgb(92, 26, 26),
                BorderColor = Color.FromArgb(112, 36, 36),
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(0, 0, 10, 0),
                Cursor = Cursors.Hand
            };
            logoutBtn.Click += (s, e) =>
            {
                if (File.Exists("auth.token")) File.Delete("auth.token");
                Application.Exit();
            };
            buttons.Controls.Add(logoutBtn);

            // Save
            var saveBtn = new Guna2Button
            {
                Text = "Save",
                Size = new Size(100, 36),
                FillColor = Color.FromArgb(20, 24, 26),
                BorderColor = Color.FromArgb(39, 42, 44),
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            saveBtn.Click += (s, e) =>
            {
                _settings.ApiBaseUrl = baseUrlBox.Text.Trim();
                _settings.Save();
                if (Uri.TryCreate(_settings.ApiBaseUrl, UriKind.Absolute, out var uri))
                {
                    _http.BaseAddress = uri;
                    Cards.Show("Success", "Settings saved.", "OK");
                }
                else
                {
                    Cards.Show("Error", "Invalid URL.", "OK");
                }
            };
            buttons.Controls.Add(saveBtn);
        }
    }
}
