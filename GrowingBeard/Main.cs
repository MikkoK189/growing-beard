using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace GrowingBeard
{
    public class Main : MBSubModuleBase
    {

         
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            Campaign campaign = game.GameType as Campaign;
            if (campaign == null) return;
            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
            AddBehaviors(gameInitializer);
            AddGameModels(gameInitializer);
        }

        private void AddGameModels(CampaignGameStarter gameInitializer)
        {
            //   gameInitializer.AddModel(new DefaultAgeModel());
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
        }

        private void AddBehaviors(CampaignGameStarter gameInitializer)
        {
            gameInitializer.AddBehavior(new GrowingBeard());
        }
    }
}
