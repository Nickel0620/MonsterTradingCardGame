using MonsterTradingCardsGame.server;
using System.Text.Json;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Endpoints
{
    internal class ScoreboardEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET)
            {
                GetScores(rq, rs);
                return true;
            }

            return false;
        }

        public void GetScores(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                string username = ExtractUsername(rq.Headers["Authorization"]);

                if (username != null)
                {
                    var user = new UserManager();
                    var scores = user.GetScores(username);

                    if (scores != null)
                    {
                        rs.ResponseCode = 200; // OK
                        rs.Content = JsonSerializer.Serialize(scores);
                    }
                    else
                    {
                        rs.ResponseCode = 400; // Bad Request
                        rs.Content = "Failed to retrieve scores!";
                    }
                }
                else
                {
                    rs.ResponseCode = 401; // Unauthorized
                    rs.Content = "Unauthorized access!";
                }
            }
            catch (Exception)
            {
                rs.ResponseCode = 400; // Bad Request
                rs.Content = "Failed to parse request data!";
            }
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
