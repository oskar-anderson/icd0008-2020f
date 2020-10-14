using System;
using System.Collections.Generic;
using System.Linq;
using Point = ConsoleGameEngineCore.Point;

namespace GameEngine
{
    public class UpdateLogic
    {
       public float fOffsetX = 0.0f;
       public float fOffsetY = 0.0f;
       
       private float fStartPanX;
       private float fStartPanY;

       private static bool mouseLeftIsHeld;
       private static double fKeyboardLockedMillis;

       public static bool IsHorizontalPlacement;

       public UpdateLogic()
       {
          fStartPanX = 0.0f;
          fStartPanY = 0.0f;

          mouseLeftIsHeld = false;
          fKeyboardLockedMillis = 0.0d;
          
          IsHorizontalPlacement = true;
       }
       
       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       public bool Update(double gameTime)
       {
          if (Game.NeedToSwapTurn)
          {
             Game.SwapTurns();
             Game.NeedToSwapTurn = false;
          }

          if (Game.ConsoleEngine.GetKey(ConsoleKey.Escape))
          {
             DrawLogic.DrawBlank();
             Helper.FixConsole();
             Console.ResetColor();
             Console.Clear();
             Console.CursorVisible = true;
             return false;
          }

          if (fKeyboardLockedMillis >= 0)
          {
             fKeyboardLockedMillis -= gameTime;
          }




          bool lockKeyboard = false;
          HandlePlayerMovement(ref lockKeyboard);
          
          if (Game.ConsoleEngine.GetKey(ConsoleKey.C) && fKeyboardLockedMillis < 0)
          {
             Game.ActivePlayer.IsViewingOwnBoard = !Game.ActivePlayer.IsViewingOwnBoard;
             Game.ActivePlayer.BoardHover.Clear();
             fKeyboardLockedMillis = 0.1d;
             return true;
          }

          if (Game.ConsoleEngine.GetKey(ConsoleKey.X) && fKeyboardLockedMillis < 0)
          {
             lockKeyboard = true;
             IsHorizontalPlacement = !IsHorizontalPlacement;
          }
          
          if (Game.Phase == 1 && Game.ActivePlayer.IsViewingOwnBoard)
          {
             string ship = Game.ActivePlayer.Ships.FirstOrDefault(e => e != null);
             if (ship == null) { throw new Exception("Unexpected!"); }
             List<Point> shipPoints = new List<Point>();
             bool canBePlaced = TryGetShipPlacement(ref shipPoints, ship);
             bool arePlacedOnBoardLayer = PlaceShip(canBePlaced, shipPoints, ref lockKeyboard);
             if (arePlacedOnBoardLayer)
             {
                Game.ActivePlayer.Ships.Remove(ship);
                if (Game.ActivePlayer.Ships.All(e => e == null))
                {
                   Game.NeedToSwapTurn = true;
                   if (Game.InactivePlayer.Ships.All(e => e == null))
                   {
                      Game.InactivePlayer.IsViewingOwnBoard = !Game.ActivePlayer.IsViewingOwnBoard;   // this will be active player on next frame
                      Game.Phase = 2;
                      fKeyboardLockedMillis = 0.1d;
                      return true;
                   }
                }
             }
          }

          int selectedOppTileIndex = Game.ActivePlayer.pPlayer.Y * Game.BoardWidth + Game.ActivePlayer.pPlayer.X;
          TileValue selectedOppTile = Game.InactivePlayer.Board[selectedOppTileIndex];
          if (Game.Phase == 2 && 
              Game.ConsoleEngine.GetKey(ConsoleKey.Z) &&
              fKeyboardLockedMillis < 0 &&
              ! Tile.HitTiles.Contains(selectedOppTile)
              )
          {
             switch (selectedOppTile)
             {
                case TileValue.EmptyTileV1:
                   Game.InactivePlayer.Board[selectedOppTileIndex] = TileValue.HitWater;
                   break;
                case TileValue.Ship:
                   Game.InactivePlayer.Board[selectedOppTileIndex] = TileValue.HitShip;
                   break;
                default:
                   throw new Exception("Unexpected!");
             }

             fKeyboardLockedMillis = 0.1d;
          }

          if (lockKeyboard)
          {
             fKeyboardLockedMillis = 0.1d;
          }

          HandleZooming();
          HandleKeyboardPanning(gameTime);
          HandleMousePanning();
          
          return true;
       }
       
       
       public void WorldToScreen(float fWorldX, float fWorldY, out int nScreenX, out int nScreenY)
       {
       nScreenX = (int) Math.Floor((fWorldX - fOffsetX) * Game.ActivePlayer.fScaleX);
       nScreenY = (int) Math.Floor((fWorldY - fOffsetY) * Game.ActivePlayer.fScaleY);
       }
          
          
       public void ScreenToWorld(int nScreenX, int nScreenY, out float fWorldX, out float fWorldY)
       {
       fWorldX = (float) (nScreenX / Game.ActivePlayer.fScaleX) + fOffsetX;
       fWorldY = (float) (nScreenY / Game.ActivePlayer.fScaleY) + fOffsetY;
       }


       private void HandlePlayerMovement(ref bool lockKeyboard)
       {
          if (Game.ConsoleEngine.GetKey(ConsoleKey.A) || Game.ConsoleEngine.GetKey(ConsoleKey.LeftArrow))
          {
             if (Game.ActivePlayer.pPlayer.X > 0 && fKeyboardLockedMillis < 0)
             {
                Game.ActivePlayer.pPlayer.X -= 1;
                lockKeyboard = true;
                fOffsetX -= Tile.Width;

             }
          }

          if (Game.ConsoleEngine.GetKey(ConsoleKey.S) || Game.ConsoleEngine.GetKey(ConsoleKey.DownArrow))
          {
             if (Game.ActivePlayer.pPlayer.Y < Game.BoardHeight - 1 && fKeyboardLockedMillis < 0)
             {
                Game.ActivePlayer.pPlayer.Y += 1;
                lockKeyboard = true;
                fOffsetY += Tile.Height;
             }
          }

          if (Game.ConsoleEngine.GetKey(ConsoleKey.D) || Game.ConsoleEngine.GetKey(ConsoleKey.RightArrow))
          {
             if (Game.ActivePlayer.pPlayer.X < Game.BoardWidth - 1 && fKeyboardLockedMillis < 0)
             {
                Game.ActivePlayer.pPlayer.X += 1;
                lockKeyboard = true;
                fOffsetX += Tile.Width;
             }
          }

          if (Game.ConsoleEngine.GetKey(ConsoleKey.W) || Game.ConsoleEngine.GetKey(ConsoleKey.UpArrow))
          {
             if (Game.ActivePlayer.pPlayer.Y > 0 && fKeyboardLockedMillis < 0)
             {
                Game.ActivePlayer.pPlayer.Y -= 1;
                lockKeyboard = true;
                fOffsetY -= Tile.Height;
             }
          }
       }

       private static bool TryGetShipPlacement(ref List<Point> points, string ship)
       {
          bool allTilesPlaceable = true;
          for (int i = 0; i < ship.Length; i++)
          {
             if (Game.ActivePlayer.pPlayer.X + i >= Game.BoardWidth && IsHorizontalPlacement
                 || Game.ActivePlayer.pPlayer.Y + i >= Game.BoardHeight && !IsHorizontalPlacement)
             {
                return false;
             }
             Point boardPoint;
             boardPoint = IsHorizontalPlacement ? 
                new Point(Game.ActivePlayer.pPlayer.X + i, Game.ActivePlayer.pPlayer.Y) : 
                new Point(Game.ActivePlayer.pPlayer.X, Game.ActivePlayer.pPlayer.Y + i);
             points.Add(boardPoint);
             // TODO add ship collision
          }
          return allTilesPlaceable;
       }


