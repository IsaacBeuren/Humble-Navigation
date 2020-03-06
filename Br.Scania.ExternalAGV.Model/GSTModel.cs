using System;
using System.Collections.Generic;
using System.Text;

namespace Br.Scania.ExternalAGV.Model
{
    public class GSTModel
    {
        public int Type { get; set; }
        public string Message_ID { get; set; }
        public string UTC_of_position_fix { get; set; }
        public string RMS_value_of_the_pseudorange_residuals { get; set; }
        public string Error_ellipse_semi_major_axis_1_sigma_error { get; set; }
        public string Error_ellipse_semi_minor_axis_1_sigma_error { get; set; }
        public string Error_ellipse_orientation { get; set; }
        public string Latitude_1_sigma_error { get; set; }
        public string Longitude_1_sigma_error { get; set; }
        public string Height_1_sigma_error { get; set; }
        public string The_checksum_data { get; set; }
    }
}
