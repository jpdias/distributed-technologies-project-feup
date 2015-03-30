using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Community.CsharpSqlite.SQLiteClient;
using System.Timers;

namespace Common
{
    public class DES : MarshalByRefObject, IDES
    {
        List<User> usersList;
        List<Diginote> diginotesList;
        Dictionary<Diginote, User> market;
        Dictionary<SaleOrder, User> saleOrders;
        Dictionary<BuyOrder, User> buyOrders;
        SqliteConnection m_dbConnection;
        Timer timer;

        public DES()
        {
            m_dbConnection = new SqliteConnection("Data Source=db/db.sqlite;Version=3;");
            m_dbConnection.Open();

            Console.WriteLine("Constructor called.");
            usersList = GetUsersListFromDb();
            diginotesList = new List<Diginote>();
            market = new Dictionary<Diginote, User>();
            saleOrders = new Dictionary<SaleOrder, User>();
            buyOrders = new Dictionary<BuyOrder, User>();

            Diginote diginote = new Diginote();
            diginote.Quote = 0.98f;
            diginotesList.Add(diginote);

            market.Add(diginote, (User) usersList[0]);

            diginote = new Diginote();
            diginote.Quote = 1.00f;
            diginotesList.Add(diginote);

            market.Add(diginote, (User) usersList[0]);

            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(updateEvent);
            timer.Interval = 1000; // in miliseconds
            timer.Enabled = true;
            timer.Start();
        }

        private void updateEvent(object sender, ElapsedEventArgs e)
        {
            User seller;
            User buyer;

            foreach (var saleOrder in saleOrders)
            {
                seller = saleOrder.Value;

                if (saleOrder.Key.Processed == false)
                {
                    foreach (var buyOrder in buyOrders)
                    {
                        if (buyOrder.Key.Processed == false)
                        {
                            buyer = buyOrder.Value;

                            if (seller.Nickname != buyer.Nickname)
                            {
                                if (saleOrder.Key.Quantity == buyOrder.Key.Quantity)
                                {
                                    int numTransations = 0;

                                    while (numTransations != saleOrder.Key.Quantity)
                                    {
                                        List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                        market[sellerDiginotes[0]] = buyer;
                                        numTransations += 1;
                                    }

                                    saleOrder.Key.Processed = true;
                                    buyOrder.Key.Processed = true;

                                    Console.WriteLine(saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public List<User> GetUsersListFromDb()
        {
            List<User> result = new List<User>();
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

        public List<User> GetUsersList()
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

            foreach (var diginote in market)
            {
                quote += diginote.Key.Quote;
                numDiginotes += 1;
            }

            quote = quote / numDiginotes;

            return quote;
        }

        public List<SaleOrder> GetSaleOrders(ref User user)
        {
            List<SaleOrder> userSaleOrders = new List<SaleOrder>();

            foreach (var saleOrder in saleOrders)
            {
                if(saleOrder.Value.Nickname.Equals(user.Nickname))
                {
                    userSaleOrders.Add(saleOrder.Key);
                }
            }

            return userSaleOrders;
        }

        public string AddSaleOrder(ref User user, int quantity)
        {
            if (GetDiginotes(ref user).Count >= quantity)
            {
                saleOrders.Add(new SaleOrder(quantity), user);

                return "New sale order added successfully!";
            }
            else
            {
                return "Error: you do not have enough diginotes!";
            }
        }
        
        public void EditSaleOrder(ref User user, int orderIndex, int quantity)
        {
            int tmpOrderIndex = 0;

            foreach (var saleOrder in saleOrders)
            {
                if (saleOrder.Value.Nickname.Equals(user.Nickname))
                {
                    if (tmpOrderIndex == orderIndex)
                    {
                        saleOrder.Key.Quantity = quantity;

                        break;
                    }

                    tmpOrderIndex += 1;
                }
            }
        }

        public List<BuyOrder> GetBuyOrders(ref User user)
        {
            List<BuyOrder> userBuyOrders = new List<BuyOrder>();

            foreach (var buyOrder in buyOrders)
            {
                if (buyOrder.Value.Nickname.Equals(user.Nickname))
                {
                    userBuyOrders.Add(buyOrder.Key);
                }
            }

            return userBuyOrders;
        }

        public void AddBuyOrder(ref User user, int quantity)
        {
            buyOrders.Add(new BuyOrder(quantity), user);
        }

        public void EditBuyOrder(ref User user, int orderIndex, int quantity)
        {
            int tmpOrderIndex = 0;

            foreach (var buyOrder in buyOrders)
            {
                if (buyOrder.Value.Nickname.Equals(user.Nickname))
                {
                    if (tmpOrderIndex == orderIndex)
                    {
                        buyOrder.Key.Quantity = quantity;

                        break;
                    }

                    tmpOrderIndex += 1;
                }
            }
        }

        public List<Diginote> GetDiginotes(ref User user)
        {
            List<Diginote> diginotes = new List<Diginote>();

            foreach (var diginote in market)
            {
                if (diginote.Value.Nickname.Equals(user.Nickname))
                {
                    diginotes.Add(diginote.Key);
                }
            }

            return diginotes;
        }
    }

    public interface IDES
    {
        string AddUser(string name, string nickname, string password);
        string RemoveUser(string nickname, string password);
        List<User> GetUsersList();
        string Login(string nickname, string password);
        User GetUser(string nickname);
        string Logout(string nickname, string password);
        Dictionary<Diginote, User> GetMarket();
        float GetQuote();
        string AddSaleOrder(ref User user, int quantity);
        List<SaleOrder> GetSaleOrders(ref User user);
        void EditSaleOrder(ref User user, int orderIndex, int quantity);
        List<BuyOrder> GetBuyOrders(ref User user);
        void AddBuyOrder(ref User user, int quantity);
        void EditBuyOrder(ref User user, int orderIndex, int quantity);
        List<Diginote> GetDiginotes(ref User user);
    }
}