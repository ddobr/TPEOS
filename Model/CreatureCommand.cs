using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPEOS.Model
{
    public class CreatureCommand
    {
        public Point PostionsDelta;
        public Creature Spawn;
        public Hit Hit;

        public CreatureCommand(Point delta)
        {
            PostionsDelta = delta;
        }

        public CreatureCommand(Point delta, Creature spawn)
        {
            PostionsDelta = delta;
            Spawn = spawn;
        }

        public CreatureCommand(Point delta, Hit hit)
        {
            PostionsDelta = delta;
            Hit = hit;
        }
    }
}
