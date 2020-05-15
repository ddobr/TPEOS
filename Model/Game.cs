using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TPEOS.Model
{
    /// <summary>
    /// Класс с методами, определяющими логику игры и взаимодействие между объектами
    /// </summary>
    static class Game 
    {
        public static Field Field;
        public static Queue<Control> Controls = new Queue<Control>();
        public static GameStage Stage;
        public static Random RandomGenerator = new Random(36);

        public static void ProcessMove(Creature creature, CreatureCommand command)
        {
            //Переместить существо на карте
            if (!command.PostionsDelta.Equals(Point.Empty))
                Field.MoveCreature(creature.Location.Diff(command.PostionsDelta), command.PostionsDelta);
            //Если существо атаковало в каком то направлении, то обработать атаку
            if (command.Hit != null)
                ProcessHit(command.Hit);
            if (command.Spawn != null)
                ProcessSpawn(command.Spawn);
        }

        public static void ProcessPlayersMove(Creature creature, CreatureCommand command)
        {
            //проверка на наличие предметов в точке и передача их игроку 
            GiveCellsDropTo(creature);
            //Если существо атаковало в каком то направлении, то обработать атаку
            if (command.Hit != null)
                ProcessHit(command.Hit);
            if (command.Spawn != null)
                ProcessSpawn(command.Spawn);
        }

        public static bool IfCreatureIsDead(Creature creature)
        {
            //Убрать существо из игры если у него нет жизней и добавить дроп на карту
            if (creature.Health <= 0)
            {
                var dropLocation = creature.Location;
                dropLocation = Field.FindCorrectDropPoint(dropLocation);
                var drop = creature.GetDrop();
                Field.AddDrop(drop, dropLocation);
                Field.RemoveCreature(creature);
                return true;
            }

            return false;
        }

        public static void StartGame()
        {
            Field = new Field();
            Stage = GameStage.Started;
        }

        private static void GiveCellsDropTo(Creature creature)
        {
            if (!(creature is Player) || Field.DropsMap[creature.Location.X, creature.Location.Y] == Drop.None) return;
            switch (Field.DropsMap[creature.Location.X, creature.Location.Y])
            {
                case Drop.Bullet:
                    Field.Player.GetAmmo(10);
                    break;
                case Drop.Blocks:
                    Field.Player.GetBlocks(5);
                    break;
            }
            Field.RemoveDrop(creature.Location);
        }

        private static void ProcessHit(Hit hit)
        {
            var attackedPoint = hit.Sender.Location.Sum(hit.Direction);
            if (Field.CreaturesMap[attackedPoint.X, attackedPoint.Y] != null)
                Field.CreaturesMap[attackedPoint.X, attackedPoint.Y].DecreaseHealth(hit.HitPower);
            if (attackedPoint == Field.Player.Location)
                Field.Player.DecreaseHealth(hit.HitPower);
        }

        public static void ProcessSpawn(Creature spawnedCreature)
        {
            if (spawnedCreature is Bullet && Field.DoesContainsCreature(spawnedCreature.Location))
            {
                Field.CreaturesMap[spawnedCreature.Location.X, spawnedCreature.Location.Y].DecreaseHealth(spawnedCreature.HitPower);
                spawnedCreature.DecreaseHealth(spawnedCreature.Health);
                return;
            }
            Field.AddCreature(spawnedCreature);
            View.TpeosWindow.Timer.Tick += spawnedCreature.Tick;
        }

        public static void RemoveDeadCreatures()
        {
            Stage = IfCreatureIsDead(Field.Player) ? GameStage.Finished : GameStage.Playing;
            Field.CreaturesList =
                Field.CreaturesList.Where(creature => !IfCreatureIsDead(creature)).ToList();
        }

        public static void SpawnWalker()
        {
            var spawnLocation = new Point(RandomGenerator.Next(20), RandomGenerator.Next(20));
            if (RandomGenerator.Next() % 2 == 0)
            {
                spawnLocation.X = Field.Player.Location.X / ((Field.Size - 2) / 2) == 0 ? 0 : Field.Size - 1;
                while (Field.DoesContainsCreature(spawnLocation)) spawnLocation.Y = RandomGenerator.Next(20);
            }
            else
            {
                spawnLocation.Y = Field.Player.Location.Y / ((Field.Size - 2) / 2) == 0 ? 0 : Field.Size - 1;
                while (Field.DoesContainsCreature(spawnLocation)) spawnLocation.X = RandomGenerator.Next(20);
            }
            ProcessSpawn(new Walker(spawnLocation));
        }

        public static void SpawnShooter()
        {
            var spawnLocation = new Point(RandomGenerator.Next(20), RandomGenerator.Next(20));
            if (RandomGenerator.Next() % 2 == 0)
            {
                spawnLocation.X = Field.Player.Location.X / ((Field.Size - 2) / 2) == 0 ? 0 : Field.Size - 1;
                while (Field.DoesContainsCreature(spawnLocation)) spawnLocation.Y = RandomGenerator.Next(20);
            }
            else
            {
                spawnLocation.Y = Field.Player.Location.Y / ((Field.Size - 2) / 2) == 0 ? 0 : Field.Size - 1;
                while (Field.DoesContainsCreature(spawnLocation)) spawnLocation.X = RandomGenerator.Next(20);
            }
            ProcessSpawn(new Shooter(spawnLocation));
        }
    }

    public class Hit
    {
        public readonly Creature Sender;
        public readonly Point Direction;
        public readonly int HitPower;

        public Hit(Creature sender, Point direction, int hitPower)
        {
            Sender = sender;
            Direction = direction;
            HitPower = hitPower;
        }
    }
}