using System;

namespace Common
{
    [Serializable]
    public class Diginote
    {
        private static int Serial;

        public Diginote()
        {
            Serial += 1;
            Id = Serial;
            Quote = 1.0f;
        }

        public Diginote(int id)
        {
            Id = id;
            Quote = 1.0f;
        }

        public int Id { get; set; }
        public float Quote { get; set; }

        public override string ToString()
        {
            return "Diginote's" + " " + "#" + Id + " " + "quote: " + Quote + "€";
        }
    }
}