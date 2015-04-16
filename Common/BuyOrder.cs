using System;

namespace Common
{
    [Serializable]
    public class BuyOrder : Order
    {
        public BuyOrder(int quantity, float value)
        {
            Serial += 1;
            this.Id = Serial;
            this.Quantity = quantity;
            this.Value = value;
        }

        public BuyOrder(int id, int quantity, float value, bool processed)
        {
            Serial += 1; 
            this.Id = id;
            this.Quantity = quantity;
            this.Value = value;
            this.Processed = processed;
        }
    }
}
