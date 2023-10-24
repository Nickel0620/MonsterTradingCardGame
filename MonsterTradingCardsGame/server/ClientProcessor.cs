using System.Net.Sockets;
using System.Text;
//hier muss man Monstercard einbauen, alle anderen sind allgemein und ok
namespace MonsterTradingCardsGame.server
{
    public class ClientProcessor
    {

        private TcpClient clientSocket;

        public ClientProcessor(TcpClient clientSocket)
        {
            this.clientSocket = clientSocket;
        }

        public void ProcessClient()
        {

            // ----- 0. Accept the TCP-Client and create the reader and writer -----
            using var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
            using var reader = new StreamReader(clientSocket.GetStream());

            HTTPRequest request = new HTTPRequest(reader);
            request.ParseRequest();

            // ----- 2. Do the processing -----
            HTTPResponse response = new HTTPResponse(writer);
            if(request.Path == "/Users")
            {
                response.Content = "Users created";
            }   else
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Not Found";
            }
            //if else if else ....
            // .... 

            Console.WriteLine("----------------------------------------");
            response.SendResponse();
        }
    }
}