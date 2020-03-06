using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Model
{
    public class ZDAModel
    {
        public int Type { get; set; }
        public string Message_ID { get; set; }
        public string UTC { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? LTZ_Hour { get; set; }
        public int? LTZ_Minutes { get; set; }
        public string The_checksum_data { get; set; }
        public DateTime? DateConverted { get; set; }
    }
}
