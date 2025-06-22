using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Paste your list of trackers below and press Enter when done.");
        Console.WriteLine("Each tracker should be on its own line.");
        Console.WriteLine("For example:");
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine(
            "http://tracker.bt4g.com:2095/announce\n" +
            "https://tracker.yemekyedim.com:443/announce\n" +
            "udp://open.stealth.si:80/announce");
        Console.WriteLine("--------------------------------------------");

        var trackers = new List<string>();
        string? line;
        while (true)
        {
            line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                break;

            trackers.Add(line.Trim());
        }

        Console.WriteLine($"\nChecking {trackers.Count} trackers...\n");

        if (trackers.Any())
        {
            var checkTasks = trackers.Select(tracker =>
            {
                if (tracker.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    return TrackerChecker.CheckHttpTracker(tracker);
                else if (tracker.StartsWith("udp", StringComparison.OrdinalIgnoreCase))
                    return TrackerChecker.CheckUdpTracker(tracker);
                else
                {
                    Console.WriteLine($"[SKIPPED] Unknown protocol: {tracker}");
                    return Task.CompletedTask;
                }
            });

            await Task.WhenAll(checkTasks);

            Console.WriteLine("\n✅ Done!");
        }
    }
}
