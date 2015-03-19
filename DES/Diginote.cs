using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

[Serializable]
class Diginote
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
}