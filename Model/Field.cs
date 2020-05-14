using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TPEOS.Model
{
    class Field
    {
        public const int Size = 20;
        public Player Player;
        public Creature[,] CreaturesMap { get; set; }
        public List<Creature> CreaturesList { get; set; }
        public Drop[,] DropsMap  { get; set; }

        public Field(int size = Size)
        {
            if (size < 3) throw new ArgumentException();
            CreaturesMap = new Creature[size, size];
            DropsMap = new Drop[size, size];
            CreaturesList = new List<Creature>();
            Player = new Player(new Point(size / 2, size / 2));
        }

        public void MoveCreature(Point startPoint, Point delta)
        {
            var newPosition = startPoint.Sum(delta);
            CreaturesMap[newPosition.X, newPosition.Y] = CreaturesMap[startPoint.X, startPoint.Y];
            CreaturesMap[startPoint.X, startPoint.Y] = null;
        }

        public void AddCreature(Creature creature)
        {
            CreaturesMap[creature.Location.X, creature.Location.Y] = creature;
            CreaturesList.Add(creature);
        }

        public void AddDrop(Drop drop, Point location)
        {
            DropsMap[location.X, location.Y] = drop;
        }

        public void RemoveCreature(Creature creature)
        {
            CreaturesMap[creature.Location.X, creature.Location.Y] = null;
        }

        public void RemoveDrop(Point location)
        {
            DropsMap[location.X, location.Y] = Drop.None;
        }

        public bool IsInBounds(Point point)
        {
            if (point.X < 0 || point.X + 1 > CreaturesMap.GetLength(0)) return false;
            if (point.Y < 0 || point.Y + 1 > CreaturesMap.GetLength(1)) return false;
            return true;
        }

        public bool IsInPlayersBounds(Point point)
        {
            if (point.X < 1 || point.X + 2 > CreaturesMap.GetLength(0)) return false;
            if (point.Y < 1 || point.Y + 2 > CreaturesMap.GetLength(1)) return false;
            return true;
        }

        public bool DoesContainsCreature(Point point)
        {
            return CreaturesMap[point.X, point.Y] != null || Game.Field.Player.Location == point;
        }

        public Point FindCorrectDropPoint(Point point)
        {
            var result = point;
            if (result.X == 0)
                result.X++;
            else if(result.X == Size - 1)
                result.X--;
            if (result.Y == 0)
                result.Y++;
            else if (result.Y == Size - 1)
                result.Y--;
            return result;
        }


    }
}
