using System;

namespace TuringMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            BeasyBeaver BB = new BeasyBeaver("machine.json");
            BB.Start();
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();
                if (keyinfo.Key == ConsoleKey.Spacebar)
                {
                    BB.TogglePause();
                }
            }
            while (keyinfo.Key != ConsoleKey.Escape);
            BB.Stop();
        }
    }
}
