using Newtonsoft.Json.Linq;
using System;
using Moq;
using System.Collections.Generic;
using NUnit.Framework;
using MonsterTradingCardsGame;
using MonsterTradingCardsGame.logic;
using MonsterTradingCardsGame.cards;
using MonsterTradingCardsGame.user;

namespace MonsterTradingCardsGameTest
{
    public class MTCGTests
    {
        private Mock<User> mockPlayer1;
        private Mock<User> mockPlayer2;
        private Battle game;

        [SetUp]
        public void Setup()
        {
            mockPlayer1 = new Mock<User>();
            mockPlayer2 = new Mock<User>();
            game = new Battle(mockPlayer1.Object, mockPlayer2.Object);
        }

        [Test]
        public void CalculateDamage_WaterAttacksFire_DamageDoubled()
        {
            // Arrange
            var attacker = new Card(1, "Monster", "Goblin", "Water", "", 10, "");
            var defender = new Card(2, "Monster", "Dragon", "Fire", "", 5,""); // Assuming the defender is a Dragon


            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(20, damage); // Expecting the damage to be doubled
        }

        [Test]
        public void CalculateDamage_WaterAttacksNormal_DamageHalved()
        {
            // Arrange
            var attacker = new Card(1, "Monster", "Wizard", "Water", "", 10, ""); // Assuming the attacker is a Wizard
            var defender = new Card(2, "Spell", "", "Normal", "", 5, ""); // Assuming the defender is a spell card


            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(5, damage); // Expecting the damage to be halved
        }

        [Test]
        public void CalculateDamage_FireAttacksNormal_DamageDoubled()
        {
            // Arrange
            var attacker = new Card(1, "Monster", "Orc", "Fire", "", 10,""); // Assuming the attacker is an Orc
            var defender = new Card(2, "Spell", "", "Normal", "", 5,""); // Assuming the defender is a spell card


            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(20, damage); // Expecting the damage to be doubled
        }

        [Test]
        public void CalculateDamage_FireAttacksWater_DamageHalved()
        {
            // Arrange
            var attacker = new Card(1, "Monster", "Knight", "Fire", "", 10, ""); // Assuming the attacker is a Knight
            var defender = new Card(2, "Monster", "Kraken", "Water", "", 5, ""); // Assuming the defender is a Kraken


            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(5, damage); // Expecting the damage to be halved
        }

        [Test]
        public void CalculateDamage_NormalAttacksWater_DamageDoubled()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Normal", "", 10, ""); // Assuming the attacker is a spell card
            var defender = new Card(2, "Monster", "Elves", "Water", "", 5, ""); // Assuming the defender is Elves


            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(20, damage); // Expecting the damage to be doubled
        }

