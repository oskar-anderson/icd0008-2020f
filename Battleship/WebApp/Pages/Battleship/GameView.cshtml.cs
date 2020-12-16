using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using DAL;
using Domain;
using Domain.Model;
using Domain.Tile;
using Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using RogueSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApp.Pages.Battleship
{
    public class GameView : PageModel
    {
        [BindProperty]
        public string GameDataSerialized { get; set; } = null!;
        [BindProperty]
        public bool FormActionSave { get; set; } = false;
        [BindProperty, Required] 
        public string KeyPress { get; set; } = null!;
        public TileData.CharInfo[,] GameBoard { get; set; } = null!;
        public GameData GameData { get; set; } = null!;

        public readonly Dictionary<int, string> ColorIntToCssString = new Dictionary<int, string>()
        {
            { 0, "rgb(12,     12,     12)" },             // Black
            { 1, "rgb(0,      55,     218)" },            // DarkBlue
            { 2, "rgb(19,     161,    14)" },             // DarkGreen
            { 3, "rgb(58,     150,    221)" },            // DarkCyan
            { 4, "rgb(197,    15,     31)" },             // DarkRed
            { 5, "rgb(136,    23,     152)" },            // DarkMagenta
            { 6, "rgb(193,    156,    0)" },              // DarkYellow
            { 7, "rgb(204,    204,    204)" },            // Gray
            { 8, "rgb(118,    118,    118)" },            // DarkGrey
            { 9, "rgb(59,     120,    255)" },            // Blue
            { 10, "rgb(22,    192,    12)" },             // Green
            { 11, "rgb(97,    214,    214)" },            // Cyan
            { 12, "rgb(231,   72,     86)" },             // Red
            { 13, "rgb(180,   0,      158)" },            // Magenta
            { 14, "rgb(249,   241,    165)" },            // Yellow
            { 15, "rgb(242,   242,    242)" },            // White
        };

        private readonly AppDbContext _ctx;
        
        public GameView(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult OnGet(string id, bool delete)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (id == null)
            {
                return RedirectToPage("ErrorPage");
            }
            GameData? gameData = DbGameData.ToGameModel(_ctx.GameData
                .Include(d => d.ActivePlayer)
                .Include(d => d.InactivePlayer)
                .FirstOrDefault(m => m.ID == id));
            if (gameData == null)
            {
                return RedirectToPage("ErrorPage");
            }
            if (delete)
            {
                DbQueries.Delete(id);
            }

            BaseBattleship game = new WebBattle(gameData);
            
            game.Initialize();
            DoGame(game);
            GameData = game.GameData;
            GameDataSerializable gameDataSerializableSave = new GameDataSerializable(game.GameData);
            GameDataSerialized = JsonSerializer.Serialize(gameDataSerializableSave, new JsonSerializerOptions() { WriteIndented = true });
            return Page();
        }

        public IActionResult OnPost()
        {
            if (FormActionSave)
            {
                return OnPostSave();
            }
            GameDataSerializable gameDataSerializableLoad = JsonSerializer.Deserialize<GameDataSerializable>(GameDataSerialized);
            GameData gameData = GameDataSerializable.ToGameModelSerializable(gameDataSerializableLoad);
            BaseBattleship game = new WebBattle(gameData);
            game.Initialize();
            
            switch (KeyPress)
            {
                case "LEFT":
                    game.Input.KeyStatuses[UsedKeyKeys.A] = new KeyStatus(true, true);
                    break;
                
                case "RIGHT":
                    game.Input.KeyStatuses[UsedKeyKeys.D] = new KeyStatus(true, true);
                    break;
                
                case "UP":
                    game.Input.KeyStatuses[UsedKeyKeys.W] = new KeyStatus(true, true);
                    break;
                
                case "DOWN":
                    game.Input.KeyStatuses[UsedKeyKeys.S] = new KeyStatus(true, true);
                    break;
                
                case "Z":
                    game.Input.KeyStatuses[UsedKeyKeys.Z] = new KeyStatus(true, true);
                    break;
                
                case "X":
                    game.Input.KeyStatuses[UsedKeyKeys.X] = new KeyStatus(true, true);
                    break;
                
                case "d1":
                    game.Input.KeyStatuses[UsedKeyKeys.D1] = new KeyStatus(true, true);
                    break;
                
                case "d2":
                    game.Input.KeyStatuses[UsedKeyKeys.D2] = new KeyStatus(true, true);
                    break;
                
                case "d3":
                    game.Input.KeyStatuses[UsedKeyKeys.D3] = new KeyStatus(true, true);
                    break;
                
                case "takeBack":
                    game.Input.KeyStatuses[UsedKeyKeys.R] = new KeyStatus(true, true);
                    break;
                default:
                    throw new Exception("unexpected!");
            }

            DoGame(game);
            GameData = game.GameData;
            GameDataSerializable gameDataSerializableSave = new GameDataSerializable(game.GameData);
            GameDataSerialized = JsonSerializer.Serialize(gameDataSerializableSave, new JsonSerializerOptions() { WriteIndented = true });
            return Page();
        }

        private void DoGame(BaseBattleship game)
        {
            game.Update(1d, game.GameData);
            GameBoard = WebDrawLogic.GetDraw(game.GameData);
        }

        public IActionResult OnPostSave()
        {
            GameDataSerializable gameDataSerializableLoad = JsonSerializer.Deserialize<GameDataSerializable>(GameDataSerialized);
            GameData gameData = GameDataSerializable.ToGameModelSerializable(gameDataSerializableLoad);
            
            DbQueries.Save(gameData);
            return RedirectToPage("./Index");
        }
    }
}