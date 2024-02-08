using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonsterTradingCardsGame.DB
{
    class DB_Create
    {
    }
}

CREATE TABLE Users (
    UserId SERIAL PRIMARY KEY,
    Bio TEXT,
    Image TEXT,
    Elo INT,
    Coins INT,
    Username TEXT UNIQUE,
    Name TEXT,
    Password TEXT,
    GamesPlayed INT,
    IsAdmin BOOLEAN DEFAULT false
);

CREATE TABLE Cards (
    CardId SERIAL PRIMARY KEY,
    Type TEXT,
    CreatureName TEXT,
    Element TEXT,
    Damage INT
);

CREATE TABLE Packages (
    PackageId SERIAL PRIMARY KEY
    -- Cards are linked through PackageCards table
);

CREATE TABLE UserCards (
    UserCardId SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(UserId),
    CardId INT REFERENCES Cards(CardId),
    AcquiredFromPackageId INT REFERENCES Packages(PackageId)
);

CREATE TABLE UserDeck (
    UserDeckId SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(UserId),
    CardId INT REFERENCES Cards(CardId)
);

CREATE TABLE PackageCards (
    PackageCardId SERIAL PRIMARY KEY,
    PackageId INT REFERENCES Packages(PackageId),
    CardId INT REFERENCES Cards(CardId)
);