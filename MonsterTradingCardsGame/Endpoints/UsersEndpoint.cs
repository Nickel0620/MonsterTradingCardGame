using System.Text.Json;
using MonsterTradingCardsGame.server;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using MonsterTradingCardsGame.user;

namespace MonsterTradingCardsGame.Endpoints
{
    public class UsersEndpoint : IHttpEndpoint
    {
        //user manager = new UserManager();
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST)
            {
                CreateUser(rq, rs);
                return true;
            }
            else if (rq.Method == HttpMethod.GET)
            {
                GetUsers(rq, rs);
                return true;
            }
            return false;
        }


        public void CreateUser(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                var user = JsonSerializer.Deserialize<User>(rq.Content ?? "");

                // call BL

                rs.ResponseCode = 201;
                rs.ResponseMessage = "OK";
            }
            catch (Exception)
            {
                rs.ResponseCode = 400;
                rs.Content = "Failed to parse User data! ";
            }
        }

        public void GetUsers(HttpRequest rq, HttpResponse rs)
        {
            //rq -> ami bemegy request a serverhez (böngészö) 
            //rs -> válasz a szervertöl (cliensnek)
            rs.Content = JsonSerializer.Serialize(new User[] { new User() { Id = 1, Elo = 100, Coins = 20, Name = "Max Muster", Password = "1234" } });
            rs.Headers.Add("Content-Type", "application/json");
        }
    }
}
