using System.Collections.Generic;
using Game.Tile;
using RogueSharp;

namespace Game
{
    public class DrawLogicData
    {
        public int PlayerTileValue = TileData.SelectedTileGreen.exponent;
        public string Message = "";
        public readonly List<DialogItem> DialogOptions = new List<DialogItem>();
        public struct DialogItem
        {
            public bool isActive;
            public readonly string key;
            public readonly string text;

            public DialogItem(bool isActive, string key, string text)
            {
                this.isActive = isActive;
                this.key = key;
                this.text = text;
            }

            public DialogItem SetActive(bool b)
            {
                isActive = b;
                return this;
            }
        }
    }
    

}