using System;

namespace Common
{
    [Serializable]
    public class BuyOrder : Order
    {
        public BuyOrder(int quantity, float value)
        {
            Serial += 1;
            Id = Serial;
            Quantity = quantity;
            Value = value;
        }

        public BuyOrder(int id, int quantity, float value, bool processed)
        {
            Id = id;
            Quantity = quantity;
            Value = value;
            Processed = processed;
        }
    }
}