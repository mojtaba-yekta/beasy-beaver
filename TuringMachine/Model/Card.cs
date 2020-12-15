using System;
using System.Collections.Generic;
using System.Text;

namespace TuringMachine
{
    public class Card
    {
        public string CardName { get; set; }
        public int Print0 { get; set; }
        public Shift Shift0 { get; set; }
        public string GotoCard0 { get; set; }
        public int Print1 { get; set; }
        public Shift Shift1 { get; set; }
        public string GotoCard1 { get; set; }

    }
}
