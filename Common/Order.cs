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
        protected static int Serial = 0; 
        public int Id { get; set; }
        public int Quantity { get; set; }
        public float Value { get; set; }
        public bool Processed { get; set; }
    }
}
