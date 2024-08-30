using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _240701_5TreeMake
{
    class Revive
    {
        public int X { get; set; }
        public int Y { get; set; }  
        public STONE Stone { get; set; }
        public int Seq { get; set; }    
        public Revive(int x, int y, STONE s, int seq) 
        {
            this.X = x;
            this.Y = y;
            this.Seq = seq;
            this.Stone = s;
        }
    }
}
