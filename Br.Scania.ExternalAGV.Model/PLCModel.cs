using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Model
{
    public class Commands2PLCModel
    {
        public bool EnableAuto { get; set; }
        public bool OnStraight { get; set; }
        public double Velocity { get; set; }
        public int Degree { get; set; }
        public int RealDegree { get; set; }
        public bool RightLight { get; set; }
        public bool LeftLight { get; set; }
        public int Counter { get; set; }
        public double PathDeviation { get; set; }
    }

    public class Parameters2PLCModel
    {
        public int Maximum_steer_Turning { get; set; }
        public int Maximum_steer_on_Straight { get; set; }
        public double ManVelocity { get; set; }
        public double WarningVelocity { get; set; }
    }

    public class StatusPLCModel
    {
        public double VelocityTraction { get; set; }
        //public int TravelDistance { get; set; }
        public bool BT_Load { get; set; }
        public bool ManualEnabled { get; set; }
        public bool RightScanner { get; set; }
        public bool LeftScanner { get; set; }
        public bool LiddarScanner { get; set; }
        public bool VehicleRun { get; set; }

    }


}












