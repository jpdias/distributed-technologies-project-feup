using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Community.CsharpSqlite.SQLiteClient;
using System.Timers;
using System.IO;
using System.Threading;

namespace Common
{
    public class DES : MarshalByRefObject, IDES
    {
        public event AlterDelegate alterEvent;
        
        List<User> usersList;
        List<Diginote> diginotesList;
        Dictionary<Diginote, User> market;
        Dictionary<SaleOrder, User> saleOrders;
        Dictionary<BuyOrder, User> buyOrders;
        SqliteConnection m_dbConnection;
        // Timer timer;

        public enum Operation { Add, Change };

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
                    result.Add(new User(reader.GetInt32(0), reader["username"].ToString(), reader["nickname"].ToString(), reader["password"].ToString()));
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
                    result.Add(new Diginote(reader.GetInt32(0)));
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

            if ((usersList.Any(p => p.Nickname == nickname)) == false)
            {
                User newUser = new User(name, nickname, password);
                usersList.Add(newUser);

                string sql = String.Format("INSERT INTO MarketUsers ('id', 'nickname', 'username', 'password') VALUES ('{0}', '{1}', '{2}', '{3}')", newUser.Id, nickname, name, password);
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

                // Give 10 Diginotes to the new user
                for (int i = 0; i < 10; i++)
                {
                    AddDiginote(usersList.Count);
                }

                return "User added successfully!";
            }
            else
            {
                return "Username already exists!";
            }
        }

