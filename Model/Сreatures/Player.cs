using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPEOS.Model
{
    class Player : Creature
    {
        public int BlocksAmount { get; private set; }
        public int Ammo { get; private set; } = ModelConstants.InitialPlayerAmmo;
        public bool DoesHoldWeapon { get; private set; } = true;
        public int WalkTimeRemains { get; private set; }
        public int ShootTimeRemains { get; private set; }
        public Dictionary<Control, Func<Point, CreatureCommand>> Actions;
        public Dictionary<Control, Point> Deltas;

        public CreatureCommand Act(Control control)
        {
            if (control == Control.None) return new CreatureCommand(Point.Empty);
            if (control == Control.SwapAction)
            {
                DoesHoldWeapon = !DoesHoldWeapon;
                return new CreatureCommand(Point.Empty);
            }
            return Actions[control](Deltas[control]);
        }

        public override Hit Attack(Point delta)
        {
            return new Hit(this, delta, HitPower);
        }

        public void IncreaseHealth(int health)
        {
            if (health < 0) throw new ArgumentException();
            Health += health;
        }

        public void GetAmmo(int ammo)
        {
            if (ammo < 0) throw new ArgumentException();
            Ammo += ammo;
        }

        public void GetBlocks(int blocks)
        {
            if (blocks < 0) throw new ArgumentException();
            BlocksAmount += blocks;
        }

        public override void Tick(object sender, EventArgs args)
        {
            if (ShootTimeRemains != 0) ShootTimeRemains--;
            if (WalkTimeRemains != 0) WalkTimeRemains--;
        }

        public CreatureCommand GoCommand(Point delta)
        {
            var newLocation = Location.Sum(delta);
            if (Game.Field.IsInPlayersBounds(newLocation) && !Game.Field.DoesContainsCreature(newLocation) &&
                WalkTimeRemains == 0)
            {
                Location = newLocation;
                WalkTimeRemains += ModelConstants.PlayerWalkSpeed;
                return new CreatureCommand(delta);
            }
            return new CreatureCommand(Point.Empty);
        }
        
        public CreatureCommand ActionCommand(Point delta)
        {
            if (DoesHoldWeapon)
            {
                if (Ammo == 0 || ShootTimeRemains != 0) return new CreatureCommand(Point.Empty, Attack(delta));
                var bullet = new Bullet(Location, delta);
                Ammo--;
                ShootTimeRemains += ModelConstants.PlayerShootSpeed;
                return new CreatureCommand(Point.Empty, bullet);
            }
            if (BlocksAmount != 0 && Game.Field.IsInPlayersBounds(Location.Sum(delta)) &&
                !Game.Field.DoesContainsCreature(Location.Sum(delta)))
            {
                var block = new Block(Location.Sum(delta));
                BlocksAmount--;
                return new CreatureCommand(Point.Empty, block);
            }
            return new CreatureCommand(Point.Empty);
        }

        public override Drop GetDrop()
        {
            return Drop.None;
        }

        public override CreatureCommand Act()
        {
            return Act(Control.None);
        }

        public Player(Point initLocation)
        {
            HitPower = ModelConstants.PlayerHitPower;
            Location = initLocation;
            Health = ModelConstants.PlayerHealth;
            BlocksAmount = ModelConstants.InitialPlayerBlocks;

            Actions = new Dictionary<Control, Func<Point, CreatureCommand>>
            {
                {Control.GoUp, GoCommand},
                {Control.GoDown, GoCommand},
                {Control.GoLeft, GoCommand},
                {Control.GoRight, GoCommand},
                {Control.ActionUp, ActionCommand},
                {Control.ActionDown, ActionCommand},
                {Control.ActionLeft, ActionCommand},
                {Control.ActionRight, ActionCommand}
            };

            Deltas = new Dictionary<Control, Point>
            {
                {Control.GoUp, new Point(0, -1)},
                {Control.GoDown, new Point(0, 1)},
                {Control.GoLeft, new Point(-1, 0)},
                {Control.GoRight, new Point(1, 0)},
                {Control.ActionUp, new Point(0, -1)},
                {Control.ActionDown, new Point(0, 1)},
                {Control.ActionLeft, new Point(-1, 0)},
                {Control.ActionRight, new Point(1, 0)}
            };
        }
    }
}
