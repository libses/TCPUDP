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
                tasks.Add(ScanPortTCP(dest, 1000));
            }

            await Task.WhenAll(tasks);
        }

        public static async Task ScanPortTCP(IPEndPoint ipPort, int timeout)
        {
            var message = "someverybigmessagewithalotofmesssymbolsandihopeitisbiglmaobiggerthanudpatleastimtiredofdoingit".Select(x => (int)x).Select(x => (byte)x).ToArray();
            var sb = new StringBuilder();
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
                var stream = client.GetStream();
                var data = new Byte[256];
                var write = stream.WriteAsync(message);
                await write;
                var i = 1;
                while (i != 0)
                {
                    var read = stream.ReadAsync(data, 0, data.Length, ct.Token);
                    await read;
                    var strr = Encoding.ASCII.GetString(data, 0, read.Result);
                    sb.Append(strr);
                    i = read.Result;
                }   
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                
            }
            finally
            {
                var rand = new Random();
                var r = rand.Next(0, 100);
                client.Close();
                client.Dispose();
                var str = sb.ToString();
                if (str.Length > 0)
                {
                    var b = str.ToCharArray().Select(x => (int)x);
                    if (b.Count(x => x == 63) > 4)
                    {
                        Console.WriteLine($"{ipPort} WE HAVE TELNET!");
                    }
                }
                
                if (str.Contains("SSH"))
                {
                    Console.WriteLine($"{ipPort} GET SSH!!!");
                }
                else if (str.Contains("FTP"))
                {
                    Console.WriteLine($"{ipPort} HAVE FTP!!!");
                }
                else if (str.Contains("HTTP"))
                {
                    Console.WriteLine($"{ipPort} HTTP DETECTED");
                }
                else if (str.Contains("SMTP"))
                {
                    Console.WriteLine($"{ipPort} SMTP SPAM SPAM SPAM");
                }
                else if (str.Contains("POP"))
                {
                    Console.WriteLine($"{ipPort} POP POP POP POP POP IMA CAT");
                }
            }
        }

        public static async Task ScanPortUDP(IPEndPoint ipPort, int timeout)
        {
            byte[] buffer = new byte[1];
            var message = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa".Select(x => (int)x).Select(x => (byte)x).ToArray();
            try
            {
                using var client = new UdpClient(new IPEndPoint(IPAddress.Parse("192.168.0.102"), ipPort.Port));
                var ct = new CancellationTokenSource(timeout);
                var send = client.SendAsync(message, ipPort, ct.Token);
                await send;
                var recieve = client.ReceiveAsync(ct.Token);
                await recieve;

                if (recieve.IsCompletedSuccessfully)
                {
                    Console.WriteLine($"{ipPort} UDP returned something or closed.");
                    buffer = recieve.Result.Buffer;
                }
            }
            catch
            {
                
            }
            if (buffer.Length > 34 && buffer[34] == 3)
            {
                Console.WriteLine("PORT IS CLOSED!!!");
            }
            else if (buffer.Length > 3)
            {
                if (buffer[0] == 33 && buffer[1] == 2)
                {
                    Console.WriteLine($"{ipPort} We have sntp!");
                }
                else
                {
                    Console.WriteLine("UDP return:");
                    Console.WriteLine(Encoding.ASCII.GetString(buffer));
                }
            }
        }

        public static async Task ScanUDP(IPAddress ip, int[] ports)
        {
            List<Task> tasks = new();
            foreach (var port in ports)
            {
                var dest = new IPEndPoint(ip, port);
                tasks.Add(ScanPortUDP(dest, 5000));
            }

            await Task.WhenAll(tasks);
        }
    }
}
