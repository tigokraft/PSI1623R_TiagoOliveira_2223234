using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;
using System.Runtime.InteropServices;

namespace login
{
    public partial class Register : Form
    {
        private readonly HttpClient _http;
        public Register(HttpClient httpClient)
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            _http = httpClient;
        }

        private async void LoginBtn_Click(object sender, EventArgs e)
        {
            string pwd = string.Empty;
            try
            {
                if (Pass.Text == rptPass.Text)
                {
                    pwd = Pass.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"error{ex.Message}");
            }

            var credentials = new
            {
                username = User.Text,
                password = pwd
            };

            var json = JsonSerializer.Serialize(credentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try // Added a try-catch for the HTTP request itself
            {
                var response = await _http.PostAsync("api/auth/register", content);
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
                            MessageBox.Show("Register failed: Token received was empty.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Register failed: Could not find token in response.");
                    }
                }
                else
                {
                    // Read error response if needed for more details
                    string errorBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Register failed: {response.StatusCode} - {errorBody}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Handle network or other HTTP-related errors
                MessageBox.Show($"Register failed due to network error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle other potential errors (e.g., file writing, JSON parsing)
                MessageBox.Show($"An unexpected error occurred during login: {ex.Message}");
            }
        }
    }
}
