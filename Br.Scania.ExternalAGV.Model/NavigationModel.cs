using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Model
{
    public class NavigationModel
    {
        public double? Latitude { get; set; }
        public string Direction_of_latitude { get; set; }
        public double? Longitude { get; set; }
        public string Direction_of_longitude { get; set; }
        public string Height_1_sigma_error { get; set; }
        public DateTime? DateConverted { get; set; }

    }
}
