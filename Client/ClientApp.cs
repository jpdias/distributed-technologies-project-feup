using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Common;

namespace Client
{
    class ClientApp
    {
        static void Main(string[] args)
        {
            new ClientConsole();
        }

        class ClientConsole
        {
            private IDES _iDes;
            private User loggedUser;
            Dictionary<Diginote, User> market;

            public ClientConsole()
            {
                RemotingConfiguration.Configure("Client.exe.config", false);
                _iDes = (IDES)RemoteNew.New(typeof(IDES));
                Console.WriteLine("Diginote Exchange System v0.1");
                Console.WriteLine("Welcome!");
                string option;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Menu:");
                    if(loggedUser == null)
                    {
                        Console.WriteLine("1 - Login");
                        Console.WriteLine("2 - Add User");
                        Console.WriteLine("3 - Remove User");
                    }
                    else
                    {
                        Console.WriteLine("1 - Logout");
                        Console.WriteLine("2 - Market Quote");
                        Console.WriteLine("3 - My Diginotes");
                        Console.WriteLine("4 - New Sale Order");
                        Console.WriteLine("5 - New Buy Order");
                        Console.WriteLine("6 - My Sale Orders");
                        Console.WriteLine("7 - My Buy Orders");
                    }
                    Console.WriteLine("0 - Exit");
                    Console.Write("Option: ");
                    option = Console.ReadLine();
                    if (loggedUser == null)
                    {
                        switch (option)
                        {
                            case "0":
                                Application.Exit();
                                break;
                            case "1":
                                Console.WriteLine();
                                Login();
                                break;
                            case "2":
                                Console.WriteLine();
                                addUser();
                                break;
                            case "3":
                                Console.WriteLine();
                                removeUser();
                                break;
                            default:
                                Console.WriteLine("Invalid option!");
                                break;
                        }
                    }
                    else
                    {
                        switch (option)
                        {
                            case "0":
                                Application.Exit();
                                break;
                            case "1":
                                Console.WriteLine();
                                Logout();
                                break;
                            case "2":
                                Console.WriteLine();
                                GetQuote();
                                break;
                            case "3":
                                Console.WriteLine();
                                ListDiginotes();
                                break;
                            case "4":
                                Console.WriteLine();
                                AddSaleOrder();
                                break;
                            case "5":
                                Console.WriteLine();
                                AddBuyOrder();
                                break;
                            case "6":
                                Console.WriteLine();
                                ListSaleOrders();
                                break;
                            case "7":
                                Console.WriteLine();
                                ListBuyOrders();
                                break;
                            default:
                                Console.WriteLine("Invalid option!");
                                break;
                        }
                    }
                    
                }
                while (option != "0");
            }

            public void addUser()
            {
                Console.WriteLine("Add User");

                Console.Write("Name: ");
                string name = Console.ReadLine();
                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.AddUser(name, nickname, password);
                Console.WriteLine(result);
            }

            public void removeUser()
            {
                Console.WriteLine("Remove User");

                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.RemoveUser(nickname, password);
                Console.WriteLine(result);
            }

            public void Login()
            {
                Console.WriteLine("Login");

                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.Login(nickname, password);
                Console.WriteLine(result);
                if (result.Equals("Login successful!"))
                {
                    loggedUser = _iDes.GetUser(nickname);
                    Console.WriteLine("Hello " + loggedUser.Nickname + "!");
                }
            }

            public void Logout()
            {
                Console.WriteLine("Logout");

                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.Logout(nickname, password);
                Console.WriteLine(result);
                if (result.Equals("Logout successful!"))
                {
                    Console.WriteLine("See you soon " + loggedUser.Nickname + "!");
                    loggedUser = null;
                }
            }

            public void ListUsers()
            {
                Console.WriteLine("Users");

                List<User> users = _iDes.GetUsersList();

                if (users.Count == 0)
                {
                    Console.WriteLine("No registered users!");
                }
                else
                {
                    var userIndex = 0;
                    foreach (User user in users)
                    {
                        Console.WriteLine();
                        Console.WriteLine("User #" + (userIndex + 1));
                        Console.WriteLine("  Name: " + user.Name);
                        Console.WriteLine("  Nickname: " + user.Nickname);
                        Console.WriteLine("  LoggedIn: " + user.LoggedIn);
                        userIndex += 1;
                    }
                }
            }

            public void ListMarket()
            {
                Console.WriteLine("Market");

                market = _iDes.GetMarket();

                if (market.Count == 0)
                {
                    Console.WriteLine("Empty market!");
                }
                else
                {
                    foreach (var entry in market)
                    {
                        Console.WriteLine(entry.Key);

                        if (entry.Value != null)
                        {
                            Console.WriteLine(entry.Value);
                        }
                        else
                        {
                            Console.WriteLine("No associated user!");
                        }

                        Console.WriteLine();
                    }
                }
            }

