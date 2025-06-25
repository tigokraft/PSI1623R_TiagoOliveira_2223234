using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Text;
using File = System.IO.File;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Windows.Forms;

namespace login.Helpers
{
    internal class Tasks : MainForm
    {
        public Tasks(HttpClient httpClient) : base(httpClient)
        {

        }

        public class ExpenseRequest
        {
            public decimal Amount { get; set; }
            public string Tags { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public decimal Category { get; set; }
        }

        public class Expense
        {
            public int ExpenseId { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public string Tags { get; set; }
            public string CategoryName { get; set; }
        }

        public class ExpenseResponse
        {
            public List<Expense> Expenses { get; set; }
            public decimal TotalMonthlySpent { get; set; }
            public decimal TotalAllTimeSpent { get; set; }
        }

        public class Category
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
        }

        public async Task GetExpensesAsync(HttpClient http)
        {
            var token = LoadToken();

            try
            {
                var response = await http.GetAsync("api/expense/summary");

                if ((int)response.StatusCode == 429)
                {
                    // Optional: check for Retry-After header
                    if (response.Headers.TryGetValues("Retry-After", out var retryValues))
                    {
                        int retryAfterSec = int.Parse(retryValues.First());
                        Cards.Show("Too Many Requests", $"Rate limited. Try again after {retryAfterSec} seconds.", "OK");
                    }
                    else
                    {
                        Cards.Show("Too Many Requests", "Rate limited. Please wait a few seconds before trying again.", "OK");
                    }
                    return;
                }

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var expenseResponse = JsonSerializer.Deserialize<ExpenseResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (expenseResponse != null)
                    {
                        // You can now use these to update your labels/UI as needed
                        decimal monthlySpent = expenseResponse.TotalMonthlySpent;
                        decimal allTimeSpent = expenseResponse.TotalAllTimeSpent;

                        Console.WriteLine($"Monthly: {monthlySpent}, All Time: {allTimeSpent}");

                        // labelMonthlySpent.Text = $"${monthlySpent}";
                        // labelAllTimeSpent.Text = $"${allTimeSpent}";
                    }
                }
                else
                {
                    Cards.Show("Error", $"API call failed: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                Cards.Show("Error", $"Error fetching expenses:\n{ex.Message}", "OK");
            }
        }


        public static async Task<bool> PostIncome
        (
            decimal amount,
            string date,
            string description,
            bool isRecurring,
            string recurrence,
            string endDate,
            HttpClient http,
            int categoryId
        )
        {
            var payload = new
            {
                amount = amount,
                date = date,
                descr = description,
                isRecurring = isRecurring,
                recurrence = recurrence,
                endDate = endDate,
                categoryId = categoryId
            };

            var json = JsonSerializer.Serialize(payload);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                try
                {
                    var response = await http.PostAsync("api/income/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        Cards.Show("Success", "Income added successfully!", "OK");
                        return true;
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Cards.Show("Error", $"Failed to add income:\n{response.StatusCode}\n{error}", "OK");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Cards.Show("Error", $"Error posting income:\n{ex.Message}", "OK");
                    return false;
                }
            }
        }




        public static string LoadToken()
        {
            string tokenPath = "auth.token";

            if (File.Exists(tokenPath))
                return File.ReadAllText(tokenPath);

            return null;
        }
    }
}
