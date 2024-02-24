using MonsterTradingCardsGame.server;
using System.Text.Json;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using MonsterTradingCardsGame.user;
using MonsterTradingCardsGame.cards;
using MonsterTradingCardsGame.logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Endpoints
{
    internal class TradingsEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET)
            {
                GetTradingDeal(rq, rs);
                return true;
            }
            else if (rq.Method == HttpMethod.PUT)
            {
                CreateTradeDeal(rq, rs);
                return true;
            }

            return false;
        }

        public void GetTradingDeal(HttpRequest rq, HttpResponse rs)
        {
            
        }

        public void CreateTradeDeal(HttpRequest rq, HttpResponse rs)
        {
            
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
