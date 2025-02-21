using System;

namespace StackVSQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            // Queue -> Navbat
            // F.I.F.O

            Queue<string> ismlar = new Queue<string>();

            ismlar.Enqueue("Zuhra");
            ismlar.Enqueue("Fotima");
            ismlar.Enqueue("Madina");

            Console.WriteLine(ismlar.Peek());

            Console.WriteLine(ismlar.Dequeue() + " o'chirildi");

            Console.WriteLine(ismlar.Count == 0);
        }
    }
}