using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain.Model;
using Game;
using RogueSharp;
using Troschuetz.Random.Generators;
using Game.Pack;
using IrrKlang;
using NUnit.Framework;

namespace Tests
{
    static class Program
    {
        static void Main(string[] args)
        {
            // RandomTimeTest();
            // loopingTest();
            // Pack(1);
            // SoundTest();
            // MultiArrayTest();
        }

        private static void RandomTimeTest()
        {
            int upLimit = 1000 * 1000;
            var stopwatch = Stopwatch.StartNew();
/*
            for (int i = 0; i < upLimit; i++)
            {
                var std = new ALFGenerator(i);
                std.NextInclusiveMaxValue();
            }
            stopwatch.Stop();
            Console.WriteLine("ALFGenerator         " + stopwatch.Elapsed);


            
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                var std = new MT19937Generator(i);
                std.NextInclusiveMaxValue();
            }
            stopwatch.Stop();
            Console.WriteLine("MT19937Generator     " + stopwatch.Elapsed);
            
           */ 
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                var std = new NR3Generator(i);
                std.NextInclusiveMaxValue();
            }
            stopwatch.Stop();
            Console.WriteLine("NR3Generator         " + stopwatch.Elapsed);
            
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                var std = new NR3Q1Generator(i);
                std.NextInclusiveMaxValue();
            }
            stopwatch.Stop();
            Console.WriteLine("NR3Q1Generator       " + stopwatch.Elapsed);
            
            
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                var std = new NR3Q2Generator(i);
                std.NextInclusiveMaxValue();
            }
            stopwatch.Stop();
            Console.WriteLine("NR3Q2Generator       " + stopwatch.Elapsed);
            
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                var std = new StandardGenerator(127);
                std.NextInclusiveMaxValue();
            }
            stopwatch.Stop();
            Console.WriteLine("StandardGenerator    " + stopwatch.Elapsed);
            
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                var std = new XorShift128Generator(i);
                std.NextInclusiveMaxValue();
            }
            stopwatch.Stop();
            Console.WriteLine("XorShift128Generator " + stopwatch.Elapsed);

            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                var std = new Random(i);
                std.Next();
            }
            stopwatch.Stop();
            Console.WriteLine("System Random        " + stopwatch.Elapsed);
        }

        private static void loopingTest()
        {
            int upLimit = 1000 * 1000;
            var stopwatch = Stopwatch.StartNew();
            
            List<string> testList = new List<string>() { String.Concat(Enumerable.Repeat("T", 1000)) };
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                foreach (var s in testList.ToArray())
                {
                    
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Loop with collection toArray  " + stopwatch.Elapsed);
            string[] testArray = new List<string>() { String.Concat(Enumerable.Repeat("T", 1000)) }.ToArray();
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                foreach (var s in testArray)
                {
                    
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Loop with no collection toArray  " + stopwatch.Elapsed);
            
            
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                foreach (var (rectangle, idx1) in testArray.Select((value, idx) => (value, idx)))
                {
                    
                }
            }
            stopwatch.Stop();
            Console.WriteLine("LINQ tuple idx and item       " + stopwatch.Elapsed);
            
            
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                for (int j = 0; j < testArray.Length; j++)
                {
                    string s = testArray[j];
                }
            }
            stopwatch.Stop();
            Console.WriteLine("for with item from idx        " + stopwatch.Elapsed);
            
            stopwatch.Restart();
            for (int i = 0; i < upLimit; i++)
            {
                foreach (var s in testArray)
                {
                }
            }
            stopwatch.Stop();
            Console.WriteLine("foreach                       " + stopwatch.Elapsed);
        }
        
        private static List<Rectangle> Pack(int placementType)
        {
            List<Point> shipsSizesToPlace = new List<Point>()
            {
                new Point(5, 1),
                new Point(4, 1),
                new Point(3, 1),
                new Point(3, 1),
                new Point(3, 1),
                new Point(2, 1),
                new Point(2, 1),
                new Point(2, 1),
                new Point(2, 1),
            };
            List<Rectangle> packedRects;
            ShipPlacement.TryPackShip(shipsSizesToPlace, 10, 10, placementType, out packedRects);
            return packedRects;
        }

        private static void SoundTest()
        {
            ISoundEngine engine = new ISoundEngine();
            engine.Play2D("../../../../../media/Test/getout.ogg", true);
            Console.Out.WriteLine("\nHello World");

            do
            {
                Console.Out.WriteLine("Press any key to play some sound, press 'q' to quit.");

                // play a single sound
                engine.Play2D("../../../../../media/Test/bell.wav");
            }
            while(_getch() != 'q');
        }
        
        // some simple function for reading keys from the console
        [System.Runtime.InteropServices.DllImport("msvcrt")]
        static extern int _getch();

        private static void MultiArrayTest()
        {
            // WTF is going on with jagged array dimensions?
            // Length is supposed to get the total number of elements in all the dimensions of the array,
            // but only gives current array element count. 
            // GetLength() throws exceptions in further dept
            int[][] testMap = new int[2][];
            testMap[0] = new[] {1, 2, 3};
            testMap[1] = new[] {1, 2, 3};
            Assert.AreEqual(2, testMap.Length);
            Assert.AreEqual(3, testMap[0].Length);
            Assert.AreEqual(2,testMap.GetLength(0));
            Assert.Throws<IndexOutOfRangeException>(() => testMap.GetLength(1));
        }

        #region Rider test
        private static Action Test { get; set; } = null!;

        private static void SetTest(Action? action = null)
        {
            // Rider IDE doesn't seem to highlight nullable to non-nullable assignment.
            Test = action;
        }
        #endregion
    }
}