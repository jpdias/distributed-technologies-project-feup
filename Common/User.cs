using System;

namespace Common
{
    [Serializable]
    public class User
    {
        private static int Serial;

        public User(string name, string nickname, string password)
        {
            Serial += 1;
            Id = Serial;
            Name = name;
            Nickname = nickname;
            Password = password;
            LoggedIn = false;
        }

        public User(int id, string name, string nickname, string password)
        {
            Id = id;
            Name = name;
            Nickname = nickname;
            Password = password;
            LoggedIn = false;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public bool LoggedIn { get; set; }

        public override string ToString()
        {
            return "User's" + " " + "nickname: " + Nickname;
        }
    }
}