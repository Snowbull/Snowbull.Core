using System;
using System.Net;

namespace Snowbull {
    class Program {
        public static void Main(string[] args) {
            Snowbull server = new Snowbull(IPAddress.Any, 9000);
            Console.ReadLine();
        }
    }
}
