using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinSync.Panel
{
    public static class ConsolePanel
    {
        public static void Start()
        {
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
                var response = await client.GetAsync("http://localhost:5000/health");
                centerBlock(new[] { response.IsSuccessStatusCode ? "✅ API is healthy." : $"❌ API error: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                centerBlock(new[] { $"❌ Failed to check API: {ex.Message}" });
            }
        }

        private static void ShowAdminPanel()
        {
            centerBlock(new[]
            {
                "🛠️ Admin Panel:",
                "- Alerts count: TODO",
                "- Jobs running: TODO",
                "- Users online: TODO"
            });
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
