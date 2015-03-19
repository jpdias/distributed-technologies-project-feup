using System;
using System.Collections;

[Serializable]
public class User
{
    public string Name { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public bool LoggedIn { get; set; }

    public User(string name, string nickname, string password)
    {
        Name = name;
        Nickname = nickname;
        Password = password;
        LoggedIn = false;
    }
}

