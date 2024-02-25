using MonsterTradingCardsGame.server;
using System.Text.Json;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace MonsterTradingCardsGame.Endpoints
{
    internal class DeckEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET)
            {
                GetUserDeck(rq, rs);
                return true;
            }else if (rq.Method == HttpMethod.PUT)
            {
                ConfigureUserDeck(rq, rs);
                return true;
            }

            return false;
        }

        public void GetUserDeck(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                //bool hasFormatQuery = rq.QueryParams.TryGetValue("format", out string formatValue) && .... wurde mit chatGPT gemacht 
                //hatte keine Ahnung wie ich den Teil nach deck rauslesen kann aus der URL da Path.Last() immer nur Deck zurückgibt
                //GPT hat mir erklärt dass QueryParams die URL nach ? ausliest und Path.Last() den letzten Teil der URL ausliest aber nach ? alles "ignoriert"
                string urlusername = rq.Path.Last();
                string username = ExtractUsername(rq.Headers["Authorization"]);
                bool hasFormatQuery = rq.QueryParams.TryGetValue("format", out string formatValue) &&
                         formatValue.Equals("plain", StringComparison.OrdinalIgnoreCase);

                if (username != null && urlusername == "deck" && hasFormatQuery)
                {
                    var user = new UserCardManager(username);
                    var deck = user.GetUserDeck(username);

                    if (deck.Count != 4)
                    {
                        rs.ResponseCode = 204; // Bad Request
                        rs.Content = "Your deck is currently not configured!";
                        return;
                    }
                    else if (deck != null)
                    {
                        rs.ResponseCode = 200; // OK
                        rs.Content = JsonSerializer.Serialize(deck);
                    }
                    else
                    {
                        rs.ResponseCode = 400; // Bad Request
                        rs.Content = "Failed to retrieve user deck!";
                    }
                }
            
                else if (username != null)
                {
                    var user = new UserCardManager(username);
                    var deck = user.GetUserDeckAsTableRows(username);

                    if (deck.Count <= 2) // Includes header and separator
                    {
                        rs.ResponseCode = 204; // No Content
                        rs.Content = "Your deck is currently not configured!";
                    }
                    else
                    {
                        rs.ResponseCode = 200; // OK
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

        public void ConfigureUserDeck(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                string username = ExtractUsername(rq.Headers["Authorization"]);

                if (username != null)
                {
                    var user = new UserCardManager(username);
                    var deck = JsonSerializer.Deserialize<List<string>>(rq.Content ?? "");

                    if (deck != null)
                    {
                        if (user.ConfigureUserDeck(username, deck))
                        {
                            rs.ResponseCode = 200; // OK
                            rs.Content = "User deck configuration successful!";
                        } 
                        else if(deck.Count != 4)
                        {
                            rs.ResponseCode = 400; // Bad Request
                            rs.Content = "Not Enough Cards Selected!";
                        }

                        else
                        {
                            rs.ResponseCode = 400; // Bad Request
                            rs.Content = "Failed to configure user deck!";
                        }
                    }
                    else
                    {
                        rs.ResponseCode = 400; // Bad Request
                        rs.Content = "Failed to parse request data!";
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
