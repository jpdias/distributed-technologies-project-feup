using System;

namespace Common
{
    [Serializable]
    public class SaleOrder : Order
    {
        public SaleOrder(int quantity, float value)
        {
            Serial += 1;
            Id = Serial;
            Quantity = quantity;
            Value = value;
        }

        public SaleOrder(int id, int quantity, float value, bool processed)
        {
            Id = id;
            Quantity = quantity;
            Value = value;
            Processed = processed;
        }
    }
}