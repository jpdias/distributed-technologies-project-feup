using System;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
    public class DES : MarshalByRefObject, IDES
    {
        ArrayList usersList;
        ArrayList diginotesList;
        Dictionary<Diginote, User> market;

        public DES()
        {
            Console.WriteLine("Constructor called.");
            usersList = new ArrayList();
            diginotesList = new ArrayList();
            market = new Dictionary<Diginote, User>();

            User user = new User("Joao", "joao", "joao");
            user.AddBuyOrder(10);
            usersList.Add(user);

            Diginote diginote = new Diginote();
            diginote.Quote = 0.98f;
            diginotesList.Add(diginote);
            market.Add(diginote, user);

            diginote = new Diginote();
            diginote.Quote = 1.00f;
            diginotesList.Add(diginote);
            market.Add(diginote, null);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public string AddUser(string name, string nickname, string password)
        {
            Console.WriteLine("AddUser called.");

            foreach (User user in usersList)
            {
                if (user.Nickname.Equals(nickname))
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
            foreach (User user in usersList)
            {
                if (user.Nickname.Equals(nickname))
                {
                    if (user.Password.Equals(password))
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