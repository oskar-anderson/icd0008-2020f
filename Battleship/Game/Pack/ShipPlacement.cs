using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Troschuetz.Random.Generators;
using Rectangle = RogueSharp.Rectangle;
using Point = RogueSharp.Point;

namespace Game.Pack
{
    public static class ShipPlacement
    {
        public static List<Rectangle> PlaceShips(List<Point> shipsSizesToPlaceOrig, int x, int y, int placementType)
        {
            if (! new int[] {0, 1, 2}.Contains(placementType))
            {
                throw new Exception("Unknown placementType!");
            }
            List<Point> shipsSizesToPlace = shipsSizesToPlaceOrig.Select(p => new Point(p.X, p.Y)).ToList();
            shipsSizesToPlace = shipsSizesToPlace.OrderByDescending(r => r.X * r.Y).ToList();
            RandomizeRot(shipsSizesToPlace);
            List<Rectangle> packedRects;
            bool isPacked = TryPackShip(shipsSizesToPlace, x, y, placementType, out packedRects);
            if (!isPacked)
            {
                throw new Exception("Packing failed, unable to place ships");
            }
            
            
            List<Rectangle> placedShipsShuffled = packedRects.ConvertAll(rect => new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
            for (int unusedShuffleI = 0; unusedShuffleI < 10; unusedShuffleI++)
            {
                for (int idx1 = 0; idx1 < placedShipsShuffled.Count; idx1++)
                {
                    Rectangle rectTmp = placedShipsShuffled[idx1];
                    for (int unusedAttemptJ = 0; unusedAttemptJ < 10; unusedAttemptJ++)
                    {
                        Rectangle rectToPlace = new Rectangle(
                            RandomGen(x - rectTmp.Width + 1), RandomGen(y - rectTmp.Height + 1),
                            rectTmp.Width, rectTmp.Height);
                        List<Point> rectToPlacePoints = rectToPlace.ToHitboxPoints(placementType);
                        bool anyIntersects = PlacedShipsIntersectToPlace(placedShipsShuffled, idx1, rectToPlacePoints);
                        if (anyIntersects)
                        {
                            continue;
                        }
                        placedShipsShuffled[idx1] = rectToPlace;
                        break;
                        
                    }
                }
            }

            return placedShipsShuffled;
        }

        private static bool PlacedShipsIntersectToPlace(List<Rectangle> placedShipsShuffled, int rectToPlaceIdx, List<Point> rectToPlacePoints)
        {
            for (int idx2 = 0; idx2 < placedShipsShuffled.Count; idx2++)
            {
                Rectangle rectInPlace = placedShipsShuffled[idx2];
                if (idx2 == rectToPlaceIdx)
                {
                    continue;
                }
                if (rectToPlacePoints.Any(rectToPlacePoint => rectInPlace.Contains(rectToPlacePoint)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryPackShip(List<Point> shipsSizesToPlace, int x, int y, int placementType, out List<Rectangle> packedRects)
        {
            ArevaloRectanglePacker packer = new ArevaloRectanglePacker(x, y, placementType);
            packedRects = new List<Rectangle>();
            foreach (var shipSize in shipsSizesToPlace)
            {
                System.Drawing.Point point;
                bool isPossible = packer.TryPack(shipSize.X, shipSize.Y, out point);
                
                if (! isPossible)
                {
                    return false;
                }
                Rectangle ship = new Rectangle(new Point(point.X, point.Y), new Point(shipSize.X, shipSize.Y));
                packedRects.Add(ship);
            }
            return true;
        }
        
        private static int RandomGen(int max)
        {
            XorShift128Generator generator = new XorShift128Generator();
            return generator.Next() % max;
        }

        private static void RandomizeRot(List<Point> shipsToPlace)
        {
            for (int i = 0; i < shipsToPlace.Count; i++)
            {
                if (RandomGen(2) % 2 != 0) continue;
                var ship = shipsToPlace[i];
                shipsToPlace[i] = new Point(ship.Y, ship.X);
            }
        }
    }
}