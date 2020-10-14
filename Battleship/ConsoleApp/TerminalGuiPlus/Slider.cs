using System.Linq;
using System.Text;
using ConsoleApp.GameMenu;
using Terminal.Gui;
using static ConsoleApp.GameMenu.ColorSchemeHolder;

namespace ConsoleApp.TerminalGuiPlus
{
    public static class Slider
    {
        public class BarHelper
        {
            public readonly Point Position;
            public readonly int Width;

            public BarHelper(int x, int y, int width)
            {
                Position = new Point(x, y);
                Width = width;
            }
        }
        public class DescriptionHelper
        {
            public readonly Point Position;
            public readonly string Text ;

            public DescriptionHelper(int x, int y, string text)
            {
                Position = new Point(x, y);
                Text = text;
            }
        }
        public class IndicatorHelper
        {
            public readonly int Width;
            public readonly int BaseValue;
            public readonly int Step;
            
            public IndicatorHelper(int width = 2, int baseValue = 0, int step = 1)
            {
                Width = width;
                BaseValue = baseValue;
                Step = step;
            }
        }
        
        public struct Result
        {
            public Label Description;
            public MyButton Right;
            public Label Middle;
            public MyButton Left;
            public Label Indicator;

            public View[] AsList()
            {
                return new View[] {Description, Left, Middle, Right, Indicator};
            }
        }

        private const char Empty = '.';
        private const char Filled = '|';

        public static Result CreateSlider(DescriptionHelper descriptionHelper, BarHelper barHelper, IndicatorHelper indicatorHelper)
        {
            indicatorHelper ??= new IndicatorHelper();
            int x = barHelper.Position.X;
            int y = barHelper.Position.Y;

            int xEnd = barHelper.Width + Global.ButtonTextLength + x;
            int progress = barHelper.Width / 2;
            var middleString = 
                string.Concat(Enumerable.Repeat(Filled, progress)) + 
                string.Concat(Enumerable.Repeat(Empty, barHelper.Width - progress));
            var progressValue = GetProgressValue(
                middleString.ToCharArray(), 
                indicatorHelper.BaseValue,
                indicatorHelper.Step
                );
            var indicatorText = LastDigitsWithPadding(
                progressValue, 
                indicatorHelper.Width
                );

            Label description = new Label(descriptionHelper.Text)
            {
                X = descriptionHelper.Position.X,
                Y = descriptionHelper.Position.Y,
            };
            
            Label indicator = new Label(indicatorText)
            {
                X = xEnd + Global.ButtonTextLength + 1,
                Y = y,
            };
            
            Label middle = new Label(middleString)
            {
                X = x + Global.ButtonTextLength,
                Y = y,
            };
            
            MyButton left = new MyButton("<")
            {
                X = x,
                Y = y,
                Clicked = () => Update(false, middle, indicator, indicatorHelper),
                ColorScheme = GetActiveInteractableScheme(),
                HotKey = Key.F1
            };

            MyButton right = new MyButton(">")
            {
                X = xEnd,
                Y = y,
                Clicked = () => Update(true, middle, indicator, indicatorHelper),
                ColorScheme = GetActiveInteractableScheme(),
                HotKey = Key.InsertChar
            };
            


            return new Result()
            {
                Description = description,
                Left = left,
                Middle = middle,
                Right = right,
                Indicator = indicator,
            };
        }
        
        private static int GetProgressValue(char[] middleString, int baseValue, int step)
        {
            return baseValue + GetProgressCount(middleString) * step;
        }

        private static int GetProgressCount(char[] middleString)
        {
            return middleString.Count(c => c == Filled);
        }
        private static void Update(bool increase, Label middle, Label indicator, IndicatorHelper indicatorHelper)
        {
            var middleString = middle.Text.ToString()!.ToCharArray();
            int filledCount = GetProgressCount(middleString);
            if (increase && filledCount >= middleString.Length
                || !increase && filledCount <= 0) 
            {
                return;
            }
            int changeIndex = increase ? filledCount : filledCount - 1;
            char symbol = increase ? Filled : Empty;

            middleString[changeIndex] = symbol;
            var progressValue = GetProgressValue(middleString, indicatorHelper.BaseValue, indicatorHelper.Step);
            indicator.Text = LastDigitsWithPadding(progressValue, indicatorHelper.Width);
            middle.Text = new string(middleString);
        }

        private static string LastDigitsWithPadding(int number, int numberOfDigits)
        {
            var numberString = number.ToString();
            StringBuilder result = new StringBuilder();
            for (int i = numberOfDigits; i > 0; i--)
            {
                if (numberString.Length >= i)
                {
                    foreach (var numberChar in numberString)
                    {
                        result.Append(numberChar);
                    }
                    break;
                }
                result.Append(' ');
            }

            return result.ToString();
        } 
    }
}