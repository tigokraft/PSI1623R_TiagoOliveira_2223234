using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace login
{
    public partial class Form1 : Form
    {
        private readonly HttpClient httpClient;
        private readonly HttpClient _http;
        public Form1(HttpClient httpClient)
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            _http = httpClient;
        }

        private async void LoginBtn_Click(object sender, EventArgs e)
        {
            var credentials = new
            {
                username = txtUser.Text,
                password = passtxt.Text
            };

            var json = JsonSerializer.Serialize(credentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try // Added a try-catch for the HTTP request itself
            {
                var response = await _http.PostAsync("api/auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var tokenJson = await response.Content.ReadAsStringAsync();
                    // Use nullable reference types check for safety
                    var doc = JsonDocument.Parse(tokenJson);
                    if (doc.RootElement.TryGetProperty("token", out JsonElement tokenElement) && tokenElement.ValueKind == JsonValueKind.String)
                    {
                        var token = tokenElement.GetString();

                        if (!string.IsNullOrEmpty(token)) // Basic check for the token string
                        {
                            File.WriteAllText("auth.token", token);
                            // Assuming _http is the correct HttpClient instance used in Program.cs
                            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                            // Set the DialogResult to OK before closing
                            this.DialogResult = DialogResult.OK;
                            this.Close(); // Close the login form
                        }
                        else
                        {
                            MessageBox.Show("Login failed: Token received was empty.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Login failed: Could not find token in response.");
                    }
                }
                else
                {
                    // Read error response if needed for more details
                    string errorBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Login failed: {response.StatusCode} - {errorBody}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Handle network or other HTTP-related errors
                MessageBox.Show($"Login failed due to network error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle other potential errors (e.g., file writing, JSON parsing)
                MessageBox.Show($"An unexpected error occurred during login: {ex.Message}");
            }
        }

        private void resetpwd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPwd forgot = new ForgotPwd();
            //forgot.FormClosed += new FormClosedEventHandler(resetClosed);

            //this.Hide();
            forgot.Show();
        }
        private void resetClosed(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void pwdShow_CheckedChanged(object sender, EventArgs e)
        {
            if (pwdShow.Checked)
            {
                passtxt.UseSystemPasswordChar = false;
            }
            else
            {
                passtxt.UseSystemPasswordChar = true;
            }
        }
    }
}
