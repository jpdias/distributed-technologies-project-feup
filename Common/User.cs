using System;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class User
    {
        private static int Serial = 0; 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public bool LoggedIn { get; set; }

        public User(string name, string nickname, string password)
        {
            Serial += 1;
            this.Id = Serial;
            this.Name = name;
            this.Nickname = nickname;
            this.Password = password;
            this.LoggedIn = false;
        }

        public User(int id, string name, string nickname, string password)
        {
            this.Id = id;
            this.Name = name;
            this.Nickname = nickname;
            this.Password = password;
            this.LoggedIn = false;
        }

        public override string ToString()
        {
            return "User's" + " " + "nickname: " + Nickname;
        }
    }
}

