using System;
using System.Collections;
using System.Threading;

public class ListSingleton : MarshalByRefObject, IListSingleton {
  ArrayList usersList;

  public ListSingleton()
  {
      Console.WriteLine("Constructor called.");
      usersList = new ArrayList();
  }

  public override object InitializeLifetimeService()
  {
      return null;
  }

  public bool AddUser(string name, string nickname, string password)
  {
      Console.WriteLine("AddUser called.");

      foreach(User user in usersList)
      {
          if(user.Nickname.Equals(nickname))
          {
              return false;
          }
      }

      User newUser = new User(name, nickname, password);
      usersList.Add(newUser);
      return true;
  }

  public bool RemoveUser(string nickname, string password)
  {
      Console.WriteLine("RemoveUser called.");

      var userIndex = 0;
      foreach(User user in usersList)
      {
          if(user.Nickname.Equals(nickname) && user.Password.Equals(password))
          {
              usersList.RemoveAt(userIndex);
              return true;
          }

          userIndex += 1;
      }

      return false;
  }

  public ArrayList GetUsersList()
  {
      Console.WriteLine("GetUsersList called.");
      return usersList;
  }

  public bool Login(string nickname, string password)
  {
      Console.WriteLine("Login called.");

      foreach(User user in usersList)
      {
          if(user.Nickname.Equals(nickname) && user.Password.Equals(password))
          {
              return true;
          }
      }

      return false;
  }
}