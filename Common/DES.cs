using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Community.CsharpSqlite.SQLiteClient;

namespace Common
{
    public class DES : MarshalByRefObject, IDES
    {
        ArrayList usersList = new ArrayList();
        ArrayList diginotesList;
        Dictionary<Diginote, User> market;
        SqliteConnection m_dbConnection;

        public DES()
        {
            m_dbConnection = new SqliteConnection("Data Source=db/db.sqlite;Version=3;");
            m_dbConnection.Open();

            Console.WriteLine("Constructor called.");
            usersList = GetUsersArrayList();
            diginotesList = new ArrayList();
            market = new Dictionary<Diginote, User>();

           // user.AddBuyOrder(10);
           // usersList.Add(user);

            Diginote diginote = new Diginote();
            diginote.Quote = 0.98f;
          //  diginotesList.Add(diginote);
          //  market.Add(diginote, user);

            diginote = new Diginote();
            diginote.Quote = 1.00f;
            diginotesList.Add(diginote);
            market.Add(diginote, null);


        }

       
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ArrayList GetUsersArrayList()
        {
            ArrayList result = new ArrayList();
            string sql = "SELECT * FROM MarketUsers";
            try
            {
                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new User(reader["username"].ToString(), reader["nickname"].ToString(), reader["password"].ToString()));
                }

            }
            catch (Exception e)
            {
                
            }
            return result;
        }
        public string AddUser(string name, string nickname, string password)
        {

            Console.WriteLine("AddUser called.");

         
            if (usersList != null )
            {
                if(usersList.Cast<User>().Any(user => user.Nickname.Equals(nickname)))
                    return "Error adding user";
            }

            string sql = String.Format("INSERT INTO MarketUsers ('nickname', 'username', 'password') VALUES ('{0}', '{1}', '{2}')", nickname, name, password);
            try
            {
                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Error adding user to db";
            }

            User newUser = new User(name, nickname, password);
            usersList.Add(newUser);
            return "User added successfully!";
        }

        public string RemoveUser(string nickname, string password)
        {
            Console.WriteLine("RemoveUser called.");

            var userIndex = 0;
            foreach (User user in usersList)
            {
                if (user.Nickname.Equals(nickname))
                {
                    if (user.Password.Equals(password))
                    {
                        usersList.RemoveAt(userIndex);
                        return "User removed successfully!";
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

            foreach (User user in usersList)
            {
                if (user.Nickname.Equals(nickname))
                {
                    if (user.Password.Equals(password))
                    {
                        if (user.LoggedIn == false)
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

        public User GetUser(string nickname)
        {
            Console.WriteLine("GetUser called.");

            foreach (User user in usersList)
            {
                if (user.Nickname.Equals(nickname))
                {
                    return user;
                }
            }

            return null;
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

        public Dictionary<Diginote, User> GetMarket()
        {
            return market;
        }

        public float GetQuote()
        {
            float quote = 0.0f;
            int numDiginotes = 0;

            foreach (var entry in market)
            {
                quote += entry.Key.Quote;
                numDiginotes += 1;
            }

            quote = quote / numDiginotes;

            return quote;
        }

        public List<SaleOrder> GetSaleOrders(User user)
        {
            return user.SaleOrders;
        }
        
        public void EditSaleOrder(ref User user, int orderIndex, int quantity)
        {
            user.EditSaleOrder(orderIndex, quantity);
        }

        public List<BuyOrder> GetBuyOrders(User user)
        {
            return user.BuyOrders;
        }

        public void EditBuyOrder(ref User user, int orderIndex, int quantity)
        {
            user.EditBuyOrder(orderIndex, quantity);
        }
    }

    public interface IDES
    {
        string AddUser(string name, string nickname, string password);
        string RemoveUser(string nickname, string password);
        ArrayList GetUsersList();
        string Login(string nickname, string password);
        User GetUser(string nickname);
        string Logout(string nickname, string password);
        Dictionary<Diginote, User> GetMarket();
        float GetQuote();
        List<SaleOrder> GetSaleOrders(User user);
        void EditSaleOrder(ref User user, int orderIndex, int quantity);
        List<BuyOrder> GetBuyOrders(User user);
        void EditBuyOrder(ref User user, int orderIndex, int quantity);
    }
}