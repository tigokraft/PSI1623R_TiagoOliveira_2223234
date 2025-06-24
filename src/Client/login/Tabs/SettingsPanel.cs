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
        private readonly HttpClient _http;
        private readonly ClientSettings _settings;
        private TextBox baseUrlBox;

        public SettingsPanel(HttpClient http)
        {
            _http = http;
            _settings = ClientSettings.Load();
            InitializeComponent();
            baseUrlBox.Text = _settings.ApiBaseUrl;
        }

        private void InitializeComponent()
        {
            this.Text = "Settings";
            this.BackColor = Color.FromArgb(18, 20, 20);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(600, 200);

            var lbl = new Label
            {
                Text = "API Base URL",
                ForeColor = Color.White,
                Location = new Point(30, 30),
                AutoSize = true
            };
            baseUrlBox = new TextBox
            {
                Location = new Point(30, 60),
                Width = 400
            };
            var saveBtn = new Button
            {
                Text = "Save",
                Location = new Point(30, 100),
                Width = 80
            };
            var logoutBtn = new Button
            {
                Text = "Logout",
                Location = new Point(120, 100),
                Width = 80
            };

            saveBtn.Click += (s, e) =>
            {
                _settings.ApiBaseUrl = baseUrlBox.Text.Trim();
                _settings.Save();
                if (Uri.TryCreate(_settings.ApiBaseUrl, UriKind.Absolute, out var uri))
                {
                    _http.BaseAddress = uri;
                    MessageBox.Show("Settings saved.");
                }
                else
                {
                    MessageBox.Show("Invalid URL.");
                }
            };

            logoutBtn.Click += (s, e) =>
            {
                if (File.Exists("auth.token"))
                    File.Delete("auth.token");
                Application.Exit();
            };

            this.Controls.Add(lbl);
            this.Controls.Add(baseUrlBox);
            this.Controls.Add(saveBtn);
            this.Controls.Add(logoutBtn);
        }
    }
}