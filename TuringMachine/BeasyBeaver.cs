using System;
using System.Collections.Generic;
using System.Linq;
using TuringMachine.Model;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace TuringMachine
{
    public class BeasyBeaver
    {
        List<Card> cards;
        int step;
        Card currentCard;
        int headPosition;
        bool pause;
        bool stop;
        List<int> tape;

        public BeasyBeaver(string machineAddress)
        {
            MapCards(machineAddress);
            tape = new List<int> { 0 };
            step = 0;
            headPosition = 0;
            pause = true;
            stop = false;
            if (cards == null || cards.Count == 0)
                return;
        }

        public void Start()
        {
            new Task(() =>
            {
                pause = false;
                stop = false;
                while (!stop)
                {
                    NextStep();
                }
            }).Start();
        }

        public void Stop()
        {
            stop = true;
            pause = true;
        }

        public void TogglePause()
        {
            pause = !pause;
        }

        void MapCards(string machineAddress)
        {
            Machine settings = JsonConvert.DeserializeObject<Machine>(File.ReadAllText(machineAddress));
            var startCard = settings.Cards.Where(x => x.Name == settings.StartPoint).FirstOrDefault();

            if (startCard == null)
                throw new InvalidDataException("Invalid Startpoint");
            if (startCard.State0.Length != 3)
                throw new InvalidDataException("Invalid State0");
            if (startCard.State1.Length != 3)
                throw new InvalidDataException("Invalid State1");

            cards = new List<Card>();

            cards.Add(new Card
            {
                CardName = startCard.Name,
                GotoCard0 = startCard.State0[0].ToString(),
                Print0 = startCard.State0[1] == '1' ? 1 : 0,
                Shift0 = startCard.State0[2] == 'L' ? Shift.Left : Shift.Right,
                GotoCard1 = startCard.State1[0].ToString(),
                Print1 = startCard.State1[1] == '1' ? 1 : 0,
                Shift1 = startCard.State1[2] == 'L' ? Shift.Left : Shift.Right
            });

            foreach (var item in settings.Cards.Where(x => x.Name != settings.StartPoint))
            {
                if (startCard.State0.Length != 3)
                    throw new InvalidDataException($"Invalid State0 for card {item.Name}");
                if (startCard.State1.Length != 3)
                    throw new InvalidDataException($"Invalid State1 for card {item.Name}");

                cards.Add(new Card
                {
                    CardName = item.Name,
                    GotoCard0 = item.State0[0].ToString(),
                    Print0 = item.State0[1] == '1' ? 1 : 0,
                    Shift0 = item.State0[2] == 'L' ? Shift.Left : Shift.Right,
                    GotoCard1 = item.State1[0].ToString(),
                    Print1 = item.State1[1] == '1' ? 1 : 0,
                    Shift1 = item.State1[2] == 'L' ? Shift.Left : Shift.Right
                });
            }

            InitCurrentCard(settings.StartPoint);
        }

        void InitCurrentCard(string cardName)
        {
            currentCard = cards.Where(x => x.CardName == cardName).FirstOrDefault();
        }

        void NextStep()
        {
            if (currentCard != null && !pause)
            {
                PrintStep();
                step++;

                bool outOfRange = headPosition == -1 || headPosition >= tape.Count;

                if (outOfRange || tape[headPosition] == 0)
                {
                    WriteOnTape(currentCard.Print0);
                    DoShift(currentCard.Shift0);
                    InitCurrentCard(currentCard.GotoCard0);
                }
                else if (tape[headPosition] == 1)
                {
                    WriteOnTape(currentCard.Print1);
                    DoShift(currentCard.Shift1);
                    InitCurrentCard(currentCard.GotoCard1);
                }
                else
                {
                    throw new InvalidOperationException($"Head Position = {headPosition}");
                }
            }
        }

        void WriteOnTape(int alphabet)
        {
            if (headPosition == -1)
                tape.Insert(0, alphabet);
            else if (headPosition < tape.Count)
                tape[headPosition] = alphabet;
            else
                tape.Add(alphabet);
        }

        void DoShift(Shift shift)
        {
            if (shift == Shift.Left)
                headPosition = headPosition == -1 ? -1 : headPosition - 1;
            else
                headPosition = headPosition == -1 ? 1 : headPosition + 1;
        }

        void PrintStep()
        {
            string tape = "";
            for (int i = 0; i < this.tape.Count; i++)
            {
                tape += this.tape[i].ToString();
            }
            Console.WriteLine($"Step {step} - Card {currentCard.CardName} - Tape = {tape}");
        }
    }
}
