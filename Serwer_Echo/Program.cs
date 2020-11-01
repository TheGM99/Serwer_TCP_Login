using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serwer_Echo_Lib;

namespace Serwer_Echo
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerLoginAPM server = new ServerLoginAPM(IPAddress.Parse("127.0.0.1"), 2048);
            server.Start();
        }
    }
}
