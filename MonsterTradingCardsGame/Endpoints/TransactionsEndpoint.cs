using MonsterTradingCardsGame.server;
using System.Text.Json;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.cards;


namespace MonsterTradingCardsGame.Endpoints
{
    internal class TransactionsEndpoint : IHttpEndpoint
    {
        private BuyPackage buyPackage = new BuyPackage();

        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST)
            {
                BuyNewPackage(rq, rs);
                return true;
            }
            return false;
        }

        public void BuyNewPackage(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                string username = ExtractUsername(rq.Headers["Authorization"]);

                if (username != null)
                {
                    if (!buyPackage.HasEnoughCoins(username))
                    {
                        rs.ResponseCode = 403; // Forbidden
                        rs.Content = "Not enough coins!";
                        return; // Early return to prevent further processing
                    }

                    int packageId = buyPackage.FetchFirstPackage();

                    if (packageId != null && buyPackage.AssignPackageToUser(username, packageId))
                    {
                        rs.ResponseCode = 201; // Created
                        rs.Content = "Package purchase successful!";
                    }
                    else { 
                    
                        rs.ResponseCode = 404; // Internal Server Error
                        rs.Content = "No Packages left!";
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

            rs.Headers.Add("Content-Type", "application/json");
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
