using NStack;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ConsoleApp.TerminalGuiPlus;
using RogueSharp;
using Terminal.Gui;
using static ConsoleApp.GameMenu.ColorSchemeHolder;
using Point = RogueSharp.Point;

namespace ConsoleApp.GameMenu
{
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
	[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
	internal static class Menu
	{
		private static Window? _window;
		private static MenuResult ruleSetHolder = null!;

		private static bool IsContinueActive;
		public delegate View[] MenuAction();
		private static List<MenuAction> MenuViewLevel = null!;
		
		
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


		public static RuleSet Start()
		{
			return Start(new List<MenuAction>() {getMenuMain()}, false);
		}
		
		public static RuleSet Start(List<MenuAction> menuActions, bool isContinueActive)
		{
			IsContinueActive = isContinueActive;
			ruleSetHolder = new MenuResult() { ExitCode = ExitResult.Exit };
			MenuViewLevel = menuActions;
			MenuBar? menuBar = MenubarHolder.MenuBar; // all these menubar assignments allow app to stop and reload with new menubar

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

            Application.Shutdown();

            return (RuleSet) ruleSetHolder;
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
			View button2 = IsContinueActive ? 
				(View) new MyButton("Continue")
				{
					X = Pos.Left(button1),
					Y = Pos.Top(button1) + 1,
					Clicked = () =>
					{
						ruleSetHolder = new RuleSet() { ExitCode = ExitResult.Continue };
						Application.RequestStop();
					},
					ColorScheme = GetActiveInteractableScheme()
				}
				: 
				new Label("Continue")
				{
					X = Pos.Left(button1),
					Y = Pos.Top(button1) + 1,
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
				Clicked = () =>
				{
					ruleSetHolder = new RuleSet() { ExitCode = ExitResult.Exit };
					Application.RequestStop();
				},
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
			
			var OGAnTyYO = new Label("More info - https://gitlab.cs.ttu.ee/kaande/icd0008-2020f") {X = 1, Y = rHLCLGFd.Y + 2,};

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
			
			var d6 = new Label("Ruleset:") { X = 1, Y = 2,};
			
			var slider1 = Slider.CreateSlider(
				new Slider.DescriptionHelper(3, 4, "Board Width"),
				new Slider.BarHelper(20, 4, 15, 0),
				new Slider.IndicatorHelper(2, 10, 2)
			);
			var slider2 = Slider.CreateSlider(
				new Slider.DescriptionHelper(3, 5, "Board Height"),
				new Slider.BarHelper(20, 5, 15, 0),
				new Slider.IndicatorHelper(2, 10, 2)
			);

			var radioDescription = new Label("Ships touching") { X = 3, Y = Pos.Top(slider2.Left) + 1 };
			var radioGroup = new RadioGroup ( new ustring[] {"No overlap", "No sides", "No Corners"} ) {
				X = 20,
				Y = Pos.Top(radioDescription),
				ColorScheme = GetActiveInteractableScheme(),
				SelectedItem = 2,
			};

			var tf1Description = new Label("Ship sizes") {X = 3, Y = Pos.Top(radioGroup) + radioGroup.RadioLabels.Length };
			TextField tf1 = new TextField("1x5:1; 1x4:2; 1x3:3; 1x2:4")
			{
				X = 20,
				Y = Pos.Top(radioGroup) + radioGroup.RadioLabels.Length,
				ColorScheme = GetActiveInteractableScheme()
			};

			var errorMsg = new Label("                     ") { X = 10, Y = 0 };
			
			var d4 = new MyButton("Start") { 
				X = 1, 
				Y = 0, 
				Clicked = () =>
				{
					if (! Int32.TryParse(slider1.Indicator.Text.ToString(), out var w)) { throw new Exception("parse failed");}
					if (! Int32.TryParse(slider2.Indicator.Text.ToString(), out var h)) { throw new Exception("parse failed");}
					string errorMsgText = errorMsg.Text.ToString()!;
					string shipsToParse = tf1.Text.ToString()!;

					if (TryBtnStart(shipsToParse, w, h, radioGroup.SelectedItem, ref errorMsgText))
					{
						Application.RequestStop();
					}
					else
					{
						errorMsg.Text = errorMsgText;
						errorMsg.Redraw(errorMsg.Bounds);
					}
					return;
				}, 
				ColorScheme = GetActiveInteractableScheme()
			};

			var back = new MyButton("Back") { X = 1, Y = tf1.Y + 1, Clicked = () => PopAndExec(), ColorScheme = GetActiveInteractableScheme() };

			
			var mousePos = new Label("Mouse: ") { X = 1, Y = Pos.AnchorEnd(1), Width = 60,};
			Application.RootMouseEvent += delegate (MouseEvent me) {
				mousePos.Text = $"Mouse: ({me.X},{me.Y}) - {me.Flags}";
			};
			// opponent selection

			var result = new List<View>()
				.Append(d4)
				.Append(errorMsg)
				.Append(d6)
				.Concat(slider1.AsList())
				.Concat(slider2.AsList())
				.Append(radioDescription)
				.Append(radioGroup)
				.Append(tf1Description)
				.Append(tf1)
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
			var windowColors = WindowCS.GetAll();
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
			var interactableColors = InteractableCS.GetAll();
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

		private static bool TryBtnStart(string shipsToParse, int w, int h, int selectedItem, ref string errorMsg)
		{
			String[] ships = shipsToParse.ToLower().Trim(';').Replace(" ", "").Split(';');
			List<Point> shipList = new List<Point>();
			foreach (var ship in ships)
			{
				StringBuilder sb1 = new StringBuilder();
				StringBuilder sb2 = new StringBuilder();
				StringBuilder sb3 = new StringBuilder();
				int track = 0;
				foreach (var cShip in ship)
				{
					switch (track)
					{
						case 0 when "012345679".Contains(cShip):
							sb1.Append(cShip);
							break;
						case 0 when cShip == 'x':
							track = 1;
							break;
						case 1 when "012345679".Contains(cShip):
							sb2.Append(cShip);
							break;
						case 1 when cShip == ':':
							track = 2;
							break;
						case 2 when "012345679".Contains(cShip):
							sb3.Append(cShip);
							break;
						default:
							errorMsg = "Parsing failed!";
							return false;
					}
				}

				int x;
				int y;
				int times;
				try
				{
					x = Math.Abs(int.Parse(sb1.ToString()));
					y = Math.Abs(int.Parse(sb2.ToString()));
					times = Math.Abs(int.Parse(sb3.ToString()));
				}
				catch (Exception)
				{
					errorMsg = "Parsing failed!";
					return false;
				}
				if (x == 0 || y == 0)
				{
					errorMsg = "Ship with size (0, 0)";
					return false;
				} 
				
				for (int i = 0; i < times; i++)
				{
					shipList.Add(new Point(x, y));
				}

				if (! Game.Pack.ShipPlacement.TryPackShip(shipList, w, h, selectedItem, out _))
				{
					errorMsg = "Will not fit!";
					return false;
				}
			}
			ruleSetHolder = new RuleSet()
			{
				BoardWidth = w,
				BoardHeight = h,
				AllowedPlacementType = selectedItem,
				Ships = shipList,
				ExitCode = ExitResult.Start
			};
			return true;
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