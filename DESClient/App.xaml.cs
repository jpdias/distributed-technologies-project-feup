using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using Common;

namespace DESClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static IDES IDes;
        

        private App()
        {
            RemotingConfiguration.Configure("DESClient.exe.config", false);
            IDes = (IDES)RemoteNew.New(typeof(IDES));
            
        }

       
        class RemoteNew
        {
            private static Hashtable _types = null;

            private static void InitTypeTable()
            {
                _types = new Hashtable();
                foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                    _types.Add(entry.ObjectType, entry);
            }

            public static object New(Type type)
            {
                if (_types == null)
                    InitTypeTable();
                WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)_types[type];
                if (entry == null)
                    throw new RemotingException("Type not found!");
                return RemotingServices.Connect(type, entry.ObjectUrl);
            }
        }
    }
}
