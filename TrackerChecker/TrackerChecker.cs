using System.Net;
using System.Net.Sockets;

public static class TrackerChecker
{

    public static async Task CheckHttpTracker(string url)
    {
        try
        {
            using HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(5);
            var response = await client.GetAsync(url);
            Console.WriteLine($"[HTTP {response.StatusCode}] {url}");
        }
        catch
        {
            Console.WriteLine($"[HTTP DOWN] {url}");
        }
    }

    public static async Task CheckUdpTracker(string url)
    {
        try
        {
            var uri = new Uri(url);
            var udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 5000;

            var endPoint = new IPEndPoint(Dns.GetHostAddresses(uri.Host)[0], uri.Port);

            byte[] connectRequest = new byte[16];
            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(0x41727101980)), 0, connectRequest, 0, 8);
            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(0)), 0, connectRequest, 8, 4);
            int transactionId = new Random().Next();
            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(transactionId)), 0, connectRequest, 12, 4);

            await udpClient.SendAsync(connectRequest, connectRequest.Length, endPoint);
            var result = await udpClient.ReceiveAsync();

            if (result.Buffer.Length >= 16)
            {
                int action = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(result.Buffer, 0));
                int responseTransactionId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(result.Buffer, 4));
                if (action == 0 && responseTransactionId == transactionId)
                {
                    Console.WriteLine($"[UDP OK] {url}");
                    return;
                }
            }

            Console.WriteLine($"[UDP FAIL] {url}");
        }
        catch
        {
            Console.WriteLine($"[UDP ERROR] {url}");
        }
    }
}