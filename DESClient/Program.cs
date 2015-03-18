using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using DiginoteExchangeSystem;

namespace DESClient
{
    class Program
    {
        static IDES users;
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("DESClient.exe.config", false);
           
            users = (IDES)RemoteNew.New(typeof(IDES));
            Boolean login = users.Login("xpto1","xpto1");
            Console.WriteLine(login);
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
