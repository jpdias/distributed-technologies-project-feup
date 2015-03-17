using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace DESClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("DESClient.exe.config", false);
            InitializeComponent();
        }

        private static void InitializeComponent()
        {
            throw new NotImplementedException();
        }
    }
}