        public string AddDiginote(int userId)
        {
            Console.WriteLine("AddDiginote called.");

            Diginote newDiginote = new Diginote();
            diginotesList.Add(newDiginote);

            string sql = String.Format("INSERT INTO MarketDiginotes ('id') VALUES ('{0}')", newDiginote.Id);
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

            market.Add(diginotesList[newDiginote.Id - 1], usersList[userId - 1]);

            sql = String.Format("INSERT INTO Market ('diginoteId', 'userId') VALUES ('{0}', '{1}')", newDiginote.Id, userId);
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
            return market.First().Key.Quote;
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
                SaleOrder saleOrder = new SaleOrder(quantity, GetQuote());

                saleOrders.Add(saleOrder, user);

                User seller;
                User buyer;
                
                seller = user;

                if (saleOrder.Processed == false)
                {
                    foreach (var buyOrder in buyOrders)
                    {
                        if (buyOrder.Key.Processed == false)
                        {
                            buyer = buyOrder.Value;

                            if (seller.Nickname != buyer.Nickname)
                            {
                                if (saleOrder.Quantity == buyOrder.Key.Quantity)
                                {
                                    int numTransations = 0;

                                    while (numTransations != saleOrder.Quantity)
                                    {
                                        List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                        market[sellerDiginotes[0]] = buyer;

                                        string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                        try
                                        {
                                            SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                            command.ExecuteNonQuery();
                                        }
                                        catch (Exception exception)
                                        {
                                            Console.WriteLine(exception);
                                        }

                                        numTransations += 1;
                                    }

                                    saleOrder.Processed = true;
                                    buyOrder.Key.Processed = true;

                                    // Write to log text file
                                    StreamWriter file = new StreamWriter(@"log.txt", true);
                                    file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                    file.Close();

                                    // Write to db log file
                                    string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), saleOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                    try
                                    {
                                        SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
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
                                    if (saleOrder.Quantity < buyOrder.Key.Quantity)
                                    {
                                        int numTransations = 0;

                                        while (numTransations != saleOrder.Quantity)
                                        {
                                            List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                            market[sellerDiginotes[0]] = buyer;

                                            string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                            try
                                            {
                                                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.WriteLine(exception);
                                            }

                                            numTransations += 1;
                                        }

                                        buyOrder.Key.Quantity -= numTransations;
                                        saleOrder.Processed = true;
                                        buyOrder.Key.Processed = false;

                                        // Write to log text file
                                        StreamWriter file = new StreamWriter(@"log.txt", true);
                                        file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                        file.Close();

                                        // Write to db log file
                                        string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), saleOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                        try
                                        {
                                            SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
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
                                        if (saleOrder.Quantity > buyOrder.Key.Quantity)
                                        {
                                            int numTransations = 0;

                                            while (numTransations != buyOrder.Key.Quantity)
                                            {
                                                List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                                market[sellerDiginotes[0]] = buyer;

                                                string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                                try
                                                {
                                                    SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                    command.ExecuteNonQuery();
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception);
                                                }

                                                numTransations += 1;
                                            }

                                            saleOrder.Quantity -= numTransations;
                                            saleOrder.Processed = false;
                                            buyOrder.Key.Processed = true;

                                            // Write to log text file
                                            StreamWriter file = new StreamWriter(@"log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();

                                            // Write to db log file
                                            string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), saleOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.WriteLine(exception);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (saleOrder.Processed)
                {
                    return "Sale order processed successfully!";
                }
                else
                {
                    return "Sale order not processed! Please specify a new sale value (must be less or equal than current quote).";
                }
            }
            else
            {
                return "Error: you do not have enough diginotes!";
            }
        }
        
        public string EditSaleOrder(int orderId, float orderValue)
        {
            foreach (var saleOrder in saleOrders)
            {
                if (saleOrder.Key.Id == orderId)
                {
                    if(saleOrder.Key.Processed == false)
                    {
                        saleOrder.Key.Value = orderValue;

                        foreach (var diginote in market)
                        {
                            diginote.Key.Quote = orderValue;
                        }


                        User seller;
                        User buyer;

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

                                                string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                                try
                                                {
                                                    SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                    command.ExecuteNonQuery();
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception);
                                                }

                                                numTransations += 1;
                                            }

                                            saleOrder.Key.Processed = true;
                                            buyOrder.Key.Processed = true;

                                            // Write to log text file
                                            StreamWriter file = new StreamWriter(@"log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();

                                            // Write to db log file
                                            string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
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

                                                    string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                                    try
                                                    {
                                                        SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                        command.ExecuteNonQuery();
                                                    }
                                                    catch (Exception exception)
                                                    {
                                                        Console.WriteLine(exception);
                                                    }

                                                    numTransations += 1;
                                                }

                                                buyOrder.Key.Quantity -= numTransations;
                                                saleOrder.Key.Processed = true;
                                                buyOrder.Key.Processed = false;

                                                // Write to log text file
                                                StreamWriter file = new StreamWriter(@"log.txt", true);
                                                file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                                file.Close();

                                                // Write to db log file
                                                string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                                try
                                                {
                                                    SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
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

                                                        string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                                        try
                                                        {
                                                            SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                            command.ExecuteNonQuery();
                                                        }
                                                        catch (Exception exception)
                                                        {
                                                            Console.WriteLine(exception);
                                                        }

                                                        numTransations += 1;
                                                    }

                                                    saleOrder.Key.Quantity -= numTransations;
                                                    saleOrder.Key.Processed = false;
                                                    buyOrder.Key.Processed = true;

                                                    // Write to log text file
                                                    StreamWriter file = new StreamWriter(@"log.txt", true);
                                                    file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                                    file.Close();

                                                    // Write to db log file
                                                    string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), saleOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                                    try
                                                    {
                                                        SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
                                                        command.ExecuteNonQuery();
                                                    }
                                                    catch (Exception exception)
                                                    {
                                                        Console.WriteLine(exception);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (saleOrder.Key.Processed)
                        {
                            return "Sale order edited successfully! Sale order processed successfully!";
                        }
                        else
                        {
                            return "Sale order edited successfully! Sale order not processed! Please specify a new sale value (must be less or equal than current quote).";
                        }
                    }
                    else
                    {
                        return "Error: Sale order already processed!";
                    }
                }
            }

            return "Error: Sale order not found!";
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
            BuyOrder buyOrder = new BuyOrder(quantity, GetQuote());

            buyOrders.Add(buyOrder, user);

            User seller;
            User buyer;

            buyer = user;

            if (buyOrder.Processed == false)
            {
                foreach (var saleOrder in saleOrders)
                {
                    if (saleOrder.Key.Processed == false)
                    {
                        seller = saleOrder.Value;

                        if (buyer.Nickname != seller.Nickname)
                        {
                            if (buyOrder.Quantity == saleOrder.Key.Quantity)
                            {
                                int numTransations = 0;

                                while (numTransations != buyOrder.Quantity)
                                {
                                    List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                    market[sellerDiginotes[0]] = buyer;

                                    string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                    try
                                    {
                                        SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }

                                    numTransations += 1;
                                }

                                buyOrder.Processed = true;
                                saleOrder.Key.Processed = true;

                                // Write to log text file
                                StreamWriter file = new StreamWriter(@"log.txt", true);
                                file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + buyOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                file.Close();

                                // Write to db log file
                                string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), buyOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                try
                                {
                                    SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
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
                                if (buyOrder.Quantity < saleOrder.Key.Quantity)
                                {
                                    int numTransations = 0;

                                    while (numTransations != buyOrder.Quantity)
                                    {
                                        List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                        market[sellerDiginotes[0]] = buyer;

                                        string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                        try
                                        {
                                            SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                            command.ExecuteNonQuery();
                                        }
                                        catch (Exception exception)
                                        {
                                            Console.WriteLine(exception);
                                        }

                                        numTransations += 1;
                                    }

                                    saleOrder.Key.Quantity -= numTransations;
                                    buyOrder.Processed = true;
                                    saleOrder.Key.Processed = false;

                                    // Write to log text file
                                    StreamWriter file = new StreamWriter(@"log.txt", true);
                                    file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + buyOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                    file.Close();

                                    // Write to db log file
                                    string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), buyOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                    try
                                    {
                                        SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
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
                                    if (buyOrder.Quantity > saleOrder.Key.Quantity)
                                    {
                                        int numTransations = 0;

                                        while (numTransations != saleOrder.Key.Quantity)
                                        {
                                            List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                            market[sellerDiginotes[0]] = buyer;

                                            string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                            try
                                            {
                                                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.WriteLine(exception);
                                            }

                                            numTransations += 1;
                                        }

                                        buyOrder.Quantity -= numTransations;
                                        buyOrder.Processed = false;
                                        saleOrder.Key.Processed = true;

                                        // Write to log text file
                                        StreamWriter file = new StreamWriter(@"log.txt", true);
                                        file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + buyOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                        file.Close();

                                        // Write to db log file
                                        string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), buyOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                        try
                                        {
                                            SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
                                            command.ExecuteNonQuery();
                                        }
                                        catch (Exception exception)
                                        {
                                            Console.WriteLine(exception);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            NotifyClients(Operation.Add);
            if (buyOrder.Processed)
            {
                return "Buy order processed successfully!";
            }
            else
            {
                return "Buy order not processed! Please specify a new buy value (must be greater or equal than current quote).";
            }
        }

        public string EditBuyOrder(int orderId, float orderValue)
        {
            foreach (var buyOrder in buyOrders)
            {
                if (buyOrder.Key.Id == orderId)
                {
                    if (buyOrder.Key.Processed == false)
                    {
                        buyOrder.Key.Value = orderValue;

                        foreach (var diginote in market)
                        {
                            diginote.Key.Quote = orderValue;
                        }


                        User seller;
                        User buyer;

                        buyer = buyOrder.Value;

                        if (buyOrder.Key.Processed == false)
                        {
                            foreach (var saleOrder in saleOrders)
                            {
                                if (saleOrder.Key.Processed == false)
                                {
                                    seller = saleOrder.Value;

                                    if (buyer.Nickname != seller.Nickname)
                                    {
                                        if (buyOrder.Key.Quantity == saleOrder.Key.Quantity)
                                        {
                                            int numTransations = 0;

                                            while (numTransations != buyOrder.Key.Quantity)
                                            {
                                                List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                                market[sellerDiginotes[0]] = buyer;

                                                string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                                try
                                                {
                                                    SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                    command.ExecuteNonQuery();
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception);
                                                }

                                                numTransations += 1;
                                            }

                                            buyOrder.Key.Processed = true;
                                            saleOrder.Key.Processed = true;

                                            // Write to log text file
                                            StreamWriter file = new StreamWriter(@"log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + buyOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();

                                            // Write to db log file
                                            string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), buyOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
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
                                            if (buyOrder.Key.Quantity < saleOrder.Key.Quantity)
                                            {
                                                int numTransations = 0;

                                                while (numTransations != buyOrder.Key.Quantity)
                                                {
                                                    List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                                    market[sellerDiginotes[0]] = buyer;

                                                    string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                                    try
                                                    {
                                                        SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                        command.ExecuteNonQuery();
                                                    }
                                                    catch (Exception exception)
                                                    {
                                                        Console.WriteLine(exception);
                                                    }

                                                    numTransations += 1;
                                                }

                                                saleOrder.Key.Quantity -= numTransations;
                                                buyOrder.Key.Processed = true;
                                                saleOrder.Key.Processed = false;

                                                // Write to log text file
                                                StreamWriter file = new StreamWriter(@"log.txt", true);
                                                file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + buyOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                                file.Close();

                                                // Write to db log file
                                                string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), buyOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                                try
                                                {
                                                    SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
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
                                                if (buyOrder.Key.Quantity > saleOrder.Key.Quantity)
                                                {
                                                    int numTransations = 0;

                                                    while (numTransations != saleOrder.Key.Quantity)
                                                    {
                                                        List<Diginote> sellerDiginotes = GetDiginotes(ref seller);
                                                        market[sellerDiginotes[0]] = buyer;

                                                        string sql = String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id, sellerDiginotes[0].Id);
                                                        try
                                                        {
                                                            SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                                                            command.ExecuteNonQuery();
                                                        }
                                                        catch (Exception exception)
                                                        {
                                                            Console.WriteLine(exception);
                                                        }

                                                        numTransations += 1;
                                                    }

                                                    buyOrder.Key.Quantity -= numTransations;
                                                    buyOrder.Key.Processed = false;
                                                    saleOrder.Key.Processed = true;

                                                    // Write to log text file
                                                    StreamWriter file = new StreamWriter(@"log.txt", true);
                                                    file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + buyOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                                    file.Close();

                                                    // Write to db log file
                                                    string sql_log = String.Format("INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')", string.Format("{0:HH:mm:ss tt}", DateTime.Now), buyOrder.Key.Quantity + " diginotes transferred from " + seller.Nickname + " to " + buyer.Nickname);
                                                    try
                                                    {
                                                        SqliteCommand command = new SqliteCommand(sql_log, m_dbConnection);
                                                        command.ExecuteNonQuery();
                                                    }
                                                    catch (Exception exception)
                                                    {
                                                        Console.WriteLine(exception);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (buyOrder.Key.Processed)
                        {
                            return "Buy order edited successfully! Buy order processed successfully!";
                        }
                        else
                        {
                            return "Buy order edited successfully! Buy order not processed! Please specify a new buy value (must be greater or equal than current quote).";
                        }
                    }
                    else
                    {
                        return "Error: Buy order already processed!";
                    }
                }
            }

            return "Error: Buy order not found!";
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


        void NotifyClients(Operation op)
        {
            if (alterEvent != null)
            {
                Delegate[] invkList = alterEvent.GetInvocationList();

                foreach (AlterDelegate handler in invkList)
                {
                    new Thread(() =>
                    {
                        try
                        {
                            handler(op);
                            Console.WriteLine("Invoking event handler");
                        }
                        catch (Exception)
                        {
                            alterEvent -= handler;
                            Console.WriteLine("Exception: Removed an event handler");
                        }
                    }).Start();
                }
            }
        }
    }

   

    public delegate void AlterDelegate(DES.Operation op);

    public interface IDES
    {
        event AlterDelegate alterEvent;
        
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
        string EditSaleOrder(int orderId, float orderValue);
        List<BuyOrder> GetBuyOrders(ref User user);
        string AddBuyOrder(ref User user, int quantity);
        string EditBuyOrder(int orderId, float orderValue);
        List<Diginote> GetDiginotes(ref User user);
    }

    public class AlterEventRepeater : MarshalByRefObject
    {
        public event AlterDelegate alterEvent;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Repeater(DES.Operation op)
        {
            if (alterEvent != null)
                alterEvent(op);
        }
    }
}