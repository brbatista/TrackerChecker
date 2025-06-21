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
        var trackers = new List<string>
        {
            
        };

        foreach (var tracker in trackers)
        {
            if (tracker.StartsWith("http"))
            {
                await TrackerChecker.CheckHttpTracker(tracker);
            }
            else if (tracker.StartsWith("udp"))
            {
                await TrackerChecker.CheckUdpTracker(tracker);
            }
        }
    }
}
