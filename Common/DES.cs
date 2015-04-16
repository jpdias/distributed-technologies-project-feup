using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Community.CsharpSqlite.SQLiteClient;
using Timer = System.Timers.Timer;

namespace Common
{
    public class DES : MarshalByRefObject, IDES
    {
        public enum Operation
        {
            Add,
            Change,
            StartSuspension,
            EndSuspension
        };

        private readonly int _interval = 60000;
        private readonly Dictionary<BuyOrder, User> buyOrders;
        private readonly List<Diginote> diginotesList;
        private readonly SqliteConnection m_dbConnection;
        private readonly Dictionary<Diginote, User> market;
        private readonly Dictionary<SaleOrder, User> saleOrders;
        private readonly List<User> usersList;
        private Timer _timer;

        public DES()
        {
            _timer = new Timer(_interval);
            _timer.Elapsed += timer_Tick;

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
            saleOrders = GetSaleOrdersFromDb();
            buyOrders = GetBuyOrdersFromDb();
        }

        public event AlterDelegate alterEvent;

        public string AddUser(string name, string nickname, string password)
        {
            Console.WriteLine("AddUser called.");

            if ((usersList.Any(p => p.Nickname == nickname)) == false)
            {
                var newUser = new User(name, nickname, password);
                usersList.Add(newUser);

                var sql =
                    String.Format(
                        "INSERT INTO MarketUsers ('id', 'nickname', 'username', 'password') VALUES ('{0}', '{1}', '{2}', '{3}')",
                        newUser.Id, nickname, name, password);
                try
                {
                    var command = new SqliteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "Error adding user to db!";
                }

                // Give 10 Diginotes to the new user
                for (var i = 0; i < 10; i++)
                {
                    AddDiginote(usersList.Count);
                }

                return "User added successfully!";
            }
            return "Username already exists!";
        }

