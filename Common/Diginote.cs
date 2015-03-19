using System;

namespace Common
{
    [Serializable]
    public class Diginote
    {
        private static int Serial = 0;
        public int Id { get; set; }
        public float Quote { get; set; }

        public Diginote()
        {
            Serial += 1;
            this.Id = Serial;
            this.Quote = 1.0f;
        }
        public override string ToString()
        {
            return "Diginote's" + " " + "#" + Id + " " + "quote: " + Quote + "€";
        }
    }
}