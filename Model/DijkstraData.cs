using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPEOS.Model
{
    public class DijkstraData
    {
        public Point Previous { get; set; }
        public int Price { get; set; }

        public DijkstraData(int price, Point previous)
        {
            Previous = previous;
            Price = price;
        }
    }
}
