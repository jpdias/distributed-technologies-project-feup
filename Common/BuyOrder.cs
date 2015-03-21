using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class BuyOrder : Order
    {
        public BuyOrder(int quantity)
        {
            this.Quantity = quantity;
        }
    }
}
