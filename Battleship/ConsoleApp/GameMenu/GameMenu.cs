using NStack;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ConsoleApp.TerminalGuiPlus;
using Terminal.Gui;
using static ConsoleApp.GameMenu.ColorSchemeHolder;

namespace ConsoleApp.GameMenu
{
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
	[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
	internal class Menu
	{
		private static Window? _window;
		private static bool _reloadMenuBar = true;
		private static MenuBar? _menuBarToLoad;
		private static bool startGame;
		private static RuleSet? ruleSetHolder;
		
		public delegate View[] MenuAction();
		private static List<MenuAction> MenuViewLevel = new List<MenuAction>() { };
		
		
		// for example 'FILE' is the MenuBarItem and 'Go back', 'Go main' and 'Quit' are the MenuItems
		// 
		// 	+---------------------------------------------------------+
		//  |top| FILE | EDIT | top top top top top top top top top   |
		//  +--+---------+--------------------------------------------+
		//  +--| Go back |--------window title------------------------+
		//  |  | Go main |                                            |
		//  |  | Quit    |                                            |
		//  |  +---------+        the window - the  actual app        |
		// 	|                                                         |
		// 	|                                                         |
		// 	|                                                         |
		// 	+---------------------------------------------------------+


		public static MenuResult Start()
		{
			return Start(new List<MenuAction>() {getMenuMain()});
		}
		
		public static MenuResult Start(List<MenuAction> menuActions)
		{
			MenuViewLevel = menuActions;
			MenuBar? menuBar = MenubarHolder.MenuBar; // all these menubar assignments allow app to stop and reload with new menubar
			do {
				Application.Init ();
				Colors.ColorSchemes["Menu"] = GetActiveTopScheme(); // This is for app top and the MenuItems (MenuBarItem children)
				menuBar.ColorScheme = GetActiveTopScheme();		   // This is for the MenuBarItem. Does not like curly bracket init. App needs to be init first.
				_window = new Window() { Title = "BattleShip", X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Percent (80)};
				
				Application.Top.Add(_window);
                Application.Top.Add(menuBar);

                LoadMenu();
                
                //Application.Driver.PrepareToRun();
                var runToken = Application.Begin (Application.Top);
                Application.RunLoop (runToken);
                Application.End (runToken);
                
                // Application.Run();

                menuBar = _menuBarToLoad;
                _menuBarToLoad = null;
                Application.Shutdown();
			} while (menuBar != null);

			if (startGame)
			{
				return ruleSetHolder ?? throw new Exception("value was null");
			}

			return new RuleSet() {ExitCode = "ExitMenu"};
		}
		
		private static void LoadMenu()
		{
			if (_window == null) { throw new Exception("value was null"); }
			_window.RemoveAll();
			View[] views = MenuViewLevel.Last().Invoke();
			_window.Add (views);
		}

		private static View[] MenuMain()
		{
			var button1 = new MyButton("New Game")
			{
				X = 1,
				Y = 0,
				Clicked = () => AddAndExec(MenuStart),
				ColorScheme = GetActiveInteractableScheme()
			};
			var button2 = new MyButton("Load game")
			{
				X = Pos.Left(button1),
				Y = Pos.Top(button1) + 1,
				Clicked = () => JumpToMainAndExec(),
				ColorScheme = GetActiveInteractableScheme()
			};
			var button3 = new MyButton("Options")
			{
				X = Pos.Left(button2),
				Y = Pos.Top(button2) + 1,
				Clicked = () => AddAndExec(OptionsMenu),
				ColorScheme = GetActiveInteractableScheme()
			};
			var button4 = new MyButton("Credits and About")
			{
				X = Pos.Left(button3),
				Y = Pos.Top(button3) + 1,
				Clicked = () => AddAndExec(MenuAbout),
				ColorScheme = GetActiveInteractableScheme()
			};
			var button5 = new MyButton("Quit")
			{
				X = Pos.Left(button4),
				Y = Pos.Top(button4) + 1,
				Clicked = () => Application.RequestStop(),
				ColorScheme = GetActiveInteractableScheme()
			};

			Label label1 = new Label("1.0.2b")
			{
				X = 1,
				Y = Pos.Top(button5) + 2,
			};
			
			var result = new List<View>() { button1, button2, button3, button4, button5, label1 };
			return result.ToArray();
		}
		
		private static View[] MenuAbout()
		{
			// variable names are only needed for position reference
			// https://www.random.org/strings/?num=50&len=8&upperalpha=on&loweralpha=on&unique=on&format=html&rnd=new
			var hLYKsHBT = new Label("Credits and About") {X = 1, Y = 0};
			var SxuUMJkC = new Label(string.Concat(Enumerable.Repeat("=", 24))) {X = 1, Y = hLYKsHBT.Y + 1};
			var HtAEFVmk = new Label("About:") {X = 1, Y = SxuUMJkC.Y + 1};
			var TQuCEEMe = new Label("    This project is done for ICD0008 C# course the purpose of learning C#, databases and Razor Pages.") {X = 1, Y = HtAEFVmk.Y + 1,};

			var creVBBQx = new Label("Credits:") {X = 1, Y = TQuCEEMe.Y + 2,};
			var xDIXlmqU = new Label("    Andres Käver - Instructor + Code") {X = 1, Y = creVBBQx.Y + 1,};
			var kreBJOCQ = new Label("    Karl Oskar Anderson - Creator + Design + Code") {X = 1, Y = xDIXlmqU.Y + 1,};

			var YIzFuiDQ = new Label("Made with:") {X = 1, Y = kreBJOCQ.Y + 2,};
			var rHLCLGFd = new Label("    Terminal Gui/gui.cs") {X = 1, Y = YIzFuiDQ.Y + 1,};
			
			var OGAnTyYO = new Label("Source code - https://gitlab.cs.ttu.ee/kaande/icd0008-2020f") {X = 1, Y = rHLCLGFd.Y + 2,};

			var ITVidmDZ = new MyButton("Back") { X = 1, Y = OGAnTyYO.Y + 2, Clicked = () => PopAndExec(), ColorScheme = GetActiveInteractableScheme()};

			var result = new List<View>() {
				hLYKsHBT,
				SxuUMJkC,
				HtAEFVmk,
				TQuCEEMe,
				creVBBQx,
				xDIXlmqU,
				kreBJOCQ,
				YIzFuiDQ,
				rHLCLGFd,
				OGAnTyYO,
				ITVidmDZ
			};
			return result.ToArray();
		}

		private static View[] MenuStart()
		{
			
			var d5 = new MyButton("Load ruleset") { X = 1, Y = 1, Clicked = () => NotImplementedYet(), ColorScheme = GetActiveInteractableScheme()};
			var d6 = new Label("Ruleset:") { X = 1, Y = d5.Y + 2,};
			
			var slider1 = Slider.CreateSlider(
				new Slider.DescriptionHelper(1, 4, "Board Width"),
				new Slider.BarHelper(15, 4, 15),
				new Slider.IndicatorHelper(2, 10, 2)
			);
			var slider2 = Slider.CreateSlider(
				new Slider.DescriptionHelper(1, 5, "Board Height"),
				new Slider.BarHelper(15, 5, 15),
				new Slider.IndicatorHelper(2, 10, 2)
			);

			CheckBox cb1 = new CheckBox("Adjacent ship placement")
			{
				X = 1,
				Y = Pos.Top(slider2.Left) + 1,
				ColorScheme = GetActiveInteractableScheme(),
				Checked = false
			};
			
			var d4 = new MyButton("Start") { 
				X = 1, 
				Y = 0, 
				Clicked = () =>
				{
					if (! Int32.TryParse(slider1.Indicator.Text.ToString(), out var w)) { throw new Exception("parse failed");};
					if (! Int32.TryParse(slider2.Indicator.Text.ToString(), out var h)) { throw new Exception("parse failed");};
					bool allowAdjacent = cb1.Checked;
					ruleSetHolder = new RuleSet()
					{
						BoardWidth = w,
						BoardHeight = h,
						AdjacentTilesAllowed = allowAdjacent,
						ExitCode = "StartGame"
					};
					startGame = true;
					Application.RequestStop();
				}, 
				ColorScheme = GetActiveInteractableScheme()
			};
			
			var ITVidmDZ = new MyButton("Save ruleset") { X = 1, Y = cb1.Y + 1, Clicked = () => NotImplementedYet(), ColorScheme = GetActiveInteractableScheme()};
			
			var back = new MyButton("Back") { X = 1, Y = ITVidmDZ.Y + 2, Clicked = () => PopAndExec(), ColorScheme = GetActiveInteractableScheme() };

			
			var mousePos = new Label("Mouse: ") { X = 1, Y = Pos.AnchorEnd(1), Width = 60,};
			Application.RootMouseEvent += delegate (MouseEvent me) {
				mousePos.Text = $"Mouse: ({me.X},{me.Y}) - {me.Flags}";
			};
			// opponent selection

			var result = new List<View>()
				.Append(d4)
				.Append(d5)
				.Append(d6)
				.Concat(slider1.AsList())
				.Concat(slider2.AsList())
				.Append(cb1)
				.Append(ITVidmDZ)
				.Append(back)
				.Append(mousePos)
				.ToArray();
			return result;
		}

		private static View[] OptionsMenu()
		{
			var ntSrASFe = new Label("Change Color Scheme") {X = 1, Y = 0};
			var SxuUMJkC = new Label(string.Concat(Enumerable.Repeat("=", 24))) {X = 1, Y = ntSrASFe.Y + 1};
			var hLYKsHBT = new Label("Colors are interpreted differently by terminals, adjust the colors here or in terminal setting RGB values") {X = 1, Y = SxuUMJkC.Y + 1};
			var HtAEFVmk = new Label("Window Color") {X = 1, Y = hLYKsHBT.Y + 1};
			var windowColors = ColorSchemeHolder.WindowCS.GetAll();
			RadioGroup windowsRadio = new RadioGroup()
			{
				X = 1, 
				Y = HtAEFVmk.Y + 1, 
				RadioLabels = windowColors.Select(e => ustring.Make(e.Name)).ToArray(), 
				SelectedItem = windowColors.Select(e => e.ColorScheme).ToList().IndexOf(GetActiveInteractableScheme()),
				SelectedItemChanged = (selectedItemChangedArgs) =>
				{
					var newColor = windowColors[selectedItemChangedArgs.SelectedItem].ColorScheme;
					if (_window == null) { throw new Exception("value was null"); }
					_window.ColorScheme = newColor;
					SetActiveWindowScheme(newColor);
				},
				ColorScheme = GetActiveInteractableScheme()
			};
			
			var TQuCEEMe = new Label("Interactable Color") {X = 1, Y = HtAEFVmk.Y + windowColors.Length + 2};
			var NaGdSesG = new Label("There is a weird bug with keyboard navigation skipping buttons sometimes") {X = 40, Y = TQuCEEMe.Y + 1};
			var interactableColors = ColorSchemeHolder.InteractableCS.GetAll();
			RadioGroup interactableRadio = new RadioGroup()
			{
				X = 1, 
				Y = HtAEFVmk.Y + windowColors.Length + 3, 
				RadioLabels = interactableColors.Select(e => ustring.Make(e.Name)).ToArray(), 
				SelectedItem = interactableColors.Select(e => e.ColorScheme).ToList().IndexOf(GetActiveInteractableScheme()),
				SelectedItemChanged = (selectedItemChangedArgs) =>
				{
					var newColor = interactableColors[selectedItemChangedArgs.SelectedItem].ColorScheme;
					SetActiveInteractableScheme(newColor);
					LoadMenu();
				},
				ColorScheme = GetActiveInteractableScheme()
			};
			
			var ITVidmDZ = new MyButton("Back") { X = 1, Y = Pos.Bottom(interactableRadio) + 1, Clicked = () => PopAndExec(), ColorScheme = GetActiveInteractableScheme()};

			var result = new List<View>(){
				ntSrASFe,
				hLYKsHBT,
				SxuUMJkC,
				HtAEFVmk,
				windowsRadio,
				TQuCEEMe,
				NaGdSesG,
				interactableRadio,
				ITVidmDZ,
			};
			return result.ToArray();
		}
			
		private static void AddAndExec(MenuAction menuAction)
		{
			MenuViewLevel.Add(menuAction);
			LoadMenu();
		}

		internal static void PopAndExec()
		{
			MenuViewLevel.Remove(MenuViewLevel.Last());
			LoadMenu();
		}

		private static void NotImplementedYet()
		{
			
		}
		
		internal static void JumpToMainAndExec()
		{
			MenuViewLevel.RemoveRange(1, MenuViewLevel.Count - 1);
			LoadMenu();
		}

		internal static int GetMenuLevel()
		{
			return MenuViewLevel.Count;
		}


		public static MenuAction getMenuMain()
		{
			return MenuMain;
		}
		
		public static MenuAction getMenuStart()
		{
			return MenuStart;
		}
	}
}