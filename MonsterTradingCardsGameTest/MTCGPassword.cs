using MonsterTradingCardsGame.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGameTest
{
    internal class MTCGPassword
    {
        //var expectedHash --> wurde mit onlnietool gehasht und dann hier eingefügt https://emn178.github.io/online-tools/sha256.html

        [Test]
        public void HashPassword_ReturnsCorrectHash_ForGivenPassword()
        {
            // Arrange
            var password = "TestPassword123";
            var expectedHash = "d519397a4e89a7a66d28a266ed00a679bdee93fddec9ebba7d01ff27c39c1a99"; 

            // Act
            var actualHash = PasswordHasher.HashPassword(password);

            // Assert
            Assert.AreEqual(expectedHash, actualHash, "The hashed password does not match the expected hash.");
        }

        [Test]
        public void VerifyPassword_ReturnsTrue_WhenPasswordMatchesHash()
        {
            // Arrange
            var password = "TestPassword123";
            var hashedPassword = PasswordHasher.HashPassword(password);

            // Act
            var result = PasswordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.IsTrue(result, "Password verification failed when it should have passed.");
        }

       
        [Test]
        public void VerifyPassword2_ReturnsTrue_WhenPasswordMatchesHash()
        {
            // Arrange
            var password = "TestPW2";
            var hashedPassword = PasswordHasher.HashPassword(password);

            // Act
            var result = PasswordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.IsTrue(result, "Password verification failed when it should have passed.");
        }
    }
}
