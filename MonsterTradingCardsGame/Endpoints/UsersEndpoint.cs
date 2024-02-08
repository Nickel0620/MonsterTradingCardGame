using System.Text.Json;
using MonsterTradingCardsGame.server;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using MonsterTradingCardsGame.user;

namespace MonsterTradingCardsGame.Endpoints
{
    public class UsersEndpoint : IHttpEndpoint
    {
        private UserManager userManager; // UserManager instance

        public UsersEndpoint()
        {
            userManager = new UserManager();
        }

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
            try
            {
                var loginRequest = JsonSerializer.Deserialize<LoginRequest>(rq.Content ?? "");

                User user = userManager.ValidateUserLogin(loginRequest.Username, loginRequest.Password);

                if (user != null)
                {
                    rs.Content = JsonSerializer.Serialize(user);
                    rs.ResponseCode = 200; // OK
                    rs.ResponseMessage = "Login successful";
                }
                else
                {
                    rs.ResponseCode = 401; // Unauthorized
                    rs.Content = "Invalid username or password";
                }
            }
            catch (Exception)
            {
                rs.ResponseCode = 400; // Bad Request
                rs.Content = "Failed to parse request data!";
            }

            rs.Headers.Add("Content-Type", "application/json");
        }

        // Helper class to represent the login request
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
