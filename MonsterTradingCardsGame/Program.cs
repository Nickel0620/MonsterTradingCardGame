using MonsterTradingCardsGame.server;

namespace MonsterTradingCardsGame

{
    internal class Program
    {
        static void Main(string[] args)
        {
            HTTPServer server = new HTTPServer(System.Net.IPAddress.Any, 10001);
            server.RunServer();
        }
    }
}