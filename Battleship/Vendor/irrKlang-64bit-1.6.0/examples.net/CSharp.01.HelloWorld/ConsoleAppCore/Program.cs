using System;
using IrrKlang;

namespace ConsoleAppCore
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ISoundEngine engine = new ISoundEngine();
            Console.WriteLine("Hello World!");
        }
    }
}