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
                ShowUserData(rq, rs);
                return true;
            }
            else if (rq.Method == HttpMethod.PUT)
            {
                EditUserInfo(rq, rs);
                return true;
            }
            return false;
        }


        public void CreateUser(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                var registrationRequest = JsonSerializer.Deserialize<RegistrationRequest>(rq.Content ?? "");
                
               
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

        public void ShowUserData(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                string username = ExtractUsername(rq.Headers["Authorization"]);
                if (username != null)
                {
                    var user = userManager.GetUserFromDatabase(username);
                    if (user != null)
                    {
                        rs.ResponseCode = 200; // OK
                        rs.Content = JsonSerializer.Serialize(user);
                    }
                    else
                    {
                        rs.ResponseCode = 404; 
                        rs.Content = "User not Found!";
                    }
                }
                else
                {
                    rs.ResponseCode = 401; // Unauthorized
                    rs.Content = "Unauthorized access!";
                }
            }
            catch (Exception ex)
            {
                rs.ResponseCode = 400; // Bad Request
                rs.Content = "Failed to process request: " + ex.Message;
            }
        }

        public void EditUserInfo(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                string username = ExtractUsername(rq.Headers["Authorization"]);
                if (username != null)
                {
                    var user = JsonSerializer.Deserialize<User>(rq.Content ?? "");
                    if (userManager.EditUser(user, username))
                    {
                        rs.ResponseCode = 200; // OK
                        rs.Content = "User data updated successfully!";
                    }
                    else
                    {
                        rs.ResponseCode = 404; // Bad Request
                        rs.Content = "User not Found!";
                    }
                }
                else
                {
                    rs.ResponseCode = 401; // Unauthorized
                    rs.Content = "Unauthorized access!";
                }
            }
            catch (Exception ex)
            {
                rs.ResponseCode = 400; // Bad Request
                rs.Content = "Failed to process request: " + ex.Message;
            }

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

        private string ExtractUsername(string authHeader)
        {
            if (authHeader != null)
            {
                return authHeader.Replace("Bearer ", "").Replace("-mtcgToken", "").Trim();
            }
            return null;
        }
    }
}