        public string RemoveUser(string nickname, string password)
        {
            Console.WriteLine("RemoveUser called.");

            var userIndex = 0;
            foreach (var user in usersList)
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

            foreach (var user in usersList)
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
                        return "Login error: User is already logged in!";
                    }
                    return "Login error: Wrong password!";
                }
            }

            return "Login error: Nickname not found!";
        }

        public User GetUser(string nickname)
        {
            Console.WriteLine("GetUser called.");

            foreach (var user in usersList)
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

            foreach (var user in usersList)
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
                        return "Logout error: User is not logged in!";
                    }
                    return "Logout error: Wrong password!";
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
            var userSaleOrders = new List<SaleOrder>();

            foreach (var saleOrder in saleOrders)
            {
                if (saleOrder.Value.Nickname.Equals(user.Nickname))
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
                var saleOrder = new SaleOrder(quantity, GetQuote());

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
                                    var numTransations = 0;

                                    while (numTransations != saleOrder.Quantity)
                                    {
                                        var sellerDiginotes = GetDiginotes(ref seller);
                                        market[sellerDiginotes[0]] = buyer;

                                        var sql =
                                            String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'",
                                                buyer.Id, sellerDiginotes[0].Id);
                                        try
                                        {
                                            var command = new SqliteCommand(sql, m_dbConnection);
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
                                    var file = new StreamWriter(@"log.txt", true);
                                    file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Quantity +
                                                   " diginotes transferred from " + seller.Nickname + " to " +
                                                   buyer.Nickname);
                                    file.Close();

                                    // Write to db log file
                                    var sql_log =
                                        String.Format(
                                            "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                            string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                            saleOrder.Quantity + " diginotes transferred from " + seller.Nickname +
                                            " to " + buyer.Nickname);
                                    try
                                    {
                                        var command = new SqliteCommand(sql_log, m_dbConnection);
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }

                                    break;
                                }
                                if (saleOrder.Quantity < buyOrder.Key.Quantity)
                                {
                                    var numTransations = 0;

                                    while (numTransations != saleOrder.Quantity)
                                    {
                                        var sellerDiginotes = GetDiginotes(ref seller);
                                        market[sellerDiginotes[0]] = buyer;

                                        var sql =
                                            String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'",
                                                buyer.Id, sellerDiginotes[0].Id);
                                        try
                                        {
                                            var command = new SqliteCommand(sql, m_dbConnection);
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
                                    var file = new StreamWriter(@"log.txt", true);
                                    file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Quantity +
                                                   " diginotes transferred from " + seller.Nickname + " to " +
                                                   buyer.Nickname);
                                    file.Close();

                                    // Write to db log file
                                    var sql_log =
                                        String.Format(
                                            "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                            string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                            saleOrder.Quantity + " diginotes transferred from " + seller.Nickname +
                                            " to " + buyer.Nickname);
                                    try
                                    {
                                        var command = new SqliteCommand(sql_log, m_dbConnection);
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }

                                    break;
                                }
                                if (saleOrder.Quantity > buyOrder.Key.Quantity)
                                {
                                    var numTransations = 0;

                                    while (numTransations != buyOrder.Key.Quantity)
                                    {
                                        var sellerDiginotes = GetDiginotes(ref seller);
                                        market[sellerDiginotes[0]] = buyer;

                                        var sql =
                                            String.Format("UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'",
                                                buyer.Id, sellerDiginotes[0].Id);
                                        try
                                        {
                                            var command = new SqliteCommand(sql, m_dbConnection);
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
                                    var file = new StreamWriter(@"log.txt", true);
                                    file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + saleOrder.Quantity +
                                                   " diginotes transferred from " + seller.Nickname + " to " +
                                                   buyer.Nickname);
                                    file.Close();

                                    // Write to db log file
                                    var sql_log =
                                        String.Format(
                                            "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                            string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                            saleOrder.Quantity + " diginotes transferred from " + seller.Nickname +
                                            " to " + buyer.Nickname);
                                    try
                                    {
                                        var command = new SqliteCommand(sql_log, m_dbConnection);
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


                var _sql =
                    String.Format(
                        "INSERT INTO SaleOrders ('id', 'quantity', 'value', 'processed', 'userId') VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                        saleOrder.Id, saleOrder.Quantity, saleOrder.Value, saleOrder.Processed, user.Id);
                try
                {
                    var command = new SqliteCommand(_sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "Error adding sale order to db!";
                }


                /*Notifications*/
                NotifyClients(Operation.Add);

                if (saleOrder.Processed)
                {
                    return "Sale order processed successfully!";
                }
                return
                    "Sale order not processed! Please specify a new sale value (must be less or equal than current quote).";
            }
            return "Error: you do not have enough diginotes!";
        }

        public string EditSaleOrder(int orderId, float orderValue)
        {
            foreach (var saleOrder in saleOrders)
            {
                if (saleOrder.Key.Id == orderId)
                {
                    if (saleOrder.Key.Processed == false)
                    {
                        saleOrder.Key.Value = orderValue;

                        // Update diginotes value
                        foreach (var diginote in market)
                        {
                            diginote.Key.Quote = orderValue;
                        }

                        // Update sale orders value
                        foreach (var innerSaleOrder in saleOrders)
                        {
                            innerSaleOrder.Key.Value = orderValue;
                        }
                        // Update buy orders value
                        foreach (var innerBuyOrder in buyOrders)
                        {
                            innerBuyOrder.Key.Value = orderValue;
                        }

                        _timer.Start();
                        NotifyClients(Operation.StartSuspension);

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
                                            var numTransations = 0;

                                            while (numTransations != saleOrder.Key.Quantity)
                                            {
                                                var sellerDiginotes = GetDiginotes(ref seller);
                                                market[sellerDiginotes[0]] = buyer;

                                                var sql =
                                                    String.Format(
                                                        "UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'",
                                                        buyer.Id, sellerDiginotes[0].Id);
                                                try
                                                {
                                                    var command = new SqliteCommand(sql, m_dbConnection);
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
                                            var file = new StreamWriter(@"log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) +
                                                           saleOrder.Key.Quantity + " diginotes transferred from " +
                                                           seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();

                                            // Write to db log file
                                            var sql_log =
                                                String.Format(
                                                    "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                                    string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                                    saleOrder.Key.Quantity + " diginotes transferred from " +
                                                    seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                var command = new SqliteCommand(sql_log, m_dbConnection);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.WriteLine(exception);
                                            }

                                            break;
                                        }
                                        if (saleOrder.Key.Quantity < buyOrder.Key.Quantity)
                                        {
                                            var numTransations = 0;

                                            while (numTransations != saleOrder.Key.Quantity)
                                            {
                                                var sellerDiginotes = GetDiginotes(ref seller);
                                                market[sellerDiginotes[0]] = buyer;

                                                var sql =
                                                    String.Format(
                                                        "UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'",
                                                        buyer.Id, sellerDiginotes[0].Id);
                                                try
                                                {
                                                    var command = new SqliteCommand(sql, m_dbConnection);
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
                                            var file = new StreamWriter(@"log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) +
                                                           saleOrder.Key.Quantity + " diginotes transferred from " +
                                                           seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();

                                            // Write to db log file
                                            var sql_log =
                                                String.Format(
                                                    "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                                    string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                                    saleOrder.Key.Quantity + " diginotes transferred from " +
                                                    seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                var command = new SqliteCommand(sql_log, m_dbConnection);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.WriteLine(exception);
                                            }

                                            break;
                                        }
                                        if (saleOrder.Key.Quantity > buyOrder.Key.Quantity)
                                        {
                                            var numTransations = 0;

                                            while (numTransations != buyOrder.Key.Quantity)
                                            {
                                                var sellerDiginotes = GetDiginotes(ref seller);
                                                market[sellerDiginotes[0]] = buyer;

                                                var sql =
                                                    String.Format(
                                                        "UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'",
                                                        buyer.Id, sellerDiginotes[0].Id);
                                                try
                                                {
                                                    var command = new SqliteCommand(sql, m_dbConnection);
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
                                            var file = new StreamWriter(@"log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) +
                                                           saleOrder.Key.Quantity + " diginotes transferred from " +
                                                           seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();

                                            // Write to db log file
                                            var sql_log =
                                                String.Format(
                                                    "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                                    string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                                    saleOrder.Key.Quantity + " diginotes transferred from " +
                                                    seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                var command = new SqliteCommand(sql_log, m_dbConnection);
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


                        var _sql = String.Format("UPDATE SaleOrders SET value = '{0}' WHERE id = '{1}'",
                            saleOrder.Key.Value, saleOrder.Key.Id);
                        try
                        {
                            var command = new SqliteCommand(_sql, m_dbConnection);
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return "Error editing sale order to db!";
                        }


                        /*Notifications*/
                        NotifyClients(Operation.Change);

                        if (saleOrder.Key.Processed)
                        {
                            return "Sale order edited successfully! Sale order processed successfully!";
                        }
                        return
                            "Sale order edited successfully! Sale order not processed! Please specify a new sale value (must be less or equal than current quote).";
                    }
                    return "Error: Sale order already processed!";
                }
            }

            return "Error: Sale order not found!";
        }

        public string RemoveSaleOrder(int orderId)
        {
            foreach (var saleOrder in saleOrders)
            {
                if (saleOrder.Key.Id == orderId)
                {
                    saleOrders.Remove(saleOrder.Key);


                    var _sql = String.Format("DELETE FROM SaleOrders WHERE id = '{0}'", saleOrder.Key.Id);
                    try
                    {
                        var command = new SqliteCommand(_sql, m_dbConnection);
                        command.ExecuteNonQuery();
                        NotifyClients(Operation.Change);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return "Error removing sale order to db!";
                    }


                    return "Sale order removed successfully!";
                }
            }

            return "Error: Sale order not found!";
        }

        public List<BuyOrder> GetBuyOrders(ref User user)
        {
            var userBuyOrders = new List<BuyOrder>();

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
            var buyOrder = new BuyOrder(quantity, GetQuote());

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
                                var numTransations = 0;

                                while (numTransations != buyOrder.Quantity)
                                {
                                    var sellerDiginotes = GetDiginotes(ref seller);
                                    market[sellerDiginotes[0]] = buyer;

                                    var sql = String.Format(
                                        "UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id,
                                        sellerDiginotes[0].Id);
                                    try
                                    {
                                        var command = new SqliteCommand(sql, m_dbConnection);
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
                                var file = new StreamWriter(@"log.txt", true);
                                file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + buyOrder.Quantity +
                                               " diginotes transferred from " + seller.Nickname + " to " +
                                               buyer.Nickname);
                                file.Close();

                                // Write to db log file
                                var sql_log =
                                    String.Format(
                                        "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                        string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                        buyOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " +
                                        buyer.Nickname);
                                try
                                {
                                    var command = new SqliteCommand(sql_log, m_dbConnection);
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                }

                                break;
                            }
                            if (buyOrder.Quantity < saleOrder.Key.Quantity)
                            {
                                var numTransations = 0;

                                while (numTransations != buyOrder.Quantity)
                                {
                                    var sellerDiginotes = GetDiginotes(ref seller);
                                    market[sellerDiginotes[0]] = buyer;

                                    var sql = String.Format(
                                        "UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id,
                                        sellerDiginotes[0].Id);
                                    try
                                    {
                                        var command = new SqliteCommand(sql, m_dbConnection);
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
                                var file = new StreamWriter(@"log.txt", true);
                                file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + buyOrder.Quantity +
                                               " diginotes transferred from " + seller.Nickname + " to " +
                                               buyer.Nickname);
                                file.Close();

                                // Write to db log file
                                var sql_log =
                                    String.Format(
                                        "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                        string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                        buyOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " +
                                        buyer.Nickname);
                                try
                                {
                                    var command = new SqliteCommand(sql_log, m_dbConnection);
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                }

                                break;
                            }
                            if (buyOrder.Quantity > saleOrder.Key.Quantity)
                            {
                                var numTransations = 0;

                                while (numTransations != saleOrder.Key.Quantity)
                                {
                                    var sellerDiginotes = GetDiginotes(ref seller);
                                    market[sellerDiginotes[0]] = buyer;

                                    var sql = String.Format(
                                        "UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'", buyer.Id,
                                        sellerDiginotes[0].Id);
                                    try
                                    {
                                        var command = new SqliteCommand(sql, m_dbConnection);
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
                                var file = new StreamWriter(@"log.txt", true);
                                file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + buyOrder.Quantity +
                                               " diginotes transferred from " + seller.Nickname + " to " +
                                               buyer.Nickname);
                                file.Close();

                                // Write to db log file
                                var sql_log =
                                    String.Format(
                                        "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                        string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                        buyOrder.Quantity + " diginotes transferred from " + seller.Nickname + " to " +
                                        buyer.Nickname);
                                try
                                {
                                    var command = new SqliteCommand(sql_log, m_dbConnection);
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


            var _sql =
                String.Format(
                    "INSERT INTO BuyOrders ('id', 'quantity', 'value', 'processed', 'userId') VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                    buyOrder.Id, buyOrder.Quantity, buyOrder.Value, buyOrder.Processed, user.Id);
            try
            {
                var command = new SqliteCommand(_sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Error editing buy order to db!";
            }


            /*Notifications*/
            NotifyClients(Operation.Add);

            if (buyOrder.Processed)
            {
                return "Buy order processed successfully!";
            }
            return
                "Buy order not processed! Please specify a new buy value (must be greater or equal than current quote).";
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

                        // Update diginotes value
                        foreach (var diginote in market)
                        {
                            diginote.Key.Quote = orderValue;
                        }

                        // Update sale orders value
                        foreach (var innerSaleOrder in saleOrders)
                        {
                            innerSaleOrder.Key.Value = orderValue;
                        }
                        // Update buy orders value
                        foreach (var innerBuyOrder in buyOrders)
                        {
                            innerBuyOrder.Key.Value = orderValue;
                        }

                        _timer.Start();
                        NotifyClients(Operation.StartSuspension);


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
                                            var numTransations = 0;

                                            while (numTransations != buyOrder.Key.Quantity)
                                            {
                                                var sellerDiginotes = GetDiginotes(ref seller);
                                                market[sellerDiginotes[0]] = buyer;

                                                var sql =
                                                    String.Format(
                                                        "UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'",
                                                        buyer.Id, sellerDiginotes[0].Id);
                                                try
                                                {
                                                    var command = new SqliteCommand(sql, m_dbConnection);
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
                                            var file = new StreamWriter(@"log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) +
                                                           buyOrder.Key.Quantity + " diginotes transferred from " +
                                                           seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();

                                            // Write to db log file
                                            var sql_log =
                                                String.Format(
                                                    "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                                    string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                                    buyOrder.Key.Quantity + " diginotes transferred from " +
                                                    seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                var command = new SqliteCommand(sql_log, m_dbConnection);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.WriteLine(exception);
                                            }

                                            break;
                                        }
                                        if (buyOrder.Key.Quantity < saleOrder.Key.Quantity)
                                        {
                                            var numTransations = 0;

                                            while (numTransations != buyOrder.Key.Quantity)
                                            {
                                                var sellerDiginotes = GetDiginotes(ref seller);
                                                market[sellerDiginotes[0]] = buyer;

                                                var sql =
                                                    String.Format(
                                                        "UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'",
                                                        buyer.Id, sellerDiginotes[0].Id);
                                                try
                                                {
                                                    var command = new SqliteCommand(sql, m_dbConnection);
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
                                            var file = new StreamWriter(@"log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) +
                                                           buyOrder.Key.Quantity + " diginotes transferred from " +
                                                           seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();

                                            // Write to db log file
                                            var sql_log =
                                                String.Format(
                                                    "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                                    string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                                    buyOrder.Key.Quantity + " diginotes transferred from " +
                                                    seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                var command = new SqliteCommand(sql_log, m_dbConnection);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.WriteLine(exception);
                                            }

                                            break;
                                        }
                                        if (buyOrder.Key.Quantity > saleOrder.Key.Quantity)
                                        {
                                            var numTransations = 0;

                                            while (numTransations != saleOrder.Key.Quantity)
                                            {
                                                var sellerDiginotes = GetDiginotes(ref seller);
                                                market[sellerDiginotes[0]] = buyer;

                                                var sql =
                                                    String.Format(
                                                        "UPDATE Market SET userId = '{0}' WHERE diginoteId = '{1}'",
                                                        buyer.Id, sellerDiginotes[0].Id);
                                                try
                                                {
                                                    var command = new SqliteCommand(sql, m_dbConnection);
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
                                            var file = new StreamWriter(@"log.txt", true);
                                            file.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now) +
                                                           buyOrder.Key.Quantity + " diginotes transferred from " +
                                                           seller.Nickname + " to " + buyer.Nickname);
                                            file.Close();

                                            // Write to db log file
                                            var sql_log =
                                                String.Format(
                                                    "INSERT INTO MarketLog ('time', 'description') VALUES ('{0}', '{1}')",
                                                    string.Format("{0:HH:mm:ss tt}", DateTime.Now),
                                                    buyOrder.Key.Quantity + " diginotes transferred from " +
                                                    seller.Nickname + " to " + buyer.Nickname);
                                            try
                                            {
                                                var command = new SqliteCommand(sql_log, m_dbConnection);
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


                        var _sql = String.Format("UPDATE BuyOrders SET value = '{0}' WHERE id = '{1}'",
                            buyOrder.Key.Value, buyOrder.Key.Id);
                        try
                        {
                            var command = new SqliteCommand(_sql, m_dbConnection);
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return "Error adding buy order to db!";
                        }


                        /*Notifications*/
                        NotifyClients(Operation.Change);

                        if (buyOrder.Key.Processed)
                        {
                            return "Buy order edited successfully! Buy order processed successfully!";
                        }
                        return
                            "Buy order edited successfully! Buy order not processed! Please specify a new buy value (must be greater or equal than current quote).";
                    }
                    return "Error: Buy order already processed!";
                }
            }

            return "Error: Buy order not found!";
        }

        public string RemoveBuyOrder(int orderId)
        {
            foreach (var buyOrder in buyOrders)
            {
                if (buyOrder.Key.Id == orderId)
                {
                    buyOrders.Remove(buyOrder.Key);


                    var _sql = String.Format("DELETE FROM SaleOrders WHERE id = '{0}'", buyOrder.Key.Id);
                    try
                    {
                        var command = new SqliteCommand(_sql, m_dbConnection);
                        command.ExecuteNonQuery();
                        NotifyClients(Operation.Change);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return "Error removing buy order to db!";
                    }


                    return "Buy order removed successfully!";
                }
            }

            return "Error: Sale order not found!";
        }

        public List<Diginote> GetDiginotes(ref User user)
        {
            var diginotes = new List<Diginote>();

            foreach (var diginote in market)
            {
                if (diginote.Value.Nickname.Equals(user.Nickname))
                {
                    diginotes.Add(diginote.Key);
                }
            }

            return diginotes;
        }

        public User GetUserFromOrder(Order order)
        {
            foreach (var saleOrder in saleOrders)
            {
                if (saleOrder.Key.Id == order.Id)
                {
                    return saleOrder.Value;
                }
            }

            foreach (var buyOrder in buyOrders)
            {
                if (buyOrder.Key.Id == order.Id)
                {
                    Console.WriteLine(buyOrder.Value);
                    return buyOrder.Value;
                }
            }

            return usersList[0];
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer = new Timer(_interval);
            NotifyClients(Operation.EndSuspension);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public List<User> GetUsersListFromDb()
        {
            var result = new List<User>();
            var sql = "SELECT * FROM MarketUsers";
            try
            {
                var command = new SqliteCommand(sql, m_dbConnection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new User(reader.GetInt32(0), reader["username"].ToString(), reader["nickname"].ToString(),
                        reader["password"].ToString()));
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }

        public List<Diginote> GetDiginotesListFromDb()
        {
            var result = new List<Diginote>();
            var sql = "SELECT * FROM MarketDiginotes";
            try
            {
                var command = new SqliteCommand(sql, m_dbConnection);
                var reader = command.ExecuteReader();
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
            var result = new Dictionary<Diginote, User>();
            var sql = "SELECT * FROM Market";
            try
            {
                var command = new SqliteCommand(sql, m_dbConnection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var diginoteId = reader.GetInt32(1);
                    var userId = reader.GetInt32(2);
                    result.Add(diginotesList[diginoteId - 1], usersList[userId - 1]);
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }

        public Dictionary<SaleOrder, User> GetSaleOrdersFromDb()
        {
            var result = new Dictionary<SaleOrder, User>();
            var sql = "SELECT * FROM SaleOrders";
            try
            {
                var command = new SqliteCommand(sql, m_dbConnection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var quantity = reader.GetInt32(1);
                    var valueString = reader.GetString(2);
                    if (valueString.Contains(","))
                        valueString = valueString.Replace(",", ".");
                    var value = float.Parse(valueString, CultureInfo.InvariantCulture.NumberFormat);
                    var processed = reader.GetBoolean(3);
                    var userId = reader.GetInt32(4);
                    result.Add(new SaleOrder(id, quantity, value, processed), usersList[userId - 1]);
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }

        public Dictionary<BuyOrder, User> GetBuyOrdersFromDb()
        {
            var result = new Dictionary<BuyOrder, User>();
            var sql = "SELECT * FROM BuyOrders";
            try
            {
                var command = new SqliteCommand(sql, m_dbConnection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var quantity = reader.GetInt32(1);
                    var valueString = reader.GetString(2);
                    if (valueString.Contains(","))
                        valueString = valueString.Replace(",", ".");
                    var value = float.Parse(valueString, CultureInfo.InvariantCulture.NumberFormat);
                    var processed = reader.GetBoolean(3);
                    var userId = reader.GetInt32(4);
                    result.Add(new BuyOrder(id, quantity, value, processed), usersList[userId - 1]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        public string AddDiginote(int userId)
        {
            Console.WriteLine("AddDiginote called.");

            var newDiginote = new Diginote();
            diginotesList.Add(newDiginote);

            var sql = String.Format("INSERT INTO MarketDiginotes ('id') VALUES ('{0}')", newDiginote.Id);
            try
            {
                var command = new SqliteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Error adding diginote to db!";
            }

            market.Add(diginotesList[newDiginote.Id - 1], usersList[userId - 1]);

            sql = String.Format("INSERT INTO Market ('diginoteId', 'userId') VALUES ('{0}', '{1}')", newDiginote.Id,
                userId);
            try
            {
                var command = new SqliteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Error adding diginote to db!";
            }

            return "Diginote added successfully!";
        }

        private void NotifyClients(Operation op)
        {
            if (alterEvent != null)
            {
                var invkList = alterEvent.GetInvocationList();

                foreach (AlterDelegate handler in invkList)
                {
                    new Thread(() =>
                    {
                        try
                        {
                            handler(op);
                        }
                        catch (Exception)
                        {
                            alterEvent -= handler;
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
        string RemoveSaleOrder(int orderId);
        List<BuyOrder> GetBuyOrders(ref User user);
        string AddBuyOrder(ref User user, int quantity);
        string EditBuyOrder(int orderId, float orderValue);
        string RemoveBuyOrder(int orderId);
        List<Diginote> GetDiginotes(ref User user);
        User GetUserFromOrder(Order order);
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