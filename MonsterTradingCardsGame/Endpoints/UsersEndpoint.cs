using System.Text.Json;
using MonsterTradingCardsGame.server;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            else if (rq.Method == HttpMethod.PUT)
            {
                EditUsers(rq, rs);
                return true;
            }
            return false;
        }


        public void CreateUser(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                var registrationRequest = JsonSerializer.Deserialize<RegistrationRequest>(rq.Content ?? "");
                var AuthHeader = rq.Headers["Authorization"];
               
                if (userManager.IsUsernameTaken(registrationRequest.Username))
                {
                    rs.ResponseCode = 409; // Conflict
                    rs.Content = "Username is already taken. Please choose a different username.";
                }
                else
                {
                    userManager.Register(registrationRequest.Username, registrationRequest.Name, registrationRequest.Password);
                    rs.ResponseCode = 201; // Created
                    rs.Content = "Registration successful!";
                }
            }
            catch (Exception ex)
            {
                rs.ResponseCode = 400; // Bad Request
                rs.Content = "Failed to process registration: " + ex.Message;
            }

            rs.Headers.Add("Content-Type", "application/json");
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

        public void EditUsers(HttpRequest rq, HttpResponse rs)
        {

        }

        //helper class to represent the registration request
        public class RegistrationRequest
        {
            public string Username { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
        }

        // Helper class to represent the login request
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
