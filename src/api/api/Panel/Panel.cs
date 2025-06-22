using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FinSync.Data;
using Quartz;
using System.Linq;

namespace FinSync.Panel
{
    public static class ConsolePanel
    {
        private static IServiceProvider _services;

        public static void Start(IServiceProvider services)
        {
            _services = services;
            Task.Run(() => RunMenu());
        }

        private static void RunMenu()
        {
            while (true)
            {
                Console.Clear();
                Render();

                string[] menuItems = new[]
                {
                    "1 -> Requests check",
                    "2 -> Admin Panel",
                    "3 -> STOP",
                    "4 -> Restart"
                };

                centerBlock(menuItems);

                Console.CursorVisible = false;

                var key = Console.ReadKey(true).Key;
                Console.Clear();

                switch (key)
                {
                    case ConsoleKey.D1:
                        CheckRequests().Wait();
                        break;
                    case ConsoleKey.D2:
                        ShowAdminPanel();
                        break;
                    case ConsoleKey.D3:
                        StopAPI();
                        break;
                    case ConsoleKey.D4:
                        RestartAPI();
                        break;
                    default:
                        centerBlock(new[] { "Invalid option." });
                        break;
                }

                centerBlock(new[] { "\nPress any key to return to menu..." });
                Console.ReadKey(true);
            }
        }

        // Centers the entire block of strings as a group horizontally
        private static void centerBlock(string[] lines)
        {
            int consoleWidth = Console.WindowWidth;
            int maxLength = 0;

            foreach (var line in lines)
                if (line.Length > maxLength)
                    maxLength = line.Length;

            int leftPadding = (consoleWidth - maxLength) / 2;
            if (leftPadding < 0) leftPadding = 0;

            foreach (var line in lines)
                Console.WriteLine(new string(' ', leftPadding) + line);
        }

        private static async Task CheckRequests()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync("http://localhost:5034/health");
                centerBlock(new[] { response.IsSuccessStatusCode ? "✅ API is healthy." : $"❌ API error: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                centerBlock(new[] { $"❌ Failed to check API: {ex.Message}" });
            }
        }

        private static void ShowAdminPanel()
        {
            Console.Write("Admin username: ");
            string user = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Admin password: ");
            string pass = ReadPassword();

            using var scope = _services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<FinSyncContext>();
            var admin = ctx.Users.FirstOrDefault(u => u.Username == user && u.Role == "admin");
            if (admin == null || !PasswordHelper.VerifyPassword(admin.PasswordHash, pass))
            {
                centerBlock(new[] { "Invalid credentials." });
                return;
            }
            var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
            var scheduler = schedulerFactory.GetScheduler().Result;
            var triggers = scheduler.GetTriggersOfJob(new JobKey("RecurringIncomeJob")).Result;
            var nextRun = triggers.FirstOrDefault()?.GetNextFireTimeUtc()?.ToLocalTime();

            var lines = new[]
            {
                "🛠️ Admin Panel:",
                $"- Users: {ctx.Users.Count()}",
                $"- Expenses: {ctx.Expenses.Count()}",
                $"- Incomes: {ctx.Incomes.Count()}",
                $"- Budgets: {ctx.Budgets.Count()}",
                $"- Active Schedules: {ctx.RecurringIncomeSchedules.Count(r => r.IsActive)}",
                $"- Next RecurringIncomeJob: {nextRun}"
            };

            centerBlock(lines);
        }

        private static string ReadPassword()
        {
            var sb = new StringBuilder();
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }

        private static void StopAPI()
        {
            centerBlock(new[] { "🛑 Stopping FinSync..." });
            Environment.Exit(0);
        }

        private static void RestartAPI()
        {
            centerBlock(new[] { "🔄 Triggering restart..." });
            Environment.Exit(100);
        }

        private const string AsciiArt = @"
            ·▄▄▄▪   ▐ ▄ .▄▄ ·  ▄· ▄▌ ▐ ▄  ▄▄·      ▄▄▄·  ▄▄▄·▪  
            ▐▄▄·██ •█▌▐█▐█ ▀. ▐█▪██▌•█▌▐█▐█ ▌▪    ▐█ ▀█ ▐█ ▄███ 
            ██▪ ▐█·▐█▐▐▌▄▀▀▀█▄▐█▌▐█▪▐█▐▐▌██ ▄▄    ▄█▀▀█  ██▀·▐█·
            ██▌.▐█▌██▐█▌▐█▄▪▐█ ▐█▀·.██▐█▌▐███▌    ▐█ ▪▐▌▐█▪·•▐█▌
            ▀▀▀ ▀▀▀▀▀ █▪ ▀▀▀▀   ▀ • ▀▀ █▪·▀▀▀      ▀  ▀ .▀   ▀▀▀
        ";

        public static void Render()
        {
            var startColor = (R: 0, G: 255, B: 0);
            var endColor = (R: 0, G: 0, B: 255);

            string[] lines = AsciiArt.Split('\n');
            int maxWidth = 0;
            foreach (var line in lines)
                if (line.Length > maxWidth)
                    maxWidth = line.Length;

            foreach (var line in lines)
            {
                int spaces = (Console.WindowWidth - line.Length) / 2;
                if (spaces < 0) spaces = 0;

                Console.Write(new string(' ', spaces - 5));

                for (int i = 0; i < line.Length; i++)
                {
                    double ratio = (double)i / maxWidth;
                    int r = (int)(startColor.R + (endColor.R - startColor.R) * ratio);
                    int g = (int)(startColor.G + (endColor.G - startColor.G) * ratio);
                    int b = (int)(startColor.B + (endColor.B - startColor.B) * ratio);

                    Console.Write($"\x1b[38;2;{r};{g};{b}m{line[i]}");
                }
                Console.WriteLine();
            }

            Console.Write("\x1b[0m");
        }
    }
}
