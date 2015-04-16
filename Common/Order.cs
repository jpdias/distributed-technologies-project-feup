using System;

namespace Common
{
    [Serializable]
    public abstract class Order
    {
        protected static int Serial = 0;
        public int Id { get; set; }
        public int Quantity { get; set; }
        public float Value { get; set; }
        public bool Processed { get; set; }
    }
}