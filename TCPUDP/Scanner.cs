using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPUDP
{
    internal class Scanner
    {
        public static async Task ScanTCP(IPAddress ip, int[] ports)
        {
            List<Task> tasks = new();
            foreach (var port in ports)
            {
                var dest = new IPEndPoint(ip, port);
                tasks.Add(ScanPortTCP(dest, 100));
            }

            await Task.WhenAll(tasks);
        }

        public static async Task ScanPortTCP(IPEndPoint ipPort, int timeout)
        {
            using var client = new TcpClient();
            try
            {
                var ct = new CancellationTokenSource(timeout);
                var res = client.ConnectAsync(ipPort, ct.Token);
                await res;
                
                if (res.IsCompletedSuccessfully)
                {
                    Console.WriteLine($"{ipPort} TCP open");
                }
            }
            catch
            {

            }
            finally
            {
                client.Close();
                client.Dispose();
            }
        }

        public static async Task ScanPortUDP(IPEndPoint ipPort, int timeout)
        {
            try
            {
                using var client = new UdpClient(new IPEndPoint(IPAddress.Parse("192.168.0.102"), ipPort.Port));
                var ct = new CancellationTokenSource(timeout);
                var send = client.SendAsync(new byte[] { 123 }, ipPort, ct.Token);
                await send;
                var recieve = client.ReceiveAsync(ct.Token);
                await recieve;

                if (recieve.IsCompletedSuccessfully)
                {
                    Console.WriteLine($"{ipPort} UDP closed");
                }
            }
            catch
            {
                
            }
        }

        public static async Task ScanUDP(IPAddress ip, int[] ports)
        {
            List<Task> tasks = new();
            foreach (var port in ports)
            {
                var dest = new IPEndPoint(ip, port);
                tasks.Add(ScanPortUDP(dest, 300));
            }

            await Task.WhenAll(tasks);
        }
    }
}
