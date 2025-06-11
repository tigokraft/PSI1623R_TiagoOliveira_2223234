using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.IO;

namespace login
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string tokenPath = "auth.token";
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5034")
            };
            httpClient.DefaultRequestHeaders.Add("x-api-key", "12345-abcdef-67890");

            bool tokenValidAndUsed = false;

            try
            {
                //MessageBox.Show("getting token");

                if (File.Exists(tokenPath))
                {
                    try
                    {
                        string token = File.ReadAllText(tokenPath);
                        if (IsTokenValid(token))
                        {
                            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                            tokenValidAndUsed = true;
                        }
                        else
                        {
                            //MessageBox.Show("Existing token is invalid or expired. Please log in again.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading or validating token file: {ex.Message}");
                    }
                }
                else
                {
                    //MessageBox.Show("Token file not found. Please log in.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred during setup: {ex.Message}");
            }

            if (tokenValidAndUsed)
            {
                Application.Run(new MainForm(httpClient));
            }
            else
            {
                Form1 loginForm = new Form1(httpClient);

                DialogResult loginResult = loginForm.ShowDialog();

                if (loginResult == DialogResult.OK)
                {
                    Application.Run(new MainForm(httpClient));
                }
                else if (loginResult == DialogResult.No)
                {
                    Register registerForm = new Register(httpClient);
                    DialogResult registerResult = registerForm.ShowDialog();

                    if (registerResult == DialogResult.OK)
                    {
                        Application.Run(new MainForm(httpClient));
                    }
                }
            }
        }

        static bool IsTokenValid(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp");
                if (expClaim != null && long.TryParse(expClaim.Value, out long exp))
                {
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(exp);
                    return expDate > DateTimeOffset.UtcNow;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }


    }
}
