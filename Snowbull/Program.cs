using System;
using System.Net;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Entity.Infrastructure;

namespace Snowbull {
    class Program {
        public static void Main(string[] args) {
            Snowbull server = new Snowbull(IPAddress.Any, 9000);
        }
    }
}
