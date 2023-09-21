using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;



namespace MonsterTradingCardsGame
{
    internal class User
    {
        private static int _userCount = 0;

        protected int _id = -1;
        protected string _username;
        protected string _password;
        protected int _coins = 20;

        protected Vector2 _stack = new Vector2();
  
      //  protected <type>[] deck = {"cards1-4"}; --> container klasse ist besser

        
        public User(string name, string password)
        {
            //if (name == usernameDB) { console.writeline username already exists) else{
            _username = name;
            _password = password;
            _id = ++_userCount;
        }

        //username get

    }


}
