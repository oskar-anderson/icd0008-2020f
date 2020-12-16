using Domain.Model;
using Game;
using IrrKlang;
using RogueSharp;

namespace WebApp
{
    public class WebBattle : BaseBattleship
    {
        public WebBattle(GameData gameData) : base(gameData)
        {
          
        }

        public WebBattle(int boardHeight, int boardWidth, string ships, int allowAdjacentPlacement, int startingPlayerType, int secondPlayerType)
            : base(boardHeight, boardWidth, ships, allowAdjacentPlacement, startingPlayerType, secondPlayerType)
        {
          
        }

        public override void Initialize()
        {
            const SoundEngineOptionFlag options = 
                SoundEngineOptionFlag.Use3DBuffers | 
                SoundEngineOptionFlag.MultiThreaded | 
                // SoundEngineOptionFlag.PrintDebugInfoIntoDebugger |
                // SoundEngineOptionFlag.PrintDebugInfoToStdOut | 
                SoundEngineOptionFlag.LoadPlugins;
            SoundEngine = new ISoundEngine(SoundOutputDriver.AutoDetect, options);
            Input = new WebInput();
            UpdateLogicExitEvent = () => { return;};
            UpdateLogic = new UpdateLogic(UpdateLogicExitEvent, Input, SoundEngine);
        }

        public override void Draw(double gameTime, GameData data)
        {
            // this cannot be used
            throw new System.NotImplementedException();
        }
    }
}