using System.Diagnostics.CodeAnalysis;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace ConsoleApp.GameMenu
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public static class ColorSchemeHolder
    {
        private static Color primaryTextCol = Color.DarkGray;
        private static Color primaryWinBgCol = Color.Blue;
        private static Color unusedCol = Color.Red;
        private static Color primaryTitleCol = Color.Gray;

        public struct ColorSchemeResult
        {
            public readonly ColorScheme ColorScheme;
            public readonly string Name;

            public ColorSchemeResult(ColorScheme colorScheme, string name)
            {
                ColorScheme = colorScheme;
                Name = name;
            }
        }
        
        public static class TopBarCS
        {
            internal static readonly ColorScheme TopBar1 = new ColorScheme()
            {
                Focus = Attribute.Make(primaryTextCol, Color.Brown),
                Normal = Attribute.Make(primaryTextCol, Color.Gray),    // Cyan is Green
                Disabled = Attribute.Make(Color.Gray, Color.DarkGray),
            };

            public static ColorSchemeResult[] GetAll()
            {
                return new[]
                {
                    new ColorSchemeResult(TopBar1, "TopBar 1")
                };
            }
        }
        
        public static class WindowCS
        {
            internal static readonly ColorScheme Window1 = new ColorScheme()
            {
                Focus = Attribute.Make(unusedCol, unusedCol),
                Normal = Attribute.Make(Color.Gray, primaryWinBgCol),
                HotFocus = Attribute.Make(unusedCol, unusedCol),
                HotNormal = Attribute.Make(primaryTitleCol, primaryWinBgCol),
            };
            // Focus = selected,
            // Normal = not selected,
            // HotFocus = first char (cursor pos) changes between fg and bg,
            // HotNormal = first char color is defined by fg color, bg changes title bg

            public static ColorSchemeResult[] GetAll()
            {
                return new[]
                {
                    new ColorSchemeResult(Window1, "Window 1"),
                };
            }
        }
        
        public static class InteractableCS
        {
            internal static readonly ColorScheme Interactable1 = new ColorScheme()
            {
                Focus = Attribute.Make(Color.Brown, primaryTextCol),
                Normal = Attribute.Make(primaryTextCol, Color.Gray),
                HotFocus = Attribute.Make(Color.Brown, primaryTextCol),
                HotNormal = Attribute.Make(primaryTextCol, Color.Gray),
            };

            private static readonly ColorScheme Interactable2 = new ColorScheme()
            {
                Focus = Attribute.Make(Color.Magenta, Color.Blue),
                Normal = Attribute.Make(Color.BrightRed, Color.Magenta),
                HotFocus = Attribute.Make(Color.Magenta, Color.Blue),
                HotNormal = Attribute.Make(Color.BrightRed, Color.Magenta),
            };

            public static ColorSchemeResult[] GetAll()
            {
                return new[]
                {
                    new ColorSchemeResult(Interactable1, "Interactable 1"),
                    new ColorSchemeResult(Interactable2, "Interactable 2"),
                };
            }
        }
        private static ColorScheme _activeInteractableScheme = InteractableCS.Interactable1;
        private static ColorScheme _activeWindowScheme = WindowCS.Window1;
        private static ColorScheme _activeTopScheme = TopBarCS.TopBar1;

        public static ColorScheme GetActiveTopScheme() => _activeTopScheme;
        public static ColorScheme GetActiveInteractableScheme() => _activeInteractableScheme;
        public static void SetActiveInteractableScheme(ColorScheme cs) => _activeInteractableScheme = cs;
        public static ColorScheme GetActiveWindowScheme() => _activeWindowScheme;
        public static void SetActiveWindowScheme(ColorScheme cs) => _activeWindowScheme = cs;
        
        
        /*
         
        public enum ECSType
        {
            Window,
            Interactable,
        }
        public enum ECS
        {
            Window1,
            Window2,
            Interactable1,
            Interactable2,
        }
         
        public static string EnumValue(this ECS e) {
            switch (e) {
                case ECS.Interactable1:
                    return "Interactable 1";
                case ECS.Interactable2:
                    return "Interactable 2";
                case ECS.Window1:
                    return "Window 1";
                case ECS.Window2:
                    return "Window 2";
                default:
                    throw new Exception("ColorSchemes Enum is bad!!");
            }
        }
        
        public static ColorScheme GetColor(ECSType color)
        {
            switch (color)
            {
                case ECSType.Window:
                    return ActiveWindowScheme;
                case ECSType.Interactable:
                    return ActiveInteractableScheme;
                default:
                    throw new Exception("Enum is bad!");
            }
        }
        
        public static void SetColor(ECSType type, ColorScheme color)
        {
            switch (type)
            {
                case ECSType.Window:
                    ActiveWindowScheme = color;
                    break;
                case ECSType.Interactable:
                    ActiveInteractableScheme = color;
                    break;
                default:
                    throw new Exception("Enum is bad!");
            }
        }
        */
    }
}