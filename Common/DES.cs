using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Community.CsharpSqlite.SQLiteClient;
using System.Timers;
using System.IO;

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
            usersList = new List<User>();
            diginotesList = new List<Diginote>();
            market = new Dictionary<Diginote, User>();
            saleOrders = new Dictionary<SaleOrder, User>();
            buyOrders = new Dictionary<BuyOrder, User>();

            AddUser("jc", "jc", "jc");
            AddUser("jp", "jp", "jp");

            usersList = GetUsersListFromDb();
            diginotesList = GetDiginotesListFromDb();
            market = GetMarketFromDb();
            market = GetMarket();

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

                                    /*
                                    // Write to log text file
                                    StreamWriter file = new StreamWriter(@"c:\log.txt", true);
                                    file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                    file.Close();
                                    */

                                    // Write to db log file
                                    string sql = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                    try
                                    {
                                        SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }

                                    break;
                                }
                                else
                                {
                                    if (saleOrder.Key.Quantity < buyOrder.Key.Quantity)
                                    {
                                        int numTransations = 0;

                                        while (numTransations != saleOrder.Key.Quantity)
                                        {
                                            List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                            market[sellerDiginotes[0]] = buyer;
                                            numTransations += 1;
                                        }

                                        buyOrder.Key.Quantity -= numTransations;
                                        saleOrder.Key.Processed = true;
                                        buyOrder.Key.Processed = false;

                                        /*
                                        // Write to log text file
                                        StreamWriter file = new StreamWriter(@"c:\log.txt", true);
                                        file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                        file.Close();
                                        */

                                        // Write to db log file
                                        string sql = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                        try
                                        {
                                            SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                            command.ExecuteNonQuery();
                                        }
                                        catch (Exception exception)
                                        {
                                            Console.WriteLine(exception);
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        if (saleOrder.Key.Quantity > buyOrder.Key.Quantity)
                                        {
                                            int numTransations = 0;

                                            while (numTransations != buyOrder.Key.Quantity)
                                            {
                                                List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                                market[sellerDiginotes[0]] = buyer;
                                                numTransations += 1;
                                            }

                                            saleOrder.Key.Quantity -= numTransations;
                                            saleOrder.Key.Processed = false;
                                            buyOrder.Key.Processed = true;

                                            /*
                                            // Write to log text file
                                            StreamWriter file = new StreamWriter(@"c:\log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();
                                            */

                                            // Write to db log file
                                            string sql = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.WriteLine(exception);
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            updateQuote();
        }

        public void updateQuote()
        {
            foreach (var diginote in market)
            {
                diginote.Key.Quote = 1.0f;
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

        public List<Diginote> GetDiginotesListFromDb()
        {
            List<Diginote> result = new List<Diginote>();
            string sql = "SELECT * FROM MarketDiginotes";
            try
            {
                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Diginote());
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public Dictionary<Diginote, User> GetMarketFromDb()
        {
            Dictionary<Diginote, User> result = new Dictionary<Diginote, User>();
            string sql = "SELECT * FROM Market";
            try
            {
                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int diginoteId = reader.GetInt32(1);
                    int userId = reader.GetInt32(2);
                    result.Add(diginotesList[diginoteId - 1], usersList[userId - 1]);
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

            User newUser = new User(name, nickname, password);
            usersList.Add(newUser);

            string sql = String.Format("INSERT INTO MarketUsers ('nickname', 'username', 'password') VALUES ('{0}', '{1}', '{2}')", nickname, name, password);
            try
            {
                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Error adding user to db!";
            }

            // Give 10 Diginotes to this user
            for (int i = 0; i < 10; i++)
            {
                AddDiginote(usersList.Count);
            }

            return "User added successfully!";
        }

        public string AddDiginote(int userId)
        {
            Console.WriteLine("AddDiginote called.");

            Diginote diginote = new Diginote(diginotesList.Count + 1);
            diginotesList.Add(diginote);

            string sql = String.Format("INSERT INTO MarketDiginotes ('id') VALUES ('{0}')", diginote.Id);
            try
            {
                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Error adding diginote to db!";
            }

            market.Add(diginotesList[diginote.Id - 1], usersList[userId - 1]);

            sql = String.Format("INSERT INTO Market ('diginoteId', 'userId') VALUES ('{0}', '{1}')", diginote.Id, userId);
            try
            {
                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Error adding diginote to db!";
            }

            return "Diginote added successfully!";
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
            float quoteSum = 0.0f;
            int numDiginotes = 0;

            foreach (var diginote in market)
            {
                quoteSum += diginote.Key.Quote;
                numDiginotes += 1;
            }

            quote = quoteSum / numDiginotes;

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
        
        public string EditSaleOrder(ref User user, int orderIndex, int quantity)
        {
            int tmpOrderIndex = 0;

            foreach (var saleOrder in saleOrders)
            {
                if (saleOrder.Value.Nickname.Equals(user.Nickname))
                {
                    if (tmpOrderIndex == orderIndex)
                    {
                        if(saleOrder.Key.Processed == false)
                        {
                            saleOrder.Key.Quantity = quantity;

                            return "Sale order edited successfully!";
                        }
                        else
                        {
                            return "Error: Sale order already processed!";
                        }
                    }

                    tmpOrderIndex += 1;
                }
            }

            return "Sale order edited successfully!";
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

        public string AddBuyOrder(ref User user, int quantity)
        {
            buyOrders.Add(new BuyOrder(quantity), user);

            return "New buy order added successfully!";
        }

        public string EditBuyOrder(ref User user, int orderIndex, int quantity)
        {
            int tmpOrderIndex = 0;

            foreach (var buyOrder in buyOrders)
            {
                if (buyOrder.Value.Nickname.Equals(user.Nickname))
                {
                    if (tmpOrderIndex == orderIndex)
                    {
                        if (buyOrder.Key.Processed == false)
                        {
                            buyOrder.Key.Quantity = quantity;

                            return "Buy order edited successfully!";
                        }
                        else
                        {
                            return "Error: Buy order already processed!";
                        }
                    }

                    tmpOrderIndex += 1;
                }
            }

            return "Buy order edited successfully!";
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
        string EditSaleOrder(ref User user, int orderIndex, int quantity);
        List<BuyOrder> GetBuyOrders(ref User user);
        string AddBuyOrder(ref User user, int quantity);
        string EditBuyOrder(ref User user, int orderIndex, int quantity);
        List<Diginote> GetDiginotes(ref User user);
    }
}