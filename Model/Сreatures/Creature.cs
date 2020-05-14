using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPEOS.Model
{
    public abstract class Creature
    {
        public int Health { get; protected set; }
        public Point Location { get; protected set; }
        public int HitPower { get; protected set; }
        public abstract Drop GetDrop();
        public abstract CreatureCommand Act();
        public abstract Hit Attack(Point delta);
        public abstract void Tick(object sender, EventArgs args);
        public virtual void DecreaseHealth(int damage)
        { 
            if (damage< 0) throw new ArgumentException();
            if (damage < Health)
                Health -= damage;
            else Health = 0;
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Creature objAsCreature)) return false;
            return Equals(objAsCreature);
        }

        public bool Equals(Creature other)
        {
            if (other == null) return false;
            return (Location.Equals(other.Location) && Health.Equals(other.Health));
        }
    }
}