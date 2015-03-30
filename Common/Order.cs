using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    abstract public class Order
    {
        public int Quantity { get; set; }
        public bool Processed { get; set; }
    }
}
