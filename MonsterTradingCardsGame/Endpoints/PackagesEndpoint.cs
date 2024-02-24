using MonsterTradingCardsGame.server;
using MonsterTradingCardsGame.cards;
using System.Text.Json;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MonsterTradingCardsGame.cards.MonsterTradingCardsGame.cards.CurlPack;
using MonsterTradingCardsGame.cards.MonsterTradingCardsGame.cards;

namespace MonsterTradingCardsGame.Endpoints
{
    internal class PackagesEndpoint : IHttpEndpoint
    {


        public bool HandleRequest(HttpRequest rq, HttpResponse rs) {
            if (rq.Method == HttpMethod.POST)
            {
                CreateNewPackage(rq, rs);
                return true;
            }
            return false;
        }

        public void CreateNewPackage(HttpRequest rq, HttpResponse rs)
        {
            string username = null;
          
            try
            {
                
                var AuthHeader = rq.Headers["Authorization"];
                var cardInputs = JsonSerializer.Deserialize<List<CardInput>>(rq.Content ?? "");

                if (AuthHeader != null)
                {
                    username = AuthHeader.Replace("Bearer ", "").Replace("-mtcgToken", "");
                }
                // Authentication and Authorization logic here...

                if (username == "admin")
                {
                    // Create an instance of CurlPack
                    var curlPack = new CurlPack();

                    // Process the request
                    var cards = curlPack.CreatePackage(cardInputs);

                    // Assuming the package creation is successful if cards are returned
                    if (cards.Count > 0)
                    {
                        rs.ResponseCode = 201; // Created
                        rs.Content = "Package creation successful!";
                    }
                    else
                    {
                        rs.ResponseCode = 400; // Bad Request
                        rs.Content = "Failed to create package.";
                    }
                }
                else
                {
                    rs.ResponseCode = 403; // Forbidden
                    rs.Content = "You are not authorized to create packages.";
                }
            }
            catch (Exception ex)
            {
                rs.ResponseCode = 400; // Bad Request
                rs.Content = "Failed to process package creation: " + ex.Message;
            }

            rs.Headers.Add("Content-Type", "application/json");
        }

       
    }
}
