using MonsterTradingCardsGame.server;
using MonsterTradingCardsGame.user;
using System.Text.Json;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGame.Endpoints
{
    internal class SessionsEndpoint : IHttpEndpoint
    {
        private UserManager userManager; // UserManager instance

        public SessionsEndpoint()
        {
            userManager = new UserManager();
        }
        public bool HandleRequest(HttpRequest rq, HttpResponse rs) { return false; }
    }
}
