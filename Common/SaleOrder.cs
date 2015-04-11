using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class SaleOrder : Order
    {
        public SaleOrder(int quantity, float value)
        {
            Serial += 1;
            this.Id = Serial;
            this.Quantity = quantity;
            this.Value = value;
        }
    }
}
