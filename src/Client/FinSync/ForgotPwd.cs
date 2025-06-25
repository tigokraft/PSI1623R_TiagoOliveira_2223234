using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace login
{
    public partial class ForgotPwd : Form
    {
        private readonly HttpClient _http;

        public ForgotPwd(HttpClient httpClient, string username)
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            _http = httpClient;
            userBox.Text = username;
        }

        private async void LoginBtn_Click(object sender, EventArgs e)
        {
            if (txtUser.Text != guna2TextBox1.Text)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            var payload = new
            {
                username = userBox.Text,
                newPassword = txtUser.Text
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("api/auth/reset-password", content);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Password reset successfully.");
                    this.Close();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed: {response.StatusCode} - {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void pwdShow_CheckedChanged(object sender, EventArgs e)
        {
            bool show = pwdShow.Checked;
            txtUser.UseSystemPasswordChar = !show;
            guna2TextBox1.UseSystemPasswordChar = !show;
        }
    }
}
