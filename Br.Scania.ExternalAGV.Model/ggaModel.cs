using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Model
{
    public class GGAModel
    {
        public int Type { get; set; }
        public string Message_ID { get; set; }
        public float? UTC_of_position_fix { get; set; }
        public double? Latitude { get; set; }
        public string Direction_of_latitude { get; set; }
        public double? Longitude { get; set; }
        public string Direction_of_longitude { get; set; }
        public int? GPS_Quality_indicator { get; set; }
        public int? Number_of_SVs_in_use { get; set; }
        public float? HDOP { get; set; }
        public string Orthometric_height { get; set; }
        public string M_unit { get; set; }
        public string Geoid_separation { get; set; }
        public string M_geoid_separation_measured_in_meters { get; set; }
        public string Age_of_differential_GPS_data_record { get; set; }
        public float? Reference_station_ID { get; set; }
        public string The_checksum_data { get; set; }
    }
}
