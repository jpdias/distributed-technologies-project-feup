using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiginoteExchangeSystem
{
    [Serializable]
    class User
    {
        public User( string nickname, string name, string password)
        {
            Password = password;
            Nickname = nickname;
            Name = name;
        }

        public String Name { get; set; }
        public String Nickname { get; set; }
        public String Password { get; set; }
    }
   
}
