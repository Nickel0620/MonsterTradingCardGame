﻿using MonsterTradingCardsGame.server;
using MonsterTradingCardsGame.user;
using System.Text.Json;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MonsterTradingCardsGame.Endpoints.UsersEndpoint;

namespace MonsterTradingCardsGame.Endpoints
{
    internal class SessionsEndpoint : IHttpEndpoint
    {
        private UserManager userManager; // UserManager instance

        public SessionsEndpoint()
        {
            userManager = new UserManager();
        }
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST)
            {
                LoginUsers(rq, rs);
                return true;
            }
           
            return false;
        }


        public void LoginUsers(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                var loginRequest = JsonSerializer.Deserialize<LoginRequest>(rq.Content ?? "");

                bool loginSuccess = userManager.Login(loginRequest.Username, loginRequest.Password);

                if (loginSuccess)
                {
                    rs.ResponseCode = 200; // OK
                    rs.ResponseMessage = "Login successful";
                    rs.Content = JsonSerializer.Serialize(new { Message = "Login successful" });
                }
                else
                {
                    rs.ResponseCode = 401; // Unauthorized
                    rs.Content = JsonSerializer.Serialize(new { Error = "Invalid username or password" });
                }
            }
            catch (Exception)
            {
                rs.ResponseCode = 400; // Bad Request
                rs.Content = JsonSerializer.Serialize(new { Error = "Failed to parse request data!" });
            }

            rs.Headers.Add("Content-Type", "application/json");
        }

    }
}