        [Test]
        public void CalculateDamage_NormalAttacksFire_DamageHalved()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Normal", "", 10, ""); // Assuming the attacker is a spell card
            var defender = new Card(2, "Monster", "Dragon", "Fire", "", 5, ""); // Assuming the defender is a Dragon


            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(5, damage); // Expecting the damage to be halved
        }

        [Test]
        public void CalculateDamage_GoblinVsDragon_DamageIsZero()
        {
            // Arrange
            var attacker = new Card(1, "Monster", "Goblin", "Normal", "", 10, ""); // Assuming Goblin is a Normal element Monster
            var defender = new Card(2, "Monster", "Dragon", "Normal", "", 5, ""); // Assuming Dragon is also a Normal element Monster

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // Goblins are too afraid to attack Dragons
        }

        [Test]
        public void CalculateDamage_WizardVsOrk_DamageIsZero()
        {
            // Arrange
            var attacker = new Card(1, "Monster", "Wizard", "Normal", "", 10, ""); // Assuming Wizard is a Normal element Monster
            var defender = new Card(2, "Monster", "Orc", "Normal", "", 5, ""); // Assuming Orc is a Normal element Monster

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // Wizard controls Orks, so no damage
        }

        [Test]
        public void CalculateDamage_WaterSpellVsKnight_DamageIsMax()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Water", "", 10, ""); // The attacker is a Water element Spell
            var defender = new Card(2, "Monster", "Knight", "Normal", "", 5, ""); // Assuming Knight is a Normal element Monster

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(int.MaxValue, damage); // WaterSpell drowns Knight instantly
        }

        [Test]
        public void CalculateDamage_SpellVsKraken_DamageIsZero()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Normal", "", 10, ""); // Assuming the spell is a Normal element
            var defender = new Card(2, "Monster", "Kraken", "Water", "", 5, ""); // Assuming Kraken is a Water element Monster

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // Kraken is immune to spells
        }

        [Test]
        public void CalculateDamage_FireElfVsDragon_DamageIsZero()
        {
            // Arrange
            var attacker = new Card(1, "Monster", "Elves", "Fire", "", 10, ""); // Elves are a Fire element Monster
            var defender = new Card(2,"Monster", "Dragon", "Normal", "", 5, ""); // Assuming Dragon is a Normal element Monster

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // FireElves evade Dragon attacks
        }

        [Test]
        public void CalculateDamage_WaterGoblinVsFireTroll_GoblinLoses()
        {
            // Arrange
            var attacker = new Card(1, "Monster", "Goblin", "Water", "", 10, ""); // Assuming Goblin is a Water element Monster with Id 1
            var defender = new Card(2, "Monster", "Troll", "Fire", "", 5, ""); // Assuming Troll is a Fire element Monster with Id 2

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // WaterGoblin loses, so damage is 0
        }

        [Test]
        public void CalculateDamage_FireTrollVsWaterGoblin_TrollWins()
        {
            // Arrange
            var attacker = new Card(1, "Monster", "Troll", "Fire", "", 10, ""); // Assuming Troll is a Fire element Monster with Id 3
            var defender = new Card(2,"Monster", "Goblin", "Water", "", 5, ""); // Assuming Goblin is a Water element Monster with Id 4

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(defender.Damage + 1, damage); // FireTroll wins with damage higher than the Goblin's
        }

        [Test]
        public void CalculateDamage_FireSpellVsWaterSpell_FireSpellLoses()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Fire", "", 10, ""); // Fire Spell with Id 1
            var defender = new Card(2, "Spell", "", "Water", "", 5, ""); // Water Spell with Id 2

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // FireSpell loses to WaterSpell, damage is 0
        }

        [Test]
        public void CalculateDamage_WaterSpellVsFireSpell_WaterSpellWins()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Water", "", 10, ""); // Water Spell with Id 3
            var defender = new Card(2, "Spell", "", "Fire", "", 5, ""); // Fire Spell with Id 4

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(20, damage); // WaterSpell wins, damage is doubled
        }

        [Test]
        public void CalculateDamage_SpellFight_Draw()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Fire", "", 10, ""); // Fire Spell with Id 5
            var defender = new Card(2, "Spell", "", "Fire", "", 20, ""); // Another Fire Spell with Id 6

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // Draw, no damage to either
            Assert.AreEqual(0, defender.Damage); // Defender's damage also set to 0
        }

        [Test]
        public void CalculateDamage_FireSpellVsWaterMonster_SpellLoses()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Fire", "", 10, ""); // Fire Spell with Id 1
            var defender = new Card(2, "Monster", "Goblin", "Water", "", 5, ""); // Water Monster with Id 2, assuming 'UnknownMonster' as the name

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // FireSpell loses to WaterMonster
        }

        [Test]
        public void CalculateDamage_WaterSpellVsWaterMonster_Draw()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Water", "", 10, ""); // Water Spell with Id 3
            var defender = new Card(2, "Monster", "Dragon", "Water", "", 10, ""); // Water Monster with Id 4, assuming 'UnknownMonster' as the name

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // Draw, no damage
            Assert.AreEqual(0, defender.Damage); // Defender's damage also set to 0
        }

        [Test]
        public void CalculateDamage_RegularSpellVsWaterMonster_SpellWins()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Normal", "", 10, ""); // Regular (Normal) Spell with Id 5
            var defender = new Card(2, "Monster", "Dragon", "Water", "", 5, ""); // Water Monster with Id 6, assuming 'UnknownMonster' as the name

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(20, damage); // RegularSpell wins against WaterMonster
        }

        [Test]
        public void CalculateDamage_RegularSpellVsKnight_Draw()
        {
            // Arrange
            var attacker = new Card(1, "Spell", "", "Normal", "", 10, ""); // Regular (Normal) Spell with Id 7
            var defender = new Card(2, "Monster", "Knight", "Normal", "", 10, ""); // Knight with Id 8, assuming an 'UnknownElement' for the Knight

            // Act
            double damage = game.CalculateDamage(attacker, defender);

            // Assert
            Assert.AreEqual(0, damage); // Draw, no damage to either
            Assert.AreEqual(0, defender.Damage); // Defender's damage also set to 0
        }

    }
}