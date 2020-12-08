using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL;
using Domain;
using Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Battleship
{
    public class NewGame : PageModel
    {
	    [Required]
	    [Range(10, 100)]
	    [Display(Name="Board Width")]
	    [BindProperty]
	    public int BoardWidth { get; set; } = 10;

	    [Required]
	    [Range(10, 100)]
	    [Display(Name = "Board Height")]
	    [BindProperty]
	    public int BoardHeight { get; set; } = 10;
	    [Required]
	    [Display(Name="Allowed Placement Type")]
	    [BindProperty]
	    public int AllowedPlacementType { get; set; } = 0;
	    [Required]
	    [Display(Name="Ships")]
	    [BindProperty] public string Ships { get; set; } = "1x5n1; 1x4n2; 1x3n3; 1x2n4";
	    
	    public string ErrorMsg = "                     ";

	    public void OnGet()
        {

        }

        public ActionResult OnPost()
        {
	        if (!ModelState.IsValid)
	        {
		        return Page();
	        }
	        string errorMsgText = "";
	        RuleSet? menuResult;
	        if (!Game.Utils.TryBtnStart(Ships, BoardWidth, BoardHeight, AllowedPlacementType, ref errorMsgText, out menuResult))
	        {
		        ErrorMsg = errorMsgText;
				return Page();
	        }
	        if (menuResult == null) { throw new Exception("unexpected!");}
	        BaseBattleship game = new WebBattle(
		        menuResult.BoardHeight, 
		        menuResult.BoardWidth, 
		        menuResult.Ships,
		        menuResult.AllowedPlacementType, 
		        -1, 
		        -1);
	        string id = DbQueries.Save(game.GameData);
	        
	        return RedirectToPage("GameView", new { id = id, delete = true });
        }
    }
}