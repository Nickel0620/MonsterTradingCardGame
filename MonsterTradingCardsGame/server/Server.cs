using MonsterTradingCardsGame.server;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


Console.WriteLine("Our first simple HTTP-Server: http://localhost:10001/");

HTTPServer server = new HTTPServer(IPAddress.Loopback, 10001);
server.RunServer();

return;

