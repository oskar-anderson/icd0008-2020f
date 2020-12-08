using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using RogueSharp;

namespace Game
{
    public static class Utils
    {
        public static bool TryBtnStart(string shipsToParse, int w, int h, int selectedItem, ref string errorMsg, out RuleSet? ruleSet)
        {
            ruleSet = null;
            List<Point> shipList;
            if (! ShipStringParse(shipsToParse, out shipList, ref errorMsg))
            {
                return false;
            }
            if (! Pack.ShipPlacement.TryPackShip(shipList, w, h, selectedItem, out _))
            {
                errorMsg = "Will not fit!";
                return false;
            }
            ruleSet = new RuleSet()
            {
                BoardWidth = w,
                BoardHeight = h,
                AllowedPlacementType = selectedItem,
                Ships = shipsToParse,
                ExitCode = ExitResult.Start
            };
            return true;
        }

        public static bool ShipStringParse(string shipsToParse, out List<Point> result, ref string errorMsg)
        {
            errorMsg = "";
            String[] ships = shipsToParse.ToLower().Trim(';').Replace(" ", "").Split(';');
            result = new List<Point>();
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
                        case 1 when cShip == 'n':
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
                    result.Add(new Point(x, y));
                }
            }
            
            return true;
        }
    }
}