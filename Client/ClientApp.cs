using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Common;

namespace Client
{
    internal class ClientApp
    {
        private static void Main(string[] args)
        {
            new ClientConsole();
        }

        private class ClientConsole
        {
            private readonly IDES _iDes;
            private User loggedUser;
            private Dictionary<Diginote, User> market;

            public ClientConsole()
            {
                RemotingConfiguration.Configure("Client.exe.config", false);
                _iDes = (IDES) RemoteNew.New(typeof (IDES));
                Console.WriteLine("Diginote Exchange System v0.1");
                Console.WriteLine("Welcome!");
                string option;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Menu:");
                    if (loggedUser == null)
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
                } while (option != "0");
            }

            public void addUser()
            {
                Console.WriteLine("Add User");

                Console.Write("Name: ");
                var name = Console.ReadLine();
                Console.Write("Nickname: ");
                var nickname = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();
                var result = _iDes.AddUser(name, nickname, password);
                Console.WriteLine(result);
            }

            public void removeUser()
            {
                Console.WriteLine("Remove User");

                Console.Write("Nickname: ");
                var nickname = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();
                var result = _iDes.RemoveUser(nickname, password);
                Console.WriteLine(result);
            }

            public void Login()
            {
                Console.WriteLine("Login");

                Console.Write("Nickname: ");
                var nickname = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();
                var result = _iDes.Login(nickname, password);
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
                var nickname = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();
                var result = _iDes.Logout(nickname, password);
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

                var users = _iDes.GetUsersList();

                if (users.Count == 0)
                {
                    Console.WriteLine("No registered users!");
                }
                else
                {
                    var userIndex = 0;
                    foreach (var user in users)
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

                var quote = _iDes.GetQuote();

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

                var diginotes = _iDes.GetDiginotes(ref loggedUser);

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
                var quantity = int.Parse(Console.ReadLine());

                var result = _iDes.AddSaleOrder(ref loggedUser, quantity);

                Console.WriteLine(result);
            }

            public void AddBuyOrder()
            {
                Console.WriteLine("New Buy Order");

                Console.Write("Order's quantity: ");
                var quantity = int.Parse(Console.ReadLine());

                var result = _iDes.AddBuyOrder(ref loggedUser, quantity);

                Console.WriteLine(result);
            }

            public void ListSaleOrders()
            {
                Console.WriteLine("My Sale Orders");

                var saleOrders = _iDes.GetSaleOrders(ref loggedUser);

                if (saleOrders.Count == 0)
                {
                    Console.WriteLine("No sale orders!");
                }
                else
                {
                    var saleOrderIndex = 1;
                    foreach (Order saleOrder in saleOrders)
                    {
                        Console.WriteLine("Id: " + saleOrderIndex + "; " + "Quantity: " + saleOrder.Quantity + "; " +
                                          "Value: " + saleOrder.Value);
                        Console.WriteLine("Processed: " + saleOrder.Processed);
                        saleOrderIndex += 1;
                    }
                }

                Console.WriteLine();
                string option;
                do
                {
                    Console.Write(
                        "Do you want to edit any order's value (must be less or equal than current quote)? (Y/N) ");
                    option = Console.ReadLine();
                    if (option == "Y" || option == "y")
                    {
                        Console.Write("Please enter the order id: ");
                        var orderId = int.Parse(Console.ReadLine());

                        float orderValue = 0;
                        do
                        {
                            Console.Write("Please enter the new value: ");
                            orderValue = float.Parse(Console.ReadLine());
                        } while (orderValue > _iDes.GetQuote());

                        var result = _iDes.EditSaleOrder(orderId, orderValue);
                        Console.WriteLine(result);

                        break;
                    }
                    if (option != "N" && option != "n")
                    {
                        Console.WriteLine("Invalid option!");
                        option = "INVALID_OPTION";
                    }
                } while (option == "INVALID_OPTION");

                if (option == "Y" || option == "y")
                {
                    Console.WriteLine();
                    ListSaleOrders();
                }
            }

            public void ListBuyOrders()
            {
                Console.WriteLine("My Buy Orders");

                var buyOrders = _iDes.GetBuyOrders(ref loggedUser);

                if (buyOrders.Count == 0)
                {
                    Console.WriteLine("No buy orders!");
                }
                else
                {
                    var buyOrderIndex = 1;
                    foreach (Order buyOrder in buyOrders)
                    {
                        Console.WriteLine("Id: " + buyOrderIndex + "; " + "Quantity: " + buyOrder.Quantity + "; " +
                                          "Value: " + +buyOrder.Value);
                        Console.WriteLine("Processed: " + buyOrder.Processed);
                        buyOrderIndex += 1;
                    }
                }

                Console.WriteLine();
                string option;
                do
                {
                    Console.Write(
                        "Do you want to edit any order's value (must be greater or equal than current quote)? (Y/N) ");
                    option = Console.ReadLine();
                    if (option == "Y" || option == "y")
                    {
                        Console.Write("Please enter the order id: ");
                        var orderId = int.Parse(Console.ReadLine());

                        float orderValue = 0;
                        do
                        {
                            Console.Write("Please enter the new value: ");
                            orderValue = float.Parse(Console.ReadLine());
                        } while (orderValue > _iDes.GetQuote());

                        var result = _iDes.EditBuyOrder(orderId, orderValue);
                        Console.WriteLine(result);

                        break;
                    }
                    if (option != "N" && option != "n")
                    {
                        Console.WriteLine("Invalid option!");
                        option = "INVALID_OPTION";
                    }
                } while (option == "INVALID_OPTION");

                if (option == "Y" || option == "y")
                {
                    Console.WriteLine();
                    ListBuyOrders();
                }
            }
        }

        /* Mechanism for instanciating a remote object through its interface, using the config file */

        private class RemoteNew
        {
            private static Hashtable types;

            private static void InitTypeTable()
            {
                types = new Hashtable();
                foreach (var entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                    types.Add(entry.ObjectType, entry);
            }

            public static object New(Type type)
            {
                if (types == null)
                    InitTypeTable();
                var entry = (WellKnownClientTypeEntry) types[type];
                if (entry == null)
                    throw new RemotingException("Type not found!");
                return RemotingServices.Connect(type, entry.ObjectUrl);
            }
        }
    }
}