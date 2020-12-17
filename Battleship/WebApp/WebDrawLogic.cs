using System;
using System.Collections.Generic;
using Domain;
using Domain.Model;
using Domain.Tile;
using Game;
using RogueSharp;

namespace WebApp
{
    public static class WebDrawLogic
    {
        public static TileData.CharInfo[,] GetDraw(GameData gameData)
        {
            gameData.FrameCount++;
            
            TileData.CharInfo[,] map = new TileData.CharInfo[40, 40];
            BaseDraw.GetDrawArea(gameData, ref map);
            return map;
        }
    }
}