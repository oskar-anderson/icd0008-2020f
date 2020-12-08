using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Domain;
using Domain.Model;
using Game.Pack;
using Game.Tile;
using IrrKlang;
using RogueSharp;
using Point = RogueSharp.Point;

namespace Game
{
    [SuppressMessage("ReSharper", "InlineOutVariableDeclaration")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class UpdateLogic
    {
       private static float _fStartPanX = 0.0f;
       private static float _fStartPanY = 0.0f;

       private static bool _mouseLeftIsHeld = false;
       private static double _fKeyboardLockedMillis = -1d;
       
       private readonly BaseBattleship.UpdateExitAction UpdateExitActionStrategy;
       private readonly BaseI_Input Input;
       private readonly ISoundEngine SoundEngine;

       public UpdateLogic(BaseBattleship.UpdateExitAction updateExitAction, BaseI_Input input, ISoundEngine soundEngine)
       {
          UpdateExitActionStrategy = updateExitAction;
          Input = input;
          SoundEngine = soundEngine;
       }

       /// <param name="gameTime">Provides a snapshot of timing values.</param>
       /// <param name="gameData">Game data</param>
       /// <param name="drawLogicData">Properties like error messages and available dialog options used in drawing functions</param>
       public bool Update(double gameTime, GameData gameData, out DrawLogicData drawLogicData)
       {
          drawLogicData = new DrawLogicData();
          Input.UpdateKeyboardState();
          if (gameData.WinningPlayer != null)
          {
             if (Input.NewKeyDown[ConsoleKey.Z] || Input.NewKeyDown[ConsoleKey.Escape])
             { 
                UpdateExitActionStrategy(); 
                return false;
             }
             return true;
          }

          if (Input.NewKeyDown[ConsoleKey.Escape])
          {
             UpdateExitActionStrategy();
             return false;
          }

          _fKeyboardLockedMillis = Math.Max(-1d, _fKeyboardLockedMillis - gameTime);
          bool lockKeyboard = false;
          HandlePlayerMovement(ref lockKeyboard, gameData.ActivePlayer);
          _fKeyboardLockedMillis = lockKeyboard ? 0.1d : _fKeyboardLockedMillis;
          
          drawLogicData.DialogOptions.Clear();
          ResolvePhase(gameData, drawLogicData);
          

          HandleZooming(gameData.ActivePlayer);
          HandleKeyboardPanning(gameTime, gameData.ActivePlayer);
          HandleMousePanning(gameData.ActivePlayer);
          HandleMouseSelection(gameData.ActivePlayer);
          
          return true;
       }

       private void ResolvePhase(GameData gameData, DrawLogicData drawLogicData)
       {
          switch (gameData.Phase)
          {
             case 1:
                drawLogicData.PlayerTileValue = BaseBattleship.GetBoard(gameData).Get(gameData.ActivePlayer.PPlayer) != TileData.TileValue.Ship ? 
                   TileData.SelectedTileGreen.exponent : 
                   TileData.SelectedTileRed.exponent;

                const ConsoleKey dialogAction = ConsoleKey.Z;
                const ConsoleKey dialogRot = ConsoleKey.X;
                const ConsoleKey dialogRandomize = ConsoleKey.D1;
                const ConsoleKey dialogClear = ConsoleKey.D2;
                const ConsoleKey dialogStart = ConsoleKey.D3;

                ShipPlacementStatus shipPlacementStatus = GetShipPlacementStatus(gameData);
                Dictionary<ConsoleKey, DrawLogicData.DialogItem> activeKeys = new Dictionary<ConsoleKey, DrawLogicData.DialogItem>() 
                {
                   { dialogAction, new DrawLogicData.DialogItem(shipPlacementStatus.isPlaceable, "Z", "Place") },
                   { dialogRot, new DrawLogicData.DialogItem(shipPlacementStatus.hitboxRect != null,"X", "Rotate") },
                   { dialogRandomize, new DrawLogicData.DialogItem(gameData.ActivePlayer.IsViewingOwnBoard,"1", "Randomize") },
                   { dialogClear, new DrawLogicData.DialogItem(gameData.ActivePlayer.IsViewingOwnBoard,"2", "Clear") },
                   { dialogStart, new DrawLogicData.DialogItem(shipPlacementStatus.isStartable,"3", "Start") }
                };
                
                foreach (var keyValuePair in activeKeys.Where(keyValuePair => keyValuePair.Value.isActive))
                {
                   drawLogicData.DialogOptions.Add(keyValuePair.Value);
                }
                
                if (activeKeys[dialogStart].isActive && Input.NewKeyDown[dialogStart])
                {
                   (gameData.ActivePlayer, gameData.InactivePlayer) = (gameData.InactivePlayer, gameData.ActivePlayer);
                   shipPlacementStatus = GetShipPlacementStatus(gameData);
                   if (gameData.ActivePlayer.ShipBeingPlacedIdx == -1)
                   {
                      gameData.ActivePlayer.IsViewingOwnBoard = !gameData.ActivePlayer.IsViewingOwnBoard;
                      gameData.InactivePlayer.IsViewingOwnBoard = !gameData.InactivePlayer.IsViewingOwnBoard;
                      gameData.Phase = 2;
                      return;
                   }
                }
                          
                if (activeKeys[dialogRandomize].isActive && Input.NewKeyDown[dialogRandomize])
                {
                   gameData.ActivePlayer.Board = TileFunctions.GetRndSeaTiles(gameData.ActivePlayer.Board.GetWidth(), gameData.ActivePlayer.Board.GetHeight());
                   gameData.ActivePlayer.Ships.Clear();
                   gameData.ActivePlayer.ShipBeingPlacedIdx = -1;
                   List<Rectangle> newBoard = ShipPlacement.PlaceShips(
                      gameData.ShipSizes,
                      gameData.ActivePlayer.Board.GetWidth(),
                      gameData.ActivePlayer.Board.GetHeight(), 
                      gameData.AllowedPlacementType);
                   gameData.ActivePlayer.Ships = newBoard;
                   foreach (var ship in newBoard)
                   {
                      List<Point> points = ship.ToPoints();
                      foreach (var p in points)
                      {
                         gameData.ActivePlayer.Board.Set(p, TileData.Ship.exponent);
                      }
                   }
                   shipPlacementStatus = GetShipPlacementStatus(gameData);
                }

                if (activeKeys[dialogClear].isActive && Input.NewKeyDown[dialogClear])
                {
                   gameData.ActivePlayer.Ships.Clear();
                   gameData.ActivePlayer.ShipBeingPlacedIdx = 0;
                   gameData.ActivePlayer.Board = TileFunctions.GetRndSeaTiles(gameData.ActivePlayer.Board.GetWidth(), gameData.ActivePlayer.Board.GetHeight());
                   shipPlacementStatus = GetShipPlacementStatus(gameData);
                }

                if (shipPlacementStatus.isPlaceable && activeKeys[dialogAction].isActive && Input.NewKeyDown[dialogAction])
                {
                   if (shipPlacementStatus.modelPoints == null || shipPlacementStatus.hitboxRect == null) { throw new Exception("Unexpected!");}
                   PlaceShip(shipPlacementStatus.modelPoints, (Rectangle) shipPlacementStatus.hitboxRect, 
                      BaseBattleship.GetBoard(gameData), gameData.ActivePlayer, 
                      gameData.ShipSizes.Count, drawLogicData);
                   SoundEngine.Play2D("../../../../../media/flashlight_holster.ogg");
                }

                if (activeKeys[dialogRot].isActive && Input.NewKeyDown[dialogRot])
                {
                   gameData.ActivePlayer.IsHorizontalPlacement = !gameData.ActivePlayer.IsHorizontalPlacement;
                }
                break;
             
             
             case 2:
                int selectedOppTileValue = gameData.InactivePlayer.Board.Get(gameData.ActivePlayer.PPlayer);
                drawLogicData.DialogOptions.Add(new DrawLogicData.DialogItem(true, "Z", "Shoot"));
                if (Input.NewKeyDown[ConsoleKey.Z] && ! TileData.HitTiles.Contains(selectedOppTileValue))
                {
                   if (TileData.SeaTiles.Contains(selectedOppTileValue))
                   { 
                      gameData.ActivePlayer.ShootingHistory.Push(
                         new ShootingHistoryItem(
                            gameData.ActivePlayer.PPlayer,
                            selectedOppTileValue,
                            TileData.HitWater.exponent, 
                            null)
                      );
                      gameData.InactivePlayer.Board.Set(gameData.ActivePlayer.PPlayer, TileData.HitWater.exponent);
                      (gameData.ActivePlayer, gameData.InactivePlayer) = (gameData.InactivePlayer, gameData.ActivePlayer);
                      SoundEngine.Play2D("../../../../../media/Water_Impact_2.wav");
                      return;
                   }
                   if (TileData.TileValue.Ship == selectedOppTileValue)
                   {
                      Rectangle rect = gameData.InactivePlayer.Ships.FirstOrDefault(x => x.Contains(gameData.ActivePlayer.PPlayer));
                      List<Point> rectAsPoints = rect.ToPoints();
                      bool isShipDestroyed = rectAsPoints
                         .Where(p => p != gameData.ActivePlayer.PPlayer)
                         .All(p => gameData.InactivePlayer.Board.Get(p) == TileData.HitShip.exponent);

                      if (isShipDestroyed)
                      {
                         List<Point> hitboxRectAsPoints = rect.ToHitboxPoints(gameData.AllowedPlacementType);
                         List<ShootingHistoryItem> changes = GetAreaOfSunkenShipRevealChanges(hitboxRectAsPoints, gameData.InactivePlayer.Board);
                         foreach (var change in changes)
                         {
                            gameData.InactivePlayer.Board.Set(change.point, change.currValue);
                         }
                         gameData.ActivePlayer.ShootingHistory.Push(new ShootingHistoryItem(
                            gameData.ActivePlayer.PPlayer,
                            selectedOppTileValue,
                            TileData.HitShip.exponent, 
                            changes));
                         SoundEngine.Play2D("../../../../../media/bigExp.wav");
                         
                         List<bool> areAllShipsDestroyed = new List<bool>();
                         foreach (var points in gameData.InactivePlayer.Ships.Select(ship => ship.ToPoints()))
                         {
                            areAllShipsDestroyed.Add(
                               points.All(point => gameData.InactivePlayer.Board.Get(point) == TileData.HitShip.exponent)
                            );
                         }

                         if (areAllShipsDestroyed.All(x => x))
                         {
                            gameData.WinningPlayer = gameData.ActivePlayer.Name;
                         }
                         
                      }
                      else
                      {
                         gameData.InactivePlayer.Board.Set(gameData.ActivePlayer.PPlayer, TileData.HitShip.exponent);
                         gameData.ActivePlayer.ShootingHistory.Push(
                            new ShootingHistoryItem(
                               gameData.ActivePlayer.PPlayer,
                               selectedOppTileValue,
                               TileData.HitShip.exponent, 
                               null)
                         );
                         SoundEngine.Play2D("../../../../../media/missileExplode.wav");
                      }
                   }
                   else {
                      throw new Exception("Unexpected!");
                   }
                }

                if (Input.NewKeyDown[ConsoleKey.R] && gameData.ActivePlayer.ShootingHistory.Count != 0)
                {
                   ShootingHistoryItem historyItem = gameData.ActivePlayer.ShootingHistory.Pop();
                   if (historyItem.allChangedPoints != null)
                   {
                      foreach (var changedPoint in historyItem.allChangedPoints) 
                      { 
                         gameData.InactivePlayer.Board.Set(changedPoint.point, changedPoint.prevValue); 
                      }
                   }
                   else
                   {
                      gameData.InactivePlayer.Board.Set(historyItem.point, historyItem.prevValue);
                   }
                }
                break;
             default:
                throw new Exception("Unexpected!");
          }
       }


       public static ShipPlacementStatus GetShipPlacementStatus(GameData gameData)
       {
          if (gameData.ActivePlayer.IsViewingOwnBoard && gameData.ActivePlayer.ShipBeingPlacedIdx != -1)
          {
             Point shipBeingPlaced = gameData.ShipSizes[gameData.ActivePlayer.ShipBeingPlacedIdx];
             Rectangle hitboxRect = gameData.ActivePlayer.IsHorizontalPlacement ? 
                new Rectangle(gameData.ActivePlayer.PPlayer.X, gameData.ActivePlayer.PPlayer.Y, shipBeingPlaced.Y, shipBeingPlaced.X) :
                new Rectangle(gameData.ActivePlayer.PPlayer.X, gameData.ActivePlayer.PPlayer.Y, shipBeingPlaced.X, shipBeingPlaced.Y);
             if (CanPlaceShip(out List<Point> modelPoints, hitboxRect, gameData))
             {
                return new ShipPlacementStatus(false, true, hitboxRect, modelPoints);
             }
             return new ShipPlacementStatus(false, false, hitboxRect, null);
          }
          return new ShipPlacementStatus(true, false, null, null);
       }

       public struct ShipPlacementStatus
       {
          public readonly bool isStartable;
          public readonly bool isPlaceable;
          public readonly Rectangle? hitboxRect;
          public readonly List<Point>? modelPoints;

          public ShipPlacementStatus(bool isStartable, bool isPlaceable, Rectangle? hitboxRect, List<Point>? modelPoints)
          {
             this.isStartable = isStartable;
             this.isPlaceable = isPlaceable;
             this.hitboxRect = hitboxRect;
             this.modelPoints = modelPoints;
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
       public static void WorldToScreen(
          float fWorldX, float fWorldY, 
          Player player, 
          out int nScreenX, out int nScreenY)
       {
          nScreenX = (int) Math.Floor((fWorldX - player.fOffsetX) * player.fScaleX);
          nScreenY = (int) Math.Floor((fWorldY - player.fOffsetY) * player.fScaleY);
       }

       public static void WorldToScreen(
          float fWorldX, float fWorldY, 
          float fScaleX, float fScaleY, 
          float fOffsetX, float fOffsetY, 
          out int nScreenX, out int nScreenY)
       {
          nScreenX = (int) Math.Floor((fWorldX - fOffsetX) * fScaleX);
          nScreenY = (int) Math.Floor((fWorldY - fOffsetY) * fScaleY);
       }
       
       public static void ScreenToWorld(
          int nScreenX, int nScreenY, 
          Player player, 
          out float fWorldX, out float fWorldY)
       {
          fWorldX = (nScreenX / player.fScaleX) + player.fOffsetX;
          fWorldY = (nScreenY / player.fScaleY) + player.fOffsetY;
       }
          
       public static void ScreenToWorld(
          int nScreenX, int nScreenY, 
          float fScaleX, float fScaleY, 
          float fOffsetX, float fOffsetY, 
          out float fWorldX, out float fWorldY)
       {
          fWorldX = (nScreenX / fScaleX) + fOffsetX;
          fWorldY = (nScreenY / fScaleY) + fOffsetY;
       }


       private void HandlePlayerMovement(ref bool lockKeyboard, Player player)
       {
          if (Input.KeyDown[ConsoleKey.A] || Input.KeyDown[ConsoleKey.LeftArrow])
          {
             if (player.PPlayer.X > 0 && _fKeyboardLockedMillis < 0)
             {
                player.PPlayer = new Point(player.PPlayer.X - 1, player.PPlayer.Y);
                lockKeyboard = true;
                player.fOffsetX -= TileData.GetWidth();
             }
          }

          if (Input.KeyDown[ConsoleKey.S] || Input.KeyDown[ConsoleKey.DownArrow])
          {
             if (player.PPlayer.Y < player.Board.GetHeight() - 1 && _fKeyboardLockedMillis < 0)
             {
                player.PPlayer = new Point(player.PPlayer.X, player.PPlayer.Y + 1);
                lockKeyboard = true;
                player.fOffsetY += TileData.GetHeight();
             }
          }

          if (Input.KeyDown[ConsoleKey.D] || Input.KeyDown[ConsoleKey.RightArrow])
          {
             if (player.PPlayer.X < player.Board.GetWidth() - 1 && _fKeyboardLockedMillis < 0)
             {
                player.PPlayer = new Point(player.PPlayer.X + 1, player.PPlayer.Y);
                lockKeyboard = true;
                player.fOffsetX += TileData.GetWidth();
             }
          }

          if (Input.KeyDown[ConsoleKey.W] || Input.KeyDown[ConsoleKey.UpArrow])
          {
             if (player.PPlayer.Y > 0 && _fKeyboardLockedMillis < 0)
             {
                player.PPlayer = new Point(player.PPlayer.X, player.PPlayer.Y - 1);
                lockKeyboard = true;
                player.fOffsetY -= TileData.GetHeight();
             }
          }
       }

       private static bool CanPlaceShip(out List<Point> modelPoints, Rectangle rect, GameData gameData)
       {
          CanPlaceShipData canPlaceShipData = CanPlaceShipCommon(rect, gameData);
          modelPoints = new List<Point>();
          for (int i = 0; i < canPlaceShipData.hitboxOutOfBounds.Count; i++)
          {
             if (canPlaceShipData.hitboxOutOfBounds[i]) { continue; }
             if (canPlaceShipData.isPointInModelBox[i])
             {
                modelPoints.Add(canPlaceShipData.Points[i]);
             }
          }
          return canPlaceShipData.shipPointIsPlaceable.All(x => x);
       }
       
       public static CanPlaceShipHoverStatus CanPlaceShipHover(Rectangle rect, GameData gameData)
       {
          List<Player.HoverElement> boardHover = new List<Player.HoverElement>();
          CanPlaceShipData canPlaceShipData = CanPlaceShipCommon(rect, gameData);
          for (var i = 0; i < canPlaceShipData.hitboxOutOfBounds.Count; i++)
          {
             if (canPlaceShipData.hitboxOutOfBounds[i]) { continue; }
             TileData.TileInfo tile;
             if (canPlaceShipData.shipPointIsPlaceable[i])
             {
                tile = canPlaceShipData.isPointInModelBox[i] ? TileData.Ship : TileData.ImpossibleShipHitbox;
             }
             else
             {
                tile = TileData.ImpossibleShip;
             }
             boardHover.Add(new Player.HoverElement(canPlaceShipData.Points[i], tile.exponent));
          }

          bool canBePlacedEntirely = canPlaceShipData.shipPointIsPlaceable.All(x => x);
          return new CanPlaceShipHoverStatus(canBePlacedEntirely, boardHover);
       }
       
       public struct CanPlaceShipHoverStatus
       {
          public readonly bool canBePlacedEntirely;
          public readonly List<Player.HoverElement> boardHover;
          
          public CanPlaceShipHoverStatus(bool _canBePlacedEntirely, List<Player.HoverElement> _boardHover)
          {
             canBePlacedEntirely = _canBePlacedEntirely;
             boardHover = _boardHover;
          }
       }
       
       private static CanPlaceShipData CanPlaceShipCommon(Rectangle rect, GameData gameData)
       {
          CanPlaceShipData canPlaceShipData = new CanPlaceShipData();
          List<Point> hitboxRectAsPoints = rect.ToHitboxPoints(gameData.AllowedPlacementType);
          Rectangle board = new Rectangle(0, 0, gameData.ActivePlayer.Board.GetWidth(), gameData.ActivePlayer.Board.GetHeight());
          foreach (var boundBoxPoint in hitboxRectAsPoints)
          {
             bool isPointInModelBox = rect.Contains(boundBoxPoint);
             bool isPointOutOfBounds = !board.Contains(boundBoxPoint);
             bool doesHitBoxIntersect = gameData.ActivePlayer.Ships.Any(ship => ship.Contains(boundBoxPoint));
             bool isPlaceable = !(isPointInModelBox && isPointOutOfBounds || doesHitBoxIntersect);
             bool hitboxOutOfBounds = isPointOutOfBounds && !isPointInModelBox;
             canPlaceShipData.Add(hitboxOutOfBounds, isPlaceable, isPointInModelBox, boundBoxPoint);
          }
          return canPlaceShipData;
       }
       
       private class CanPlaceShipData
       {
          public readonly List<bool> hitboxOutOfBounds;
          public readonly List<bool> shipPointIsPlaceable;
          public readonly List<bool> isPointInModelBox;
          public readonly List<Point> Points;
          
          public CanPlaceShipData()
          {
             this.hitboxOutOfBounds = new List<bool>();
             this.shipPointIsPlaceable = new List<bool>();
             this.isPointInModelBox = new List<bool>();
             this.Points = new List<Point>();
          }

          public void Add(bool _hitboxOutOfBounds, bool _shipPointIsPlaceable, bool _isPointInModelBox, Point _point)
          {
             this.hitboxOutOfBounds.Add(_hitboxOutOfBounds);
             this.shipPointIsPlaceable.Add(_shipPointIsPlaceable);
             this.isPointInModelBox.Add(_isPointInModelBox);
             this.Points.Add(_point);
          }
       }



       private static void PlaceShip(List<Point> shipTiles, Rectangle rect, int [,] board, Player player, int maxShipCount, DrawLogicData drawLogicData)
       {
          foreach (var shipTile in shipTiles)
          {
             board.Set(shipTile, TileData.TileValue.Ship);
          }
          drawLogicData.PlayerTileValue = TileData.SelectedTileRed.exponent; 
          player.Ships.Add(rect);
          player.ShipBeingPlacedIdx = player.ShipBeingPlacedIdx != maxShipCount - 1 ? 
             player.ShipBeingPlacedIdx + 1 : -1;
          
       }

       private void HandleKeyboardPanning(double gameTime, Player player)
       {
          if (Input.KeyDown[ConsoleKey.J])
          {
             player.fOffsetX -= (float) (50 * gameTime);
          }

          if (Input.KeyDown[ConsoleKey.K])
          {
             player.fOffsetY += (float) (50 * gameTime);
          }

          if (Input.KeyDown[ConsoleKey.L])
          {
             player.fOffsetX += (float) (50 * gameTime);
          }

          if (Input.KeyDown[ConsoleKey.I])
          {
             player.fOffsetY -= (float) (50 * gameTime);
          }
       }

       private void HandleMousePanning(Player player)
       {
          var input = Input.GetMousePos();
          bool mouseLeftWasHeld = _mouseLeftIsHeld;
          _mouseLeftIsHeld = Input.GetMouseLeft();
          if (!mouseLeftWasHeld)
          {
             _fStartPanX = input.X;
             _fStartPanY = input.Y;
          }

          if (!_mouseLeftIsHeld || !mouseLeftWasHeld)
          {
             return;
          }
          player.fOffsetX -= (input.X - _fStartPanX) / player.fScaleX;
          player.fOffsetY -= (input.Y - _fStartPanY) / player.fScaleY;

          _fStartPanX = input.X;
          _fStartPanY = input.Y;
       }

       private void HandleZooming(Player player)
       {
          bool zoomNegative = Input.KeyDown[ConsoleKey.OemMinus];
          bool zoomPositive = Input.KeyDown[ConsoleKey.OemPlus];
          if (! zoomNegative && ! zoomPositive)
          {
             return;
          }
          var mousePos = Input.GetMousePos();
          float fMouseWorldX_BeforeZoom, fMouseWorldY_BeforeZoom;
          ScreenToWorld(
             mousePos.X,
             mousePos.Y,
             player,
             out fMouseWorldX_BeforeZoom,
             out fMouseWorldY_BeforeZoom);

          if (zoomPositive)
          {
             player.fScaleX *= 1.001f;
             player.fScaleY *= 1.001f;
          }

          if (zoomNegative)
          {
             player.fScaleX *= 0.999f;
             player.fScaleY *= 0.999f;
          }

          float fMouseWorldX_AfterZoom, fMouseWorldY_AfterZoom;
          ScreenToWorld(
             mousePos.X,
             mousePos.Y,
             player,
             out fMouseWorldX_AfterZoom,
             out fMouseWorldY_AfterZoom);

          player.fOffsetX += fMouseWorldX_BeforeZoom - fMouseWorldX_AfterZoom;
          player.fOffsetY += fMouseWorldY_BeforeZoom - fMouseWorldY_AfterZoom;
       }

       private void HandleMouseSelection(Player player)
       {
          if (!Input.GetMouseLeft()) return;
          var mousePos = Input.GetMousePos();
          float fMouseWorldX, fMouseWorldY;
          ScreenToWorld(
             mousePos.X,
             mousePos.Y,
             player,
             out fMouseWorldX,
             out fMouseWorldY);
          player.fSelectedTileX = (float) Math.Floor(fMouseWorldX);
          player.fSelectedTileY = (float) Math.Floor(fMouseWorldY);
       }
       
       private static void SetScreenOffset(Player player)
       {
          player.fOffsetX = (player.PPlayer.X - 4) * TileData.GetWidth();
          player.fOffsetY = (player.PPlayer.Y - 4)* TileData.GetHeight();
       }
    }
}