using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiginoteExchangeSystem
{
    [Serializable]
    class Diginote
    {
        private static int _serialNumber = 0;
        public int Serial { get; private set; }

        private const int Value = 1;

        public Diginote()
        {
            _serialNumber++;
            Serial = _serialNumber;
        }

    }
}
