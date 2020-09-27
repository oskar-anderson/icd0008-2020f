using Terminal.Gui;

namespace ConsoleApp.GameMenu
{
    public static class MenubarHolder
    {
        private static readonly MenuBarItem MenuBarItem1 = new MenuBarItem("Navigate", new[]
        {
            new MenuItem("Go Back", "", () => { Menu.PopAndExec(); }, BackCanExecute),
            new MenuItem("Go to Main", "", () => { Menu.JumpToMainAndExec(); }, MainCanExecute),
            new MenuItem("Quit", "", () => { Application.RequestStop(); }, QuitCanExecute),
        });
        
        private static readonly MenuBarItem MenuBarItem2 = new MenuBarItem("File", new[]
        {
            new MenuItem("Go Back - Test", "", () => { Menu.PopAndExec(); }, BackCanExecute),
            new MenuItem("Go to Main - Test", "", () => { Menu.JumpToMainAndExec(); }, MainCanExecute),
            new MenuItem("Quit - Test", "", () => { Application.RequestStop(); }, QuitCanExecute),
        });

        public static readonly MenuBar MenuBar = new MenuBar(
            menus: new MenuBarItem[]
            {
                MenuBarItem1,
                // MenuBarItem2
            }
        );



        private static bool BackCanExecute()
        {
            if (Global.KaverVer)
            {
                return Menu.GetMenuLevel() > 2;
            }
            return Menu.GetMenuLevel() > 1;
        }
        
        private static bool MainCanExecute()
        {
            return Menu.GetMenuLevel() > 1;
        }

        private static bool QuitCanExecute()
        {
            return Menu.GetMenuLevel() > 0;
        }
    }
}