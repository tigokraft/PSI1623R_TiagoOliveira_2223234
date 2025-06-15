using System.Diagnostics;
using System.Threading.Tasks;

namespace FinSync.Panel;

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
            Console.WriteLine("============================");
            Console.WriteLine("         FINSYNC API        ");
            Console.WriteLine("============================");
            Console.WriteLine("1 -> Requests check");
            Console.WriteLine("2 -> Admin Panel");
            Console.WriteLine("3 -> STOP");
            Console.WriteLine("4 -> Restart");
            Console.WriteLine("============================");
            Console.Write("Select an option: ");

            var key = Console.ReadKey().Key;
            Console.WriteLine();

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
                    Console.WriteLine("Invalid option.");
                    break;
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }

    private static async Task CheckRequests()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("http://localhost:5000/health"); // Adjust port as needed
            Console.WriteLine(response.IsSuccessStatusCode
                ? "✅ API is healthy."
                : $"❌ API error: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to check API: {ex.Message}");
        }
    }

    private static void ShowAdminPanel()
    {
        Console.WriteLine("🛠️ Admin Panel:");
        Console.WriteLine("- Alerts count: TODO");
        Console.WriteLine("- Jobs running: TODO");
        Console.WriteLine("- Users online: TODO");
    }

    private static void StopAPI()
    {
        Console.WriteLine("🛑 Stopping FinSync...");
        Environment.Exit(0);
    }

    private static void RestartAPI()
    {
        Console.WriteLine("🔄 Triggering restart...");
        Environment.Exit(100); // Trigger loop in start.bat
    }
}
