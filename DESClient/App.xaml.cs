using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Windows;
using Common;

namespace DESClient
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IDES IDes;

        private App()
        {
            RemotingConfiguration.Configure("DESClient.exe.config", false);
            IDes = (IDES) RemoteNew.New(typeof (IDES));
        }

        private class RemoteNew
        {
            private static Hashtable _types;

            private static void InitTypeTable()
            {
                _types = new Hashtable();
                foreach (var entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                    _types.Add(entry.ObjectType, entry);
            }

            public static object New(Type type)
            {
                if (_types == null)
                    InitTypeTable();
                var entry = (WellKnownClientTypeEntry) _types[type];
                if (entry == null)
                    throw new RemotingException("Type not found!");
                return RemotingServices.Connect(type, entry.ObjectUrl);
            }
        }
    }
}