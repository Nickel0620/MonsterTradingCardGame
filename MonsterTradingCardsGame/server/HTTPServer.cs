using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.server
{
    internal class HTTPServer
    {
        public IPAddress Adress { get; set; }
        public int Port { get; set; }

        public HTTPServer(IPAddress adress, int port)
        {
            Adress = adress;
            Port = port;
        }

        public void RunServer()
        {
            // ===== I. Start the HTTP-Server =====
            var httpServer = new TcpListener(Adress, Port);
            httpServer.Start();

            while (true)
            {
                var clientSocket = httpServer.AcceptTcpClient();

                ClientProcessor clientProcessor = new ClientProcessor(clientSocket);
                clientProcessor.ProcessClient();
            }





            //server und client in eigene klassen aufteilen
            //request parser auch eigene klasse???
            //HTTP server  POST http://localhost:10001/users //CURL von moodle

            //.yaml --> swagger editor zum lesen

            //Server ist single threaded, multithread einbauen


            //REST verwendet Token zur authentification

        }
    }
}
