using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPEOS.Model
{
    class Walker : Creature
    {
        public int MoveTimeRemains { get; private set; }

        public override Drop GetDrop()
        {
            return Drop.Blocks;
        }

        public override CreatureCommand Act()
        {
            if (MoveTimeRemains != 0) return new CreatureCommand(Point.Empty);
            MoveTimeRemains += ModelConstants.WalkerSpeed;
            if (WalkerDijkstraAlgorithm.TryGetPathsDelta(Location, Game.Field.Player.Location, out var delta))
            {
                var nextPoint = delta.Sum(Location);
                if (Game.Field.Player.Location == nextPoint || Game.Field.DoesContainsCreature(nextPoint))
                    return new CreatureCommand(Point.Empty, Attack(delta));
                Location = nextPoint;
                return new CreatureCommand(delta);
            }
            delta = FindDeltaSimply(Game.Field.Player.Location);
            var nextLocation = delta.Sum(Location);
            if (Game.Field.Player.Location == nextLocation || Game.Field.DoesContainsCreature(nextLocation))
                return new CreatureCommand(Point.Empty, Attack(delta));
            Location = nextLocation;
            return new CreatureCommand(delta);
        }

        public override Hit Attack(Point delta)
        {
            var point = delta.Sum(Location);
            return new Hit(this, delta, Game.Field.CreaturesMap[point.X, point.Y] is Block ? 1 : HitPower);
        }

        public override void Tick(object sender, EventArgs args)
        {
            if (MoveTimeRemains != 0) MoveTimeRemains--;
        }

        public Walker(Point location)
        {
            Location = location;
            Health = ModelConstants.WalkerInitialHealth;
            HitPower = ModelConstants.WalkerHitPower;
        }

        public Point FindDeltaSimply(Point target)
        {
            if (target.X > Location.X) return new Point(1, 0);
            if (target.X < Location.X) return new Point(-1, 0);
            if (target.Y > Location.Y) return new Point(0, 1);
            if (target.Y < Location.Y) return new Point(0, -1);
            return Point.Empty;
        }
    }

    public static class WalkerDijkstraAlgorithm
    {
        private static Point nullPoint = new Point(-1, -1);
        public static bool TryGetPathsDelta(Point start, Point end, out Point delta)
        {
            delta = Point.Empty;
            var notVisited = new HashSet<Point>();
            var visited = new HashSet<Point>();
            notVisited.Add(start);
            var track = new Dictionary<Point, DijkstraData> {[start] = new DijkstraData(0, nullPoint)};

            while (true)
            {
                var toOpen = nullPoint;
                var bestPrice = int.MaxValue;
                foreach (var e in notVisited)
                    if (track.ContainsKey(e) && track[e].Price < bestPrice)
                    {
                        bestPrice = track[e].Price;
                        toOpen = e;
                    }

                if (toOpen == nullPoint) return false;
                if (toOpen == end) break;

                foreach (var e in toOpen.IncidentPoints()
                    .Where(x => Game.Field.IsInBounds(x) && (!Game.Field.DoesContainsCreature(x) || Game.Field.CreaturesMap[x.X, x.Y] is Block)))
                {
                    var currentPrice = track[toOpen].Price + (Game.Field.CreaturesMap[e.X, e.Y] is Block ? ModelConstants.BlockInitialHealth : 1);
                    var nextPoint = e;
                    if (!visited.Contains(e)) notVisited.Add(e);
                    if (!track.ContainsKey(nextPoint) || track[nextPoint].Price > currentPrice)
                        track[nextPoint] = new DijkstraData(currentPrice, toOpen);
                }

                visited.Add(toOpen);
                notVisited.Remove(toOpen);
            }

            var target = end;
            while (track[target].Previous != start)
                target = track[target].Previous;

            delta = target.Diff(start);
            return true;
        }
    }
}
