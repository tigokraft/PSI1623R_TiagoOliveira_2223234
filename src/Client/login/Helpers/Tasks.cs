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

namespace login.Helpers
{
    internal class Tasks : MainForm
    {
        public Tasks(HttpClient httpClient) : base(httpClient)
        {

        }

        // request classes
        public class IncomeRequest
        {
            public decimal amount { get; set; }
            public DateTime date { get; set; }
            public string descr { get; set; }
            public bool isRecurring { get; set; }
            public string recurrence { get; set; }
            public string endDate { get; set; }
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
                        MessageBox.Show($"Rate limited. Try again after {retryAfterSec} seconds.", "Too Many Requests", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Rate limited. Please wait a few seconds before trying again.", "Too Many Requests", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        // Example: you can now use these to update your labels
                        decimal monthlySpent = expenseResponse.TotalMonthlySpent;
                        decimal allTimeSpent = expenseResponse.TotalAllTimeSpent;

                        Console.WriteLine($"Monthly: {monthlySpent}, All Time: {allTimeSpent}");

                        // You can now assign these to your labels or UI
                        // labelMonthlySpent.Text = $"${monthlySpent}";
                        // labelAllTimeSpent.Text = $"${allTimeSpent}";
                    }
                }
                else
                {
                    MessageBox.Show($"API call failed: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching expenses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task PostIncome(string description, decimal amt, string endTime, decimal catId, bool isRec, string rec, HttpClient http)
        {
            var payload = new IncomeRequest
            {
                amount = amt,
                date = DateTime.Now,
                descr = description,
                isRecurring = isRec,
                recurrence = rec,
                endDate = endTime
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await http.PostAsync("api/income/", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                //return Cards.
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Failed: {response.StatusCode} - {error}");
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
