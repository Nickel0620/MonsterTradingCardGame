/*using Microsoft.Analytics.Interfaces;
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
*/

-- Users Table
CREATE TABLE Users (
    UserID SERIAL PRIMARY KEY,
    Bio TEXT,
    Image TEXT,
    Elo INT,
    Coins INT,
    Username VARCHAR(255) UNIQUE NOT NULL,
    Name VARCHAR(255),
    Password VARCHAR(255) NOT NULL, 
    GamesPlayed INT,
    GamesWon INT,
    GamesLost INT,
    IsAdmin BOOLEAN DEFAULT false
);

-- Cards Table
CREATE TABLE Cards (
    CardID SERIAL PRIMARY KEY,
    Type VARCHAR(255),
    CreatureName VARCHAR(255),
    Element VARCHAR(255),
    CurlID VARCHAR(255),
    Damage DOUBLE PRECISION,
    CardName VARCHAR(255)
);

-- Packages Table
CREATE TABLE Packages (
    PackageID SERIAL PRIMARY KEY,
    CardID1 INT REFERENCES Cards(CardID),
    CardID2 INT REFERENCES Cards(CardID),
    CardID3 INT REFERENCES Cards(CardID),
    CardID4 INT REFERENCES Cards(CardID),
    CardID5 INT REFERENCES Cards(CardID)
);

-- UserCards Table
CREATE TABLE UserCards (
    UserCardID SERIAL PRIMARY KEY,
    UserID INT REFERENCES Users(UserID),
    CardID INT REFERENCES Cards(CardID),
    InDeck BOOLEAN DEFAULT false
);

--UserDeck Table
CREATE TABLE UserDeck (
    UserDeckID SERIAL PRIMARY KEY,
    UserID INT REFERENCES Users(UserID),
    Card1 INT REFERENCES UserCards(UserCardID),
    Card2 INT REFERENCES UserCards(UserCardID),
    Card3 INT REFERENCES UserCards(UserCardID),
    Card4 INT REFERENCES UserCards(UserCardID),
    UNIQUE(UserID)
);


