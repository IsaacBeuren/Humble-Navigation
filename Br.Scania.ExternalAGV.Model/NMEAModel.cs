using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Model
{
    public class NMEAModel
    {
        public int Type { get; set; }
        public GGAModel GGAModel { get; set; }
        public ZDAModel ZDAModel { get; set; }
        public GSTModel GSTModel { get; set; }

    }
}
