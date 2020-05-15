using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPEOS.Model
{
    class Shooter : Creature
    {
        public int ShootTimeRemains;
        public int WalkTimeRemains;
        
        public override Drop GetDrop()
        {
            return Drop.Bullet;
        }

        public override CreatureCommand Act()
        {
            if (WalkTimeRemains == 0 && TryDodge(out var nextLocation))
            {
                var delta = nextLocation.Diff(Location);
                Location = nextLocation;
                WalkTimeRemains += ModelConstants.ShooterWalkSpeed;
                return new CreatureCommand(delta);
            }

            if (ShootTimeRemains == 0 && IsTargetOnAxis(Game.Field.Player.Location))
            {
                var shootDelta = GetUnitVector(Game.Field.Player.Location.Diff(Location));
                ShootTimeRemains += ModelConstants.ShooterShootSpeed;
                return new CreatureCommand(Point.Empty, new Bullet(Location, shootDelta));
            }

            if (WalkTimeRemains == 0)
            {
                var delta = Point.Empty;
                var minLength = Game.Field.CreaturesMap.Length;
                foreach (var position in GetShootPositions(Game.Field.Player.Location, 8))
                {
                    if (!ShooterDijkstraAlgorithm.TryGetPathsDelta(Location, position, out var deltaPoint, out var length)) continue;
                    if (length < minLength)
                    {
                        minLength = length;
                        delta = deltaPoint;
                    }
                }
                if (delta != Point.Empty)
                {
                    WalkTimeRemains += ModelConstants.ShooterWalkSpeed;
                    Location = Location.Sum(delta);
                    return new CreatureCommand(delta);
                }
            }

            return new CreatureCommand(Point.Empty);
        }

        public override Hit Attack(Point delta)
        {
            return new Hit(this, delta, HitPower);
        }

        public override void Tick(object sender, EventArgs args)
        {
            if (ShootTimeRemains != 0) ShootTimeRemains--;
            if (WalkTimeRemains != 0) WalkTimeRemains--;
        }

        public Shooter(Point location)
        {
            Location = location;
            Health = ModelConstants.ShooterInitialHealth;
            HitPower = ModelConstants.ShooterHitPower;
        }

        public Point[] GetShootPositions(Point target, int distance)
        {
            var result = new List<Point> {new Point(0, -1), new Point(0, 1), new Point(-1, 0), new Point(1, 0)};
            result = result.Where(p =>
                Game.Field.IsInBounds(p.Sum(target)) && !Game.Field.DoesContainsCreature(p.Sum(target))).ToList();
            for (var i = 0; i < distance - 1; i++)
                result = result.Select(p =>
                        Game.Field.IsInBounds(p.Sum(GetUnitVector(p)).Sum(target)) &&
                        !Game.Field.DoesContainsCreature(p.Sum(GetUnitVector(p)).Sum(target))
                            ? p.Sum(GetUnitVector(p))
                            : p)
                    .ToList();
            return result.Select(p => p.Sum(target)).ToArray();
        }

        public bool IsTargetOnAxis(Point target)
        {
            var delta = target.Diff(Location);
            return delta.X == 0 || delta.Y == 0;
        }

        public bool TryDodge(out Point dodgePoint)
        {
            dodgePoint = Point.Empty;
            for(var x = 0; x < Game.Field.CreaturesMap.GetLength(0); x++)
                if (Game.Field.CreaturesMap[x, Location.Y] is Bullet &&
                    DoesApproaching((Bullet) Game.Field.CreaturesMap[x, Location.Y]))
                {
                    if (Game.Field.IsInBounds(Location.Sum(new Point(0, -1))) &&
                        !Game.Field.DoesContainsCreature(Location.Sum(new Point(0, -1))))
                    {
                        dodgePoint = Location.Sum(new Point(0, -1));
                        return true;
                    }
                    if (Game.Field.IsInBounds(Location.Sum(new Point(0, 1))) &&
                        !Game.Field.DoesContainsCreature(Location.Sum(new Point(0, 1))))
                    {
                        dodgePoint = Location.Sum(new Point(0, 1));
                        return true;
                    }
                }

            for (var y = 0; y < Game.Field.CreaturesMap.GetLength(1); y++)
                if (Game.Field.CreaturesMap[Location.X, y] is Bullet &&
                    DoesApproaching((Bullet)Game.Field.CreaturesMap[Location.X, y]))
                {
                    if (Game.Field.IsInBounds(Location.Sum(new Point(-1, 0))) &&
                        !Game.Field.DoesContainsCreature(Location.Sum(new Point(-1, 0))))
                    {
                        dodgePoint = Location.Sum(new Point(-1, 0));
                        return true;
                    }
                    if (Game.Field.IsInBounds(Location.Sum(new Point(1, 0))) &&
                        !Game.Field.DoesContainsCreature(Location.Sum(new Point(1, 0))))
                    {
                        dodgePoint = Location.Sum(new Point(1, 0));
                        return true;
                    }
                }

            return false;
        }

        public bool DoesApproaching(Bullet bullet)
        {
            if (!IsTargetOnAxis(bullet.Location)) return false;
            return (bullet.Location.X < Location.X && bullet.Direction.X > 0) ||
                   (bullet.Location.X > Location.X && bullet.Direction.X < 0) ||
                   (bullet.Location.Y < Location.Y && bullet.Direction.Y > 0) ||
                   (bullet.Location.Y > Location.Y && bullet.Direction.Y < 0);
        }

        public static Point GetUnitVector(Point delta)
        {
            delta.X = delta.X == 0 ? 0 : delta.X / Math.Abs(delta.X);
            delta.Y = delta.Y == 0 ? 0 : delta.Y / Math.Abs(delta.Y);
            return delta;
        }
    }

    public static class ShooterDijkstraAlgorithm
    {
        private static Point nullPoint = new Point(-1, -1);
        public static bool TryGetPathsDelta(Point start, Point end, out Point delta, out int length)
        {
            delta = Point.Empty;
            length = 0;
            if (start == end) return false;
            var notVisited = new HashSet<Point>();
            var visited = new HashSet<Point>();
            notVisited.Add(start);
            var track = new Dictionary<Point, DijkstraData> { [start] = new DijkstraData(0, nullPoint) };

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
                    .Where(x => Game.Field.IsInBounds(x) && !Game.Field.DoesContainsCreature(x)))
                {
                    var currentPrice = track[toOpen].Price + 1;
                    var nextPoint = e;
                    if (!visited.Contains(e)) notVisited.Add(e);
                    if (!track.ContainsKey(nextPoint) || track[nextPoint].Price > currentPrice)
                        track[nextPoint] = new DijkstraData(currentPrice, toOpen);
                }

                visited.Add(toOpen);
                notVisited.Remove(toOpen);
            }

            var target = end;
            length = track[target].Price;
            while (track[target].Previous != start)
                target = track[target].Previous;

            delta = target.Diff(start);
            return true;
        }
    }
}
