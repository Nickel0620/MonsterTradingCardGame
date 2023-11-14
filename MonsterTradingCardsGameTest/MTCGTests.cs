using Newtonsoft.Json.Linq;

namespace MonsterTradingCardsGameTest
{
    public class MTCGTests
    {

        //Damit bei Setup und später bei [Test] Line nicht "not declared in scope" kommt 
        //private Line line;
        [SetUp]
        public void Setup()
        {
            //Setup wird immer aufgerufen vor [Test], wenn man code mehrmals verwenden muss/will
            //immer in Setup reingeben
            //line = new Line(0,0,1,1)
        }

        [Test]
        public void TestNameForClassTested()
        {
            //AAA Pattern
            //Arrange, (alles was man braucht, Parameter usw werden hier vorbereitet)
            //aufpassen dass Klassen nicht internal sind --> Funktioniert nur in dem eigenen Project
            //<typ> expectedResult = "was auch immer" 

            //Act
            //method die getestet werden soll

            //Assert

        }

        [Test]
        
        public void TestClassTested() {
        
            //next Test
        
        }
        
    }
}