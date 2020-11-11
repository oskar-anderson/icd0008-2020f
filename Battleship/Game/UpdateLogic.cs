using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Game.Model;
using Game.Pack;
using Game.Tile;
using IrrKlang;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Game
{
    [SuppressMessage("ReSharper", "InlineOutVariableDeclaration")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class UpdateLogic
    {
       private static Player ActivePlayer
       {
          get => GameMain.GameData.ActivePlayer;
          set => GameMain.GameData.ActivePlayer = value;
       }

       public static float fOffsetY
       {
          get => ActivePlayer.fOffsetY;
          private set => ActivePlayer.fOffsetY = value;
       }
       public static float fOffsetX
       {
          get => ActivePlayer.fOffsetX;
          private set => ActivePlayer.fOffsetX = value;
       }
       
       
       private static float _fStartPanX = 0.0f;
       private static float _fStartPanY = 0.0f;

       private static bool _mouseLeftIsHeld = false;
       private static double _fKeyboardLockedMillis = 0.0d;

       private static Dictionary<ConsoleKey, bool> _newKeyDown = ResetKeyboardState();
       private static Dictionary<ConsoleKey, bool> _currentTurnKeyDown = ResetKeyboardState();
       
       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       public static bool Update(double gameTime)
       {
          if (GameMain.GameData.WinningPlayer != null)
          {
             while (true)
             {
                UpdateKeyboardState();
                if (_newKeyDown[ConsoleKey.Z] || _newKeyDown[ConsoleKey.Escape])
                {
                   break;
                }
             }
             Helper.FixConsole();
             Console.CursorVisible = true;
             return false;
          }
          UpdateKeyboardState();
          
          if (_newKeyDown[ConsoleKey.Escape])
          {
             Helper.FixConsole();
             Console.CursorVisible = true;
             return false;
          }

          _fKeyboardLockedMillis = Math.Max(-1d, _fKeyboardLockedMillis - gameTime);
          bool lockKeyboard = false;
          HandlePlayerMovement(ref lockKeyboard);
          _fKeyboardLockedMillis = lockKeyboard ? 0.1d : _fKeyboardLockedMillis;
          
          
          if (_newKeyDown[ConsoleKey.C])
          {
             ActivePlayer.IsViewingOwnBoard = !ActivePlayer.IsViewingOwnBoard;
             ActivePlayer.BoardHover.Clear();
             return true;
          }

          DrawLogic.DialogOptions.Clear();
          ResolvePhase();
          

          HandleZooming();
          HandleKeyboardPanning(gameTime);
          HandleMousePanning();
          
          return true;
       }

       private static Dictionary<ConsoleKey, bool> ResetKeyboardState()
       {
          var currentTurnKeyDown = new Dictionary<ConsoleKey, bool>
          {
             [ConsoleKey.R] = GameMain.ConsoleEngine.GetKey(ConsoleKey.R),
             [ConsoleKey.X] = GameMain.ConsoleEngine.GetKey(ConsoleKey.X),
             [ConsoleKey.Escape] = GameMain.ConsoleEngine.GetKey(ConsoleKey.Escape),
             [ConsoleKey.C] = GameMain.ConsoleEngine.GetKey(ConsoleKey.C),
             [ConsoleKey.Z] = GameMain.ConsoleEngine.GetKey(ConsoleKey.Z),
             [ConsoleKey.D1] = GameMain.ConsoleEngine.GetKey(ConsoleKey.D1),
             [ConsoleKey.D2] = GameMain.ConsoleEngine.GetKey(ConsoleKey.D2),
          };
          return currentTurnKeyDown;
       }

       private static void UpdateKeyboardState()
       {
          Dictionary<ConsoleKey, bool> lastTurnKeyDown = _currentTurnKeyDown.ToDictionary(entry => entry.Key, entry => entry.Value);
          _currentTurnKeyDown = ResetKeyboardState();

          _newKeyDown = new Dictionary<ConsoleKey, bool>();
          foreach (var key in lastTurnKeyDown.Keys)
          {
             _newKeyDown[key] = _currentTurnKeyDown[key] && !lastTurnKeyDown[key];
          }
       }

       private static void ResolvePhase()
       {
          switch (GameMain.GameData.Phase)
          {
             case 1:
                TileFunctions.playerTileValue = GameMain.GetBoard().Get(ActivePlayer.PPlayer) != TileData.TileValue.Ship ? 
                   TileData.SelectedTileGreen.exponent : 
                   TileData.SelectedTileRed.exponent;

                DrawLogic.DialogOptions.Add("Randomize");
                ConsoleKey dialogRandomize = ConsoleKey.D1;
                
                
                ConsoleKey dialogStart = default;
                if (ActivePlayer.ShipBeingPlacedIdx == -1)
                {
                   DrawLogic.DialogOptions.Add("Start");
                   dialogStart = ConsoleKey.D2;
                }
                
                if (dialogStart != default && _newKeyDown[dialogStart])
                {
                   (ActivePlayer, GameMain.GameData.InactivePlayer) = (GameMain.GameData.InactivePlayer, ActivePlayer);
                   if (ActivePlayer.ShipBeingPlacedIdx == -1)
                   {
                      ActivePlayer.IsViewingOwnBoard = !ActivePlayer.IsViewingOwnBoard;
                      GameMain.GameData.InactivePlayer.IsViewingOwnBoard = !GameMain.GameData.InactivePlayer.IsViewingOwnBoard;
                      GameMain.GameData.Phase = 2;
                      return;
                   }
                }
                          
                if (dialogRandomize != default && _newKeyDown[dialogRandomize])
                {
                   ActivePlayer.Board = TileFunctions.GetRndSeaTiles(GameMain.GameData.BoardWidth, GameMain.GameData.BoardHeight);
                   ActivePlayer.Ships.Clear();
                   ActivePlayer.ShipBeingPlacedIdx = -1;
                   ActivePlayer.BoardHover.Clear();
                   List<Rectangle> newBoard = ShipPlacement.PlaceShips(
                      GameMain.GameData.ShipSizes,
                      GameMain.GameData.BoardWidth,
                      GameMain.GameData.BoardHeight, 
                      GameMain.GameData.AllowedPlacementType);
                   foreach (var ship in newBoard)
                   {
                      List<Point> points = ship.ToPoints();
                      foreach (var p in points)
                      {
                         ActivePlayer.Board.Set(p, TileData.Ship.exponent);
                      }
                      ActivePlayer.Ships.Add(ship);
                   }
                }

                if (_newKeyDown[ConsoleKey.R])
                {
                   ActivePlayer.Ships.Clear();
                   ActivePlayer.ShipBeingPlacedIdx = 0;
                   ActivePlayer.Board = TileFunctions.GetRndSeaTiles(GameMain.GameData.BoardWidth, GameMain.GameData.BoardHeight);
                }

                if (_newKeyDown[ConsoleKey.X])
                {
                   ActivePlayer.IsHorizontalPlacement = !ActivePlayer.IsHorizontalPlacement;
                }
                
                if (ActivePlayer.IsViewingOwnBoard && ActivePlayer.ShipBeingPlacedIdx != -1)
                {
                   Point ship = GameMain.GameData.ShipSizes[ActivePlayer.ShipBeingPlacedIdx];
                   Rectangle rect = GameMain.GameData.ActivePlayer.IsHorizontalPlacement ? 
                      new Rectangle(ActivePlayer.PPlayer.X, ActivePlayer.PPlayer.Y, ship.X, ship.Y) :
                      new Rectangle(ActivePlayer.PPlayer.X, ActivePlayer.PPlayer.Y, ship.Y, ship.X);
                   List<Point> modelPoints;
                   if (CanPlaceShip(out modelPoints, rect))
                   {
                      // System.IO.Directory.GetParent("../../../../");
                      if (_newKeyDown[ConsoleKey.Z])
                      {
                         PlaceShip(modelPoints, rect);
                         GameMain.SoundEngine.Play2D("../../../../../media/flashlight_holster.ogg");
                      }
                   }
                }
                break;
             
             
             case 2:
                int selectedOppTileValue = GameMain.GameData.InactivePlayer.Board.Get(ActivePlayer.PPlayer);
                if (_newKeyDown[ConsoleKey.Z] && ! TileData.HitTiles.Contains(selectedOppTileValue))
                {
                   if (TileData.SeaTiles.Contains(selectedOppTileValue))
                   { 
                      ActivePlayer.ShootingHistory.Push(
                         new ShootingHistoryItem(
                            ActivePlayer.PPlayer,
                            selectedOppTileValue,
                            TileData.HitWater.exponent, 
                            null)
                      );
                      GameMain.GameData.InactivePlayer.Board.Set(ActivePlayer.PPlayer, TileData.HitWater.exponent);
                      (ActivePlayer, GameMain.GameData.InactivePlayer) = (GameMain.GameData.InactivePlayer, ActivePlayer);
                      GameMain.SoundEngine.Play2D("../../../../../media/Water_Impact_2.wav");
                      return;
                   }
                   if (TileData.TileValue.Ship == selectedOppTileValue)
                   {
                      Rectangle rect = GameMain.GameData.InactivePlayer.Ships.FirstOrDefault(x => x.Contains(ActivePlayer.PPlayer));
                      List<Point> rectAsPoints = rect.ToPoints();
                      bool isShipDestroyed = rectAsPoints
                         .Where(p => p != ActivePlayer.PPlayer)
                         .All(p => GameMain.GameData.InactivePlayer.Board.Get(p) == TileData.HitShip.exponent);

                      if (isShipDestroyed)
                      {
                         List<Point> hitboxRectAsPoints = rect.ToHitboxPoints(GameMain.GameData.AllowedPlacementType);
                         List<ShootingHistoryItem> changes = GetAreaOfSunkenShipRevealChanges(hitboxRectAsPoints, GameMain.GameData.InactivePlayer.Board);
                         foreach (var change in changes)
                         {
                            GameMain.GameData.InactivePlayer.Board.Set(change.point, change.currValue);
                         }
                         ActivePlayer.ShootingHistory.Push(new ShootingHistoryItem(
                            ActivePlayer.PPlayer,
                            selectedOppTileValue,
                            TileData.HitShip.exponent, 
                            changes));
                         GameMain.SoundEngine.Play2D("../../../../../media/bigExp.wav");
                         
                         List<bool> areAllShipsDestroyed = new List<bool>();
                         foreach (var points in GameMain.GameData.InactivePlayer.Ships.Select(ship => ship.ToPoints()))
                         {
                            areAllShipsDestroyed.Add(points.All(point => GameMain.GameData.InactivePlayer.Board.Get(point) == TileData.HitShip.exponent));
                         }

                         if (areAllShipsDestroyed.All(x => x))
                         {
                            GameMain.GameData.WinningPlayer = ActivePlayer.Name;
                         }
                         
                      }
                      else
                      {
                         GameMain.GameData.InactivePlayer.Board.Set(ActivePlayer.PPlayer, TileData.HitShip.exponent);
                         ActivePlayer.ShootingHistory.Push(
                            new ShootingHistoryItem(
                               ActivePlayer.PPlayer,
                               selectedOppTileValue,
                               TileData.HitShip.exponent, 
                               null)
                         );
                         GameMain.SoundEngine.Play2D("../../../../../media/missileExplode.wav");
                      }
                   }
                   else {
                      throw new Exception("Unexpected!");
                   }
                }

                if (_newKeyDown[ConsoleKey.R] && ActivePlayer.ShootingHistory.Count != 0)
                {
                   ShootingHistoryItem historyItem = ActivePlayer.ShootingHistory.Pop();
                   if (historyItem.allChangedPoints != null)
                   {
                      foreach (var changedPoint in historyItem.allChangedPoints) 
                      { 
                         GameMain.GameData.InactivePlayer.Board.Set(changedPoint.point, changedPoint.prevValue); 
                      }
                   }
                   else
                   {
                      GameMain.GameData.InactivePlayer.Board.Set(historyItem.point, historyItem.prevValue);
                   }
                }
                break;
             default:
                throw new Exception("Unexpected!");
          }
       }


       private static List<ShootingHistoryItem> GetAreaOfSunkenShipRevealChanges(List<Point> hitboxRectAsPoints, int[,] board)
       {
          Rectangle bounds = new Rectangle(0, 0, board.GetLength(0), board.GetLength(1));
          List<ShootingHistoryItem> shootingHistoryItems = new List<ShootingHistoryItem>();
          foreach (var rectPoint in hitboxRectAsPoints)
          {
             if (!bounds.Contains(rectPoint))
             {
                continue;
             }
             var selectedOppTileValueIterTmp = board.Get(rectPoint);
             if (TileData.SeaTiles.Contains(selectedOppTileValueIterTmp))
             {
                int newTileValue = TileData.HitWater.exponent;
                if (selectedOppTileValueIterTmp == TileData.HitWater.exponent)
                {
                   continue;
                }
                shootingHistoryItems.Add(
                   new ShootingHistoryItem(
                      rectPoint, 
                      selectedOppTileValueIterTmp, 
                      newTileValue, 
                      null));
             } 
             else if (TileData.TileValue.Ship == selectedOppTileValueIterTmp)
             {
                int newTileValue = TileData.HitShip.exponent;
                if (selectedOppTileValueIterTmp == newTileValue)
                {
                   continue;
                }
                shootingHistoryItems.Add(
                   new ShootingHistoryItem(
                      rectPoint, 
                      selectedOppTileValueIterTmp, 
                      newTileValue,
                      null));
             }
          }

          return shootingHistoryItems;

       }

       public static void WorldToScreen(float fWorldX, float fWorldY, out int nScreenX, out int nScreenY)
       {
          nScreenX = (int) Math.Floor((fWorldX - fOffsetX) * ActivePlayer.fScaleX);
          nScreenY = (int) Math.Floor((fWorldY - fOffsetY) * ActivePlayer.fScaleY);
       }
          
          
       public static void ScreenToWorld(int nScreenX, int nScreenY, out float fWorldX, out float fWorldY)
       {
          fWorldX = (nScreenX / ActivePlayer.fScaleX) + fOffsetX;
          fWorldY = (nScreenY / ActivePlayer.fScaleY) + fOffsetY;
       }


       private static void HandlePlayerMovement(ref bool lockKeyboard)
       {
          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.A) || GameMain.ConsoleEngine.GetKey(ConsoleKey.LeftArrow))
          {
             if (ActivePlayer.PPlayer.X > 0 && _fKeyboardLockedMillis < 0)
             {
                ActivePlayer.PPlayer = new Point(ActivePlayer.PPlayer.X - 1, ActivePlayer.PPlayer.Y);
                lockKeyboard = true;
                fOffsetX -= TileData.GetWidth();
             }
          }

          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.S) || GameMain.ConsoleEngine.GetKey(ConsoleKey.DownArrow))
          {
             if (ActivePlayer.PPlayer.Y < GameMain.GameData.BoardHeight - 1 && _fKeyboardLockedMillis < 0)
             {
                ActivePlayer.PPlayer = new Point(ActivePlayer.PPlayer.X, ActivePlayer.PPlayer.Y + 1);
                lockKeyboard = true;
                fOffsetY += TileData.GetHeight();
             }
          }

          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.D) || GameMain.ConsoleEngine.GetKey(ConsoleKey.RightArrow))
          {
             if (ActivePlayer.PPlayer.X < GameMain.GameData.BoardWidth - 1 && _fKeyboardLockedMillis < 0)
             {
                ActivePlayer.PPlayer = new Point(ActivePlayer.PPlayer.X + 1, ActivePlayer.PPlayer.Y);
                lockKeyboard = true;
                fOffsetX += TileData.GetWidth();
             }
          }

          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.W) || GameMain.ConsoleEngine.GetKey(ConsoleKey.UpArrow))
          {
             if (ActivePlayer.PPlayer.Y > 0 && _fKeyboardLockedMillis < 0)
             {
                ActivePlayer.PPlayer = new Point(ActivePlayer.PPlayer.X, ActivePlayer.PPlayer.Y - 1);
                lockKeyboard = true;
                fOffsetY -= TileData.GetHeight();
             }
          }
       }

       private static bool CanPlaceShip(out List<Point> modelPoints, Rectangle rect)
       {
          modelPoints = new List<Point>();
          List<Point> hitboxRectAsPoints = rect.ToHitboxPoints(GameMain.GameData.AllowedPlacementType);
          Rectangle board = new Rectangle(0, 0, GameMain.GameData.BoardWidth, GameMain.GameData.BoardHeight);
          ActivePlayer.BoardHover = new List<Player.HoverElement>();
          bool canBePlacedEntirely = true;
          foreach (var boundBoxPoint in hitboxRectAsPoints)
          {
             bool isPointInModelBox = rect.Contains(boundBoxPoint);
             bool isPointOutOfBounds = !board.Contains(boundBoxPoint);
             if (isPointOutOfBounds && !isPointInModelBox)
             {
                continue;
             }
             bool doesHitBoxIntersect = ActivePlayer.Ships.Any(ship => ship.Contains(boundBoxPoint));
             bool isPlaceable = !(isPointInModelBox && isPointOutOfBounds || doesHitBoxIntersect);
             TileData.TileInfo tile;
             if (isPlaceable)
             {
                tile = isPointInModelBox ? TileData.Ship : TileData.ImpossibleShipHitbox;
             }
             else
             {
                canBePlacedEntirely = false;
                tile = TileData.ImpossibleShip;
             }
             ActivePlayer.BoardHover.Add(new Player.HoverElement(boundBoxPoint, tile.exponent));
             if (isPointInModelBox)
             {
                modelPoints.Add(boundBoxPoint);
             }
          }
          DrawLogic.Message = canBePlacedEntirely ? "" : "Cannot place there!";
          return canBePlacedEntirely;
       }


       private static void PlaceShip(List<Point> shipTiles, Rectangle rect)
       {
          ActivePlayer.BoardHover = new List<Player.HoverElement>();
          foreach (var shipTile in shipTiles)
          {
             GameMain.GetBoard().Set(shipTile, TileData.TileValue.Ship);
          }
          TileFunctions.playerTileValue = TileData.SelectedTileRed.exponent; 
          ActivePlayer.Ships.Add(rect);
          ActivePlayer.ShipBeingPlacedIdx = ActivePlayer.ShipBeingPlacedIdx != ActivePlayer.Ships.Capacity - 1 ? 
             ActivePlayer.ShipBeingPlacedIdx + 1 : -1;
          
       }

       private static void HandleKeyboardPanning(double gameTime)
       {
          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.J))
          {
             fOffsetX -= (float) (50 * gameTime);
          }

          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.K))
          {
             fOffsetY += (float) (50 * gameTime);
          }

          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.L))
          {
             fOffsetX += (float) (50 * gameTime);
          }

          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.I))
          {
             fOffsetY -= (float) (50 * gameTime);
          }
       }

       private static void HandleMousePanning()
       {
          int x = GameMain.ConsoleEngine.GetMousePos().X;
          int y = GameMain.ConsoleEngine.GetMousePos().Y;
          if (x > 0 && x < GameMain.ScreenWidth && y > 0 && y < GameMain.ScreenHeight)
          {
             if (GameMain.ConsoleEngine.GetMouseLeft() && !_mouseLeftIsHeld)
             {
                _fStartPanX = x;
                _fStartPanY = y;
                _mouseLeftIsHeld = true;
             }

             if (!GameMain.ConsoleEngine.GetMouseLeft())
             {
                _mouseLeftIsHeld = false;
             }

             if (GameMain.ConsoleEngine.GetMouseLeft())
             {
                fOffsetX -= (x - _fStartPanX) / ActivePlayer.fScaleX;
                fOffsetY -= (y - _fStartPanY) / ActivePlayer.fScaleY;

                _fStartPanX = x;
                _fStartPanY = y;
             }
          }
       }

       private static void HandleZooming()
       {
          float fMouseWorldX_BeforeZoom, fMouseWorldY_BeforeZoom;
          ScreenToWorld(
             GameMain.ConsoleEngine.GetMousePos().X,
             GameMain.ConsoleEngine.GetMousePos().Y,
             out fMouseWorldX_BeforeZoom,
             out fMouseWorldY_BeforeZoom);

          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.OemPlus))
          {
             ActivePlayer.fScaleX *= 1.001f;
             ActivePlayer.fScaleY *= 1.001f;
          }

          if (GameMain.ConsoleEngine.GetKey(ConsoleKey.OemMinus))
          {
             ActivePlayer.fScaleX *= 0.999f;
             ActivePlayer.fScaleY *= 0.999f;
          }

          float fMouseWorldX_AfterZoom, fMouseWorldY_AfterZoom;
          ScreenToWorld(
             GameMain.ConsoleEngine.GetMousePos().X,
             GameMain.ConsoleEngine.GetMousePos().Y,
             out fMouseWorldX_AfterZoom,
             out fMouseWorldY_AfterZoom);

          fOffsetX += fMouseWorldX_BeforeZoom - fMouseWorldX_AfterZoom;
          fOffsetY += fMouseWorldY_BeforeZoom - fMouseWorldY_AfterZoom;

          if (GameMain.ConsoleEngine.GetMouseLeft())
          {
             ActivePlayer.fSelectedTileX = (int) fMouseWorldX_AfterZoom;
             ActivePlayer.fSelectedTileY = (int) fMouseWorldY_AfterZoom;
          }
       }
       
       
       private static void SetScreenOffset()
       {
          ActivePlayer.fOffsetX = (ActivePlayer.PPlayer.X - 4) * TileData.GetWidth();
          ActivePlayer.fOffsetY = (ActivePlayer.PPlayer.Y - 4)* TileData.GetHeight();
       }
    }
}