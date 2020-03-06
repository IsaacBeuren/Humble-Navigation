using System;
using Br.Scania.ExternalAGV.Model;

namespace Br.Scania.ExternalAGV.Business
{
    public class NmeaBusiness
    {
        utilBusiness util = new utilBusiness();

        public GGAModel ConvertNmea2GGA(string item)
        {
            try
            {
                string[] array = item.Split(",");
                string[] arrayCheckSun = array[array.Length - 1].Split("*");
                if (array.Length == 15)
                {
                    GGAModel ggaModel = new GGAModel();
                    ggaModel.Message_ID = array[0];
                    ggaModel.UTC_of_position_fix = util.Convert2Float(array[1]);
                    ggaModel.Latitude = util.Convert2Double(array[2]);
                    ggaModel.Direction_of_latitude = util.Convert2String(array[3]);
                    if (ggaModel.Direction_of_latitude == "S")
                    {
                        ggaModel.Latitude = ggaModel.Latitude * -1;
                    }
                    ggaModel.Longitude = util.Convert2Double(array[4]);
                    ggaModel.Direction_of_longitude = util.Convert2String(array[5]);
                    if (ggaModel.Direction_of_longitude == "W")
                    {
                        ggaModel.Longitude = ggaModel.Longitude * -1;
                    }
                    ggaModel.GPS_Quality_indicator = util.Convert2Int(array[6]);
                    ggaModel.Number_of_SVs_in_use = util.Convert2Int(array[7]);
                    ggaModel.HDOP = util.Convert2Float(array[8]);
                    ggaModel.Orthometric_height = util.Convert2String(array[9]);
                    ggaModel.M_unit = util.Convert2String(array[10]);
                    ggaModel.Geoid_separation = util.Convert2String(array[11]);
                    ggaModel.M_geoid_separation_measured_in_meters = util.Convert2String(array[12]);
                    ggaModel.Age_of_differential_GPS_data_record = util.Convert2String(array[13]);
                    ggaModel.Reference_station_ID = util.Convert2Float(array[14]);
                    ggaModel.The_checksum_data = util.Convert2String(arrayCheckSun[0]);
                    return ggaModel;
                }
                else
                {
                    //Console.WriteLine("Erro " + item);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetGPSQualityIndicator(int? GPSQualityIndicator)
        {
            try
            {
                int GPSQuality = Convert.ToInt16(GPSQualityIndicator);

                switch (GPSQualityIndicator)
                {
                    case 0:
                        return "Invalid";
                    case 1:
                        return "GPS fix (SPS)";
                    case 2:
                        return "DGPS fix";
                    case 3:
                        return "PPS fix";
                    case 4:
                        return "RTK";
                    case 5:
                        return "Float RTK";
                    case 6:
                        return "Estimated";
                    case 7:
                        return "Manual input mode";
                    case 8:
                        return "Simulation mode";
                    default:
                        return "Erro";
                }
            }
            catch (Exception)
            {
                return "Erro";
            }
        }


        public ZDAModel ConvertNmea2ZDA(string item)
        {
            try
            {
                string[] array = item.Split(",");
                string[] arrayCheckSun = array[array.Length - 1].Split("*");
                if (array.Length == 7)
                {
                    ZDAModel zdaModel = new ZDAModel();
                    zdaModel.Message_ID = array[0];
                    zdaModel.UTC = util.Convert2Time(array[1]);
                    zdaModel.Day = util.Convert2Int(array[2]);
                    zdaModel.Month = util.Convert2Int(array[3]);
                    zdaModel.Year = util.Convert2Int(array[4]);
                    zdaModel.LTZ_Hour = util.Convert2Int(array[5]);
                    zdaModel.LTZ_Minutes = util.Convert2Int(array[6]);
                    zdaModel.The_checksum_data = util.Convert2String(arrayCheckSun[0]);
                    zdaModel.DateConverted = util.Convert2DateTime(zdaModel);
                    return zdaModel;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public GSTModel ConvertNmea2GST(string item)
        {
            try
            {
                string[] array = item.Split(",");
                string[] arrayCheckSun = array[array.Length - 1].Split("*");
                if (array.Length == 7)
                {
                    GSTModel gstModel = new GSTModel();
                    gstModel.Message_ID = array[0];
                    gstModel.UTC_of_position_fix = array[1];
                    gstModel.RMS_value_of_the_pseudorange_residuals = array[2];
                    gstModel.Error_ellipse_semi_major_axis_1_sigma_error = array[3];
                    gstModel.Error_ellipse_semi_minor_axis_1_sigma_error = array[4];
                    gstModel.Error_ellipse_orientation = array[5];
                    gstModel.Latitude_1_sigma_error = array[6];
                    gstModel.Longitude_1_sigma_error = array[7];
                    gstModel.Height_1_sigma_error = array[8];
                    gstModel.The_checksum_data = util.Convert2String(arrayCheckSun[0]);
                    return gstModel;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
