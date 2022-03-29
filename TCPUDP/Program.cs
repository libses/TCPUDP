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
            var startIp = "0.8.8.8";
            var endIp = "8.8.8.8";
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

            var tcpPorts = Enumerable.Range(0, 800).ToArray();
            var udpPorts = Enumerable.Range(0, 800).ToArray();
            foreach (var ip in ips)
            {
                await Scanner.ScanTCP(ip, tcpPorts);
                await Scanner.ScanUDP(ip, udpPorts);
            }
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
