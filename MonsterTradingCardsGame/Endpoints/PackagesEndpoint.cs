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

namespace MonsterTradingCardsGame.Endpoints
{
    internal class PackagesEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs) {
            if (rq.Method == HttpMethod.POST)
            {
                CreatePackage(rq, rs);
                return true;
            }
            return false;
        }

        public void CreatePackage(HttpRequest rq, HttpResponse rs)
        {
            string username = null;
          
            try
            {
                var packageCreatinRequest = JsonSerializer.Deserialize<List<PackageItem>>(rq.Content ?? "");
                var AuthHeader = rq.Headers["Authorization"];

                if (AuthHeader != null)
                {
                    username = AuthHeader.Replace("Bearer", "").Replace("-mtcgToken", "");
                }
                // Authentication and Authorization logic here
                // Ensure that the user is authenticated and authorized to create packages
                // ...
                if (username == "admin")
                {
                    bool packageCreatedSuccessfully = true;
                    foreach (var item in packageCreatinRequest)
                    {
                        // Logic to add each item in the package to the database
                        //packagemanager:
                    }

                    if (packageCreatedSuccessfully)
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

        public class PackageItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public double Damage { get; set; }
        }
    }
}