       private static bool PlaceShip(bool canBePlaced, List<Point> shipPoints, ref bool lockKeyboard)
       {
          if (canBePlaced)
          {
             DrawLogic.Message = "";
             if (Game.ConsoleEngine.GetKey(ConsoleKey.Z) && fKeyboardLockedMillis < 0) 
             {
                lockKeyboard = true;
                Game.ActivePlayer.BoardHover = new List<(Point, TileValue)>();
                foreach (var shipPoint in shipPoints)
                {
                   Game.GetBoard()[shipPoint.Y * Game.BoardWidth + shipPoint.X] = TileValue.Ship;
                }
                return true;
             }
             
             Game.ActivePlayer.BoardHover = new List<(Point, TileValue)>();
             foreach (var shipPoint in shipPoints)
             {
                Game.ActivePlayer.BoardHover.Add((new Point(shipPoint.X, shipPoint.Y), TileValue.Ship));
             }
             return false;
          }
          
          DrawLogic.Message = "Cannot place there!";
          Game.ActivePlayer.BoardHover = new List<(Point, TileValue)>();
          foreach (var shipPoint in shipPoints)
          {
             Game.ActivePlayer.BoardHover.Add((new Point(shipPoint.X, shipPoint.Y), TileValue.ImpossibleShip));
          }

          return false;
       }

       private void HandleKeyboardPanning(double gameTime)
       {
          if (Game.ConsoleEngine.GetKey(ConsoleKey.J))
          {
             fOffsetX -= (float) (50 * gameTime);
          }

          if (Game.ConsoleEngine.GetKey(ConsoleKey.K))
          {
             fOffsetY += (float) (50 * gameTime);
          }

          if (Game.ConsoleEngine.GetKey(ConsoleKey.L))
          {
             fOffsetX += (float) (50 * gameTime);
          }

          if (Game.ConsoleEngine.GetKey(ConsoleKey.I))
          {
             fOffsetY -= (float) (50 * gameTime);
          }
       }

       private void HandleMousePanning()
       {
          int x = Game.ConsoleEngine.GetMousePos().X;
          int y = Game.ConsoleEngine.GetMousePos().Y;
          if (x > 0 && x < Game.ScreenWidth && y > 0 && y < Game.ScreenHeight)
          {
             if (Game.ConsoleEngine.GetMouseLeft() && !mouseLeftIsHeld)
             {
                fStartPanX = x;
                fStartPanY = y;
                mouseLeftIsHeld = true;
             }

             if (!Game.ConsoleEngine.GetMouseLeft())
             {
                mouseLeftIsHeld = false;
             }

             if (Game.ConsoleEngine.GetMouseLeft())
             {
                fOffsetX -= (x - fStartPanX) / Game.ActivePlayer.fScaleX;
                fOffsetY -= (y - fStartPanY) / Game.ActivePlayer.fScaleY;

                fStartPanX = x;
                fStartPanY = y;
             }
          }
       }

       private void HandleZooming()
       {
          float fMouseWorldX_BeforeZoom, fMouseWorldY_BeforeZoom;
          ScreenToWorld(
             Game.ConsoleEngine.GetMousePos().X,
             Game.ConsoleEngine.GetMousePos().Y,
             out fMouseWorldX_BeforeZoom,
             out fMouseWorldY_BeforeZoom);

          if (Game.ConsoleEngine.GetKey(ConsoleKey.OemPlus))
          {
             Game.ActivePlayer.fScaleX *= 1.001f;
             Game.ActivePlayer.fScaleY *= 1.001f;
          }

          if (Game.ConsoleEngine.GetKey(ConsoleKey.OemMinus))
          {
             Game.ActivePlayer.fScaleX *= 0.999f;
             Game.ActivePlayer.fScaleY *= 0.999f;
          }

          float fMouseWorldX_AfterZoom, fMouseWorldY_AfterZoom;
          ScreenToWorld(
             Game.ConsoleEngine.GetMousePos().X,
             Game.ConsoleEngine.GetMousePos().Y,
             out fMouseWorldX_AfterZoom,
             out fMouseWorldY_AfterZoom);

          fOffsetX += fMouseWorldX_BeforeZoom - fMouseWorldX_AfterZoom;
          fOffsetY += fMouseWorldY_BeforeZoom - fMouseWorldY_AfterZoom;

          if (Game.ConsoleEngine.GetMouseLeft())
          {
             Game.ActivePlayer.fSelectedTileX = (int) fMouseWorldX_AfterZoom;
             Game.ActivePlayer.fSelectedTileY = (int) fMouseWorldY_AfterZoom;
          }
       }
       
    }
    
}