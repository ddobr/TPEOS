using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TPEOS.Model
{
    class GameState
    {
        public int WalkerSpawnTimeRemains;
        public int ShooterSpawnTimeRemains;
        public int FoodSpawnTimeRemains;

        public void GameTick(object sender, EventArgs e)
        {
            if (Game.Stage == GameStage.Playing)
            {
                ProcessActs();
                DecreaseSpawnTimeRemaining();
                ProcessSpawns();
            }
        }
        
        public void ProcessActs()
        {
            Game.RemoveDeadCreatures();
            if (Game.Controls.Count == 0)
                Game.Controls.Enqueue(Control.None);
            var command = Game.Field.Player.Act(Game.Controls.Dequeue());
            Game.ProcessPlayersMove(Game.Field.Player, command);
            for (var i = 0; i < Game.Field.CreaturesList.Count; i++)
            {
                command = Game.Field.CreaturesList[i].Act();
                Game.ProcessMove(Game.Field.CreaturesList[i], command);
            }
        }

        public void DecreaseSpawnTimeRemaining()
        {
            if (WalkerSpawnTimeRemains != 0) WalkerSpawnTimeRemains--;
            if (ShooterSpawnTimeRemains != 0) ShooterSpawnTimeRemains--;
            if (FoodSpawnTimeRemains != 0) FoodSpawnTimeRemains--;
        }

        public void ProcessSpawns()
        {
            if (WalkerSpawnTimeRemains == 0)
            {
                //Game.SpawnWalker();
                WalkerSpawnTimeRemains += ModelConstants.WalkerSpawnerCooldown;
            }

            if (ShooterSpawnTimeRemains == 0)
            {
                Game.SpawnShooter();
                ShooterSpawnTimeRemains += ModelConstants.ShooterSpawnerCooldown;
            }
            
        }
    }
}
