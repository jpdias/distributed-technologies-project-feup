using System;
using System.Collections;
using System.Threading;

public class DES : MarshalByRefObject, IDES {
  ArrayList usersList;

  public DES()
  {
      Console.WriteLine("Constructor called.");
      usersList = new ArrayList();
  }

  public override object InitializeLifetimeService()
  {
      return null;
  }

  public string AddUser(string name, string nickname, string password)
  {
      Console.WriteLine("AddUser called.");

      foreach(User user in usersList)
      {
          if(user.Nickname.Equals(nickname))
          {
              return "Error adding user: Nickname already exists!";
          }
      }

      User newUser = new User(name, nickname, password);
      usersList.Add(newUser);
      return "User added successfully!";
  }

  public string RemoveUser(string nickname, string password)
  {
      Console.WriteLine("RemoveUser called.");

      var userIndex = 0;
      foreach(User user in usersList)
      {
          if(user.Nickname.Equals(nickname))
          {
              if(user.Password.Equals(password))
              {
                  usersList.RemoveAt(userIndex);
                  return "User removed successfully!";
              }
              else
              {
                  return "Error removing user: Wrong password!";
              }
          }

          userIndex += 1;
      }

      return "Error removing user: Nickname not found!";
  }

  public ArrayList GetUsersList()
  {
      Console.WriteLine("GetUsersList called.");
      return usersList;
  }

  public string Login(string nickname, string password)
  {
      Console.WriteLine("Login called.");

      foreach(User user in usersList)
      {
          if(user.Nickname.Equals(nickname))
          {
              if(user.Password.Equals(password))
              {
                  if(user.LoggedIn == false)
                  {
                      user.LoggedIn = true;
                      return "Login successful!";
                  }
                  else
                  {
                      return "Login error: User is already logged in!";
                  }
              }
              else
              {
                  return "Login error: Wrong password!";
              }
          }
      }

      return "Login error: Nickname not found!";
  }

  public string Logout(string nickname, string password)
  {
      Console.WriteLine("Logout called.");

      foreach (User user in usersList)
      {
          if (user.Nickname.Equals(nickname))
          {
              if (user.Password.Equals(password))
              {
                  if (user.LoggedIn)
                  {
                      user.LoggedIn = false;
                      return "Logout successful!";
                  }
                  else
                  {
                      return "Logout error: User is not logged in!";
                  }
              }
              else
              {
                  return "Logout error: Wrong password!";
              }
          }
      }

      return "Logout error: Nickname not found!";
  }
}
public interface IDES
{
    string AddUser(string name, string nickname, string password);
    string RemoveUser(string nickname, string password);
    ArrayList GetUsersList();
    string Login(string nickname, string password);
    string Logout(string nickname, string password);
}