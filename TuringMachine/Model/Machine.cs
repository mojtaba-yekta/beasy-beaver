using System;
using System.Collections.Generic;
using System.Text;

namespace TuringMachine.Model
{
    public class Machine
    {
        public string StartPoint { get; set; }
        public List<CardRaw> Cards { get; set; }
    }
}