            public void GetQuote()
            {
                Console.WriteLine("Current Quote");

                float quote = _iDes.GetQuote();

                if (quote == 0.0f)
                {
                    Console.WriteLine("No diginotes available on the market!");
                }
                else
                {
                    Console.WriteLine(quote + "€");
                }
            }

            public void ListDiginotes()
            {
                Console.WriteLine("My Diginotes");

                List<Diginote> diginotes = _iDes.GetDiginotes(ref loggedUser);

                if (diginotes.Count == 0)
                {
                    Console.WriteLine("No diginotes!");
                }
                else
                {
                    Console.WriteLine("You have " + diginotes.Count + " diginotes!");
                }
            }

            public void AddSaleOrder()
            {
                Console.WriteLine("New Sale Order");

                Console.Write("Order's quantity: ");
                int quantity = int.Parse(Console.ReadLine());

                string result = _iDes.AddSaleOrder(ref loggedUser, quantity);

                Console.WriteLine(result);
            }

            public void AddBuyOrder()
            {
                Console.WriteLine("New Buy Order");

                Console.Write("Order's quantity: ");
                int quantity = int.Parse(Console.ReadLine());

                _iDes.AddBuyOrder(ref loggedUser, quantity);

                Console.WriteLine("New buy order added successfully!");
            }

            public void ListSaleOrders()
            {
                Console.WriteLine("My Sale Orders");

                List<SaleOrder> saleOrders = _iDes.GetSaleOrders(ref loggedUser);

                if (saleOrders.Count == 0)
                {
                    Console.WriteLine("No sale orders!");
                }
                else
                {
                    var saleOrderIndex = 1;
                    foreach(Order saleOrder in saleOrders)
                    {
                        Console.WriteLine("Order #" + saleOrderIndex + "'s quantity: " + saleOrder.Quantity);
                        saleOrderIndex += 1;
                    }
                }

                Console.WriteLine();
                string option;
                do
                {
                    Console.Write("Do you want to edit any order's quantity? (Y/N) ");
                    option = Console.ReadLine();
                    if (option == "Y" || option == "y")
                    {
                        Console.Write("Please enter the order index: ");
                        int orderIndex = int.Parse(Console.ReadLine()) - 1;
                        Console.Write("Please enter the new quantity: ");
                        int quantity = int.Parse(Console.ReadLine());

                        _iDes.EditSaleOrder(ref loggedUser, orderIndex, quantity);

                        break;
                    }
                    else
                    {
                        if (option != "N" && option != "n")
                        {
                            Console.WriteLine("Invalid option!");
                            option = "INVALID_OPTION";
                        }
                    }
                }
                while (option == "INVALID_OPTION");

                if(option == "Y" || option == "y")
                {
                    Console.WriteLine();
                    ListSaleOrders();
                }
            }

            public void ListBuyOrders()
            {
                Console.WriteLine("My Buy Orders");

                List<BuyOrder> buyOrders = _iDes.GetBuyOrders(ref loggedUser);

                if (buyOrders.Count == 0)
                {
                    Console.WriteLine("No buy orders!");
                }
                else
                {
                    var buyOrderIndex = 1;
                    foreach (Order buyOrder in buyOrders)
                    {
                        Console.WriteLine("Order #" + buyOrderIndex + "'s quantity: " + buyOrder.Quantity);
                        buyOrderIndex += 1;
                    }
                }

                Console.WriteLine();
                string option;
                do
                {
                    Console.Write("Do you want to edit any order's quantity? (Y/N) ");
                    option = Console.ReadLine();
                    if (option == "Y" || option == "y")
                    {
                        Console.Write("Please enter the order index: ");
                        int orderIndex = int.Parse(Console.ReadLine()) - 1;
                        Console.Write("Please enter the new quantity: ");
                        int quantity = int.Parse(Console.ReadLine());

                        _iDes.EditBuyOrder(ref loggedUser, orderIndex, quantity);

                        break;
                    }
                    else
                    {
                        if (option != "N" && option != "n")
                        {
                            Console.WriteLine("Invalid option!");
                            option = "INVALID_OPTION";
                        }
                    }
                }
                while (option == "INVALID_OPTION");

                if (option == "Y" || option == "y")
                {
                    Console.WriteLine();
                    ListBuyOrders();
                }
            }
        }

        /* Mechanism for instanciating a remote object through its interface, using the config file */
        class RemoteNew
        {
            private static Hashtable types = null;

            private static void InitTypeTable()
            {
                types = new Hashtable();
                foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                    types.Add(entry.ObjectType, entry);
            }

            public static object New(Type type)
            {
                if (types == null)
                    InitTypeTable();
                WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)types[type];
                if (entry == null)
                    throw new RemotingException("Type not found!");
                return RemotingServices.Connect(type, entry.ObjectUrl);
            }
        }
    }
}
