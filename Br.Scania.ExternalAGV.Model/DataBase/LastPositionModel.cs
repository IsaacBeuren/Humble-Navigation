using System;

namespace Br.Scania.ExternalAGV.Model.DataBase
{
    public partial class LastPositionModel
    {
        public int ID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime UpdateTime { get; set; }
        public int GPSQuality { get; set; }
        public int IDAGV { get; set; }
    }
}
