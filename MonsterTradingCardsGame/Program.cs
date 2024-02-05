using MonsterTradingCardsGame;
using MonsterTradingCardsGame.server;
using MonsterTradingCardsGame.Endpoints;
using System.Net;

Console.WriteLine("Our first simple HTTP-Server: http://localhost:10001/");

// ===== I. Start the HTTP-Server =====
HttpServer httpServer = new HttpServer(IPAddress.Any, 10001);
httpServer.RegisterEndpoint("users", new UsersEndpoint());
httpServer.Run();