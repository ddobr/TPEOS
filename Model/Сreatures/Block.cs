using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPEOS.Model
{
    class Block : Creature
    {
        public override Drop GetDrop()
        {
            return Drop.None;
        }

        public override CreatureCommand Act()
        {
            return new CreatureCommand(new Point(0, 0));
        }

        public override Hit Attack(Point delta)
        {
            return null;
        }

        public override void Tick(object sender, EventArgs args)
        { }

        public Block(Point location)
        {
            Location = location;
            Health = ModelConstants.BlockInitialHealth;
        }
    }
}
