using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleApp
{
    public static class PauseMenu
    {
        private static bool _preventEscape;
        public static PauseResult Run()
        {
            _preventEscape = true;
            int result = Pause();
            if (!Enum.IsDefined(typeof(PauseResult), result))
            {
                throw new Exception("unexpected");
            }
            return (PauseResult) result;
        }
        
        public enum PauseResult
        {
            Cont,
            SaveDb,
            SaveJson,
            LoadDb,
            LoadJson,
            MainMenu,
            Quit
        }

        private static int Pause()
        {
            int selectedOption = 0;
            String[] options = {"Continue", "Save to DB", "Save to JSON", "Load from DB", "Load from json", "Main Menu", "Quit"};
            while (true)
            {
                Console.Clear();
                for (var i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    if (selectedOption == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.WriteLine(option);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                bool isEnter = GetInput(ref selectedOption,options.Length - 1);
                if (isEnter)
                {
                    return selectedOption;
                }
            }
        }

        private static bool GetInput(ref int selectedOption, int max)
        {
            List<ConsoleKey> down = new List<ConsoleKey> { ConsoleKey.D, ConsoleKey.S, ConsoleKey.DownArrow, ConsoleKey.RightArrow };
            List<ConsoleKey> up = new List<ConsoleKey> { ConsoleKey.W, ConsoleKey.A, ConsoleKey.UpArrow, ConsoleKey.LeftArrow };
            List<ConsoleKey> enter = new List<ConsoleKey> { ConsoleKey.Enter, ConsoleKey.Spacebar, ConsoleKey.Z };
            
            List<ConsoleKey> validKeys = down.Concat(up).Concat(enter).ToList();
            validKeys.Add(ConsoleKey.Escape);
            
            while (true)
            {
                // clear input buffer
                // https://stackoverflow.com/questions/3769770/clear-console-buffer
                while(Console.KeyAvailable) 
                {
                    Console.ReadKey(true);
                }
                
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape && _preventEscape)
                {
                    Thread.Sleep(400);
                    _preventEscape = false;
                    continue;
                }
                
                if (validKeys.Contains(key))
                {
                    if (down.Contains(key))
                    {
                        if (selectedOption + 1 > max)
                        {
                            selectedOption = 0;
                            return false;
                        }
                        selectedOption++;
                        return false;
                    }

                    if (up.Contains(key))
                    {
                        if (selectedOption - 1 < 0)
                        {
                            selectedOption = max;
                            return false;
                        }

                        selectedOption--;
                        return false;
                    }

                    if (enter.Contains(key))
                    {
                        return true;
                    }

                    if (key == ConsoleKey.Escape && !_preventEscape)
                    {
                        selectedOption = (int) PauseResult.Quit;
                        return true;
                    }
                }
            }
        }
    }
}