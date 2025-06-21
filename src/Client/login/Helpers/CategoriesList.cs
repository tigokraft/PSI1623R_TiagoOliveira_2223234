using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace login.Helpers
{
    internal static class CategoriesList
    {
        public class Category
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string Color { get; set; }
        }

        public static async Task<List<Category>> GetCategoriesAsync(HttpClient http)
        {
            try
            {
                var response = await http.GetAsync("api/category");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        $"API call failed ({response.StatusCode})",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return new List<Category>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var categories = JsonSerializer
                    .Deserialize<List<Category>>(json, options)
                    ?? new List<Category>();

                return categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error fetching categories: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return new List<Category>();
            }
        }
    }
}
