using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Model
{
    public class ManualMoviments
    {
        public bool Foward { get; set; }
        public bool Reverse { get; set; }
        public bool Right { get; set; }
        public bool Left { get; set; }
        public int Velocity { get; set; }

    }
}
