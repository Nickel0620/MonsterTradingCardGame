﻿using MonsterTradingCardsGame;
using MonsterTradingCardsGame.server;
using MonsterTradingCardsGame.Endpoints;
using System.Net;

Console.WriteLine("Our first simple HTTP-Server: http://localhost:10001/");

// ===== I. Start the HTTP-Server =====
HttpServer httpServer = new HttpServer(IPAddress.Any, 10001);
httpServer.RegisterEndpoint("users", new UsersEndpoint());
httpServer.RegisterEndpoint("tradings", new TradingsEndpoint());
httpServer.RegisterEndpoint("scoreboard", new ScoreboardEndpoint());
httpServer.RegisterEndpoint("cards", new CardsEndpoint());
httpServer.RegisterEndpoint("transactions", new TransactionsEndpoint());
httpServer.RegisterEndpoint("packages", new PackagesEndpoint());
httpServer.RegisterEndpoint("deck", new DeckEndpoint());
httpServer.RegisterEndpoint("battles", new BattlesEndpoint());


httpServer.Run();