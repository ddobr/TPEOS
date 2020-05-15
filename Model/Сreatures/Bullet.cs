using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPEOS.Model
{
    class Bullet : Creature
    {
        public readonly Point Direction;
        public int WalkTimeRemains { get; private set; }

        public override Drop GetDrop()
        {
            return Drop.None;
        }

        public override CreatureCommand Act()
        {
            var nextLocation = Location.Sum(Direction);
            if (Game.Field.IsInBounds(nextLocation) && WalkTimeRemains == 0)
            {
                if (Game.Field.DoesContainsCreature(nextLocation) || Game.Field.Player.Location == nextLocation)
                {
                    Health = 0;
                    return new CreatureCommand(Point.Empty, Attack(Direction));
                }
                WalkTimeRemains += ModelConstants.BulletSpeed;
                Location = nextLocation;
                return new CreatureCommand(Direction);
            }
            if(!Game.Field.IsInBounds(nextLocation))
                Health = 0;
            return new CreatureCommand(Point.Empty);
        }

        public override Hit Attack(Point delta)
        {
            return new Hit(this, delta, HitPower);
        }

        public override void Tick(object sender, EventArgs args)
        {
            if (WalkTimeRemains != 0) WalkTimeRemains--;
        }

        public Bullet(Point shooterLocation, Point direction)
        {
            this.Direction = direction;
            Health = ModelConstants.BulletHealth;
            WalkTimeRemains = ModelConstants.BulletSpeed;
            Location = shooterLocation.Sum(direction);
            HitPower = ModelConstants.BulletHitPower;
        }
    }
}
