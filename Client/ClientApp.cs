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
            private ArrayList users;
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
                    Console.WriteLine("1 - Add User");
                    Console.WriteLine("2 - Remove User");
                    Console.WriteLine("3 - Login");
                    Console.WriteLine("4 - Logout");
                    Console.WriteLine("5 - Market");
                    Console.WriteLine("0 - Exit");
                    Console.Write("Option: ");
                    option = Console.ReadLine();
                    switch (option)
                    {
                        case "0":
                            Application.Exit();
                            break;
                        case "1":
                            Console.WriteLine();
                            addUser();
                            break;
                        case "2":
                            Console.WriteLine();
                            removeUser();
                            break;
                        case "3":
                            Console.WriteLine();
                            login();
                            break;
                        case "4":
                            Console.WriteLine();
                            logout();
                            break;
                        case "5":
                            Console.WriteLine();
                            listMarket();
                            break;
                        default:
                            Console.WriteLine("Invalid option!");
                            break;
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

            public void login()
            {
                Console.WriteLine("Login");

                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.Login(nickname, password);
                Console.WriteLine(result);
            }

            public void logout()
            {
                Console.WriteLine("Logout");

                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string result = _iDes.Logout(nickname, password);
                Console.WriteLine(result);
            }

            public void listUsers()
            {
                Console.WriteLine("Users");

                users = _iDes.GetUsersList();

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

            public void listMarket()
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
        }

        /* Mechanism for instanciating a remote object through its interface, using the config file */

       
    }
}
