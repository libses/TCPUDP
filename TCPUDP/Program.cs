using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TCPUDP
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var ips = new List<IPAddress>();
            Console.WriteLine("Write first IP in range");
            var startIp = ReverseIp(Console.ReadLine());
            Console.WriteLine("Write last IP in range");
            var endIp = ReverseIp(Console.ReadLine());
            var startParsedIp = IPAddress.Parse(startIp);
            var endParsedIp = IPAddress.Parse(endIp);
            var start = startParsedIp.GetAddressBytes();
            var end = endParsedIp.GetAddressBytes();
            var startLong = FromBytes(start);
            var endLong = FromBytes(end);
            for (long i = startLong; i <= endLong; i+=1)
            {
                var vs = ToBytes(i);
                var j = FromBytes(new byte[] {vs[3], vs[2], vs[1], vs[0]});
                ips.Add(new IPAddress(j));
            }

            //ips = new List<IPAddress>() { IPAddress.Parse("87.240.139.193"), IPAddress.Parse("87.240.139.194"), IPAddress.Parse("87.240.139.195") };
            Console.WriteLine("Write first port in range");
            var fPort = int.Parse(Console.ReadLine());
            Console.WriteLine("Write last port in range");
            var sPort = int.Parse(Console.ReadLine()) + 1;
            var tcpPorts = Enumerable.Range(fPort, sPort).ToArray();
            var udpPorts = Enumerable.Range(fPort, sPort).ToArray();
            foreach (var ip in ips)
            {
                await Scanner.ScanTCP(ip, tcpPorts);
                await Scanner.ScanUDP(ip, udpPorts);
            }
        }

        public static string ReverseIp(string ip)
        {
            var spl = ip.Split('.');
            return $"{spl[3]}.{spl[2]}.{spl[1]}.{spl[0]}";
        }
        
        public static long FromBytes(byte[] vs)
        {
            return (long)vs[3] * 256 * 256 * 256 + (long)vs[2] * 256 * 256 + (long)vs[1] * 256 + (long)vs[0];
        }

        public static byte[] ToBytes(long vs)
        {
            return new byte[] { (byte)(vs % 256), (byte)(vs / 256 % 256), (byte)(vs / (256 * 256) % 256), (byte)(vs / (256 * 256 * 256) % 256)};
        }
    }
}
