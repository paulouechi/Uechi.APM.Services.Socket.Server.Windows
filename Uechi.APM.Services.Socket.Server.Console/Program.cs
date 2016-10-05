using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uechi.Socket.Library;

namespace Uechi.APM.Services.Socket.Server.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketServer objSck = new SocketServer();
            objSck.Iniciar(true);
        }
    }
}
