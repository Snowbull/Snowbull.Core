using System;
using System.Net;

namespace Snowbull {
    class Program {
        public static void Main(string[] args) {
            Snowbull server = new Snowbull(IPAddress.Loopback, 9000);
            Console.ReadLine();
        }
    }
}
