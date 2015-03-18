using System;
using System.Collections;
using System.Runtime.Remoting;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

class ClientApp {
    static void Main(string[] args)
    {
        new ClientConsole();
    }

    class ClientConsole
    {
        IListSingleton listServer;
        ArrayList users;

        public ClientConsole()
        {
            RemotingConfiguration.Configure("Client.exe.config", false);
            listServer = (IListSingleton)RemoteNew.New(typeof(IListSingleton));
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
                Console.WriteLine("4 - Users");
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
                        listUsers();
                        break;
                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
            while(option != "0");
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
            bool add = listServer.AddUser(name, nickname, password);
            if (add)
            {
                Console.WriteLine("User added successfully!");
            }
            else
            {
                Console.WriteLine("Error adding user: Nickname already exists!");
            }
        }

        public void removeUser()
        {
            Console.WriteLine("Remove User");

            Console.Write("Nickname: ");
            string nickname = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();
            bool add = listServer.RemoveUser(nickname, password);
            if (add)
            {
                Console.WriteLine("User removed successfully!");
            }
            else
            {
                Console.WriteLine("Error removing user: Wrong nickname/password!");
            }
        }

        public void login()
        {
            Console.WriteLine("Login");
            
            Console.Write("Nickname: ");
            string nickname = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();
            bool login = listServer.Login(nickname, password);
            if (login)
            {
                Console.WriteLine("Login successful!");
            }
            else
            {
                Console.WriteLine("Login error!");
            }
        }

        public void listUsers()
        {
            Console.WriteLine("Users");
            
            users = listServer.GetUsersList();

            if(users.Count == 0)
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
                    userIndex += 1;
                }
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
