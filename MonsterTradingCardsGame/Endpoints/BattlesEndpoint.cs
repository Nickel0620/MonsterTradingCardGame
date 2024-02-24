using MonsterTradingCardsGame.server;
using System.Text.Json;
using HttpMethod = MonsterTradingCardsGame.server.HttpMethod;
using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using MonsterTradingCardsGame.logic;
using Npgsql;

namespace MonsterTradingCardsGame.Endpoints
{
    internal class BattlesEndpoint : IHttpEndpoint
    {
        private static ConcurrentQueue<string> playerQueue = new ConcurrentQueue<string>();
        private string connectionString = "Host=localhost:5432;Username=postgres;Password=postgres;Database=mtcg";

        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST)
            {
                string username = ExtractUsername(rq.Headers["Authorization"]);
                if (username != null)
                {
                    playerQueue.Enqueue(username);
                    CheckAndStartBattle();
                }
                return true;
            }
            return false;
        }


        private void CheckAndStartBattle()
        {
            if (playerQueue.Count >= 2)
            {
                // Attempt to start a battle
                if (playerQueue.TryDequeue(out string player1) && playerQueue.TryDequeue(out string player2))
                {
                    Task.Run(() => CreateNewBattle(player1, player2));
                }
            }
        }

        private void CreateNewBattle(string player1, string player2)
        {
            // Create and start a new battle
            Battle battle = new Battle(player1, player2);
            battle.PlayGame();
            // Handle battle outcome
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
