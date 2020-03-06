using Br.Scania.ExternalAGV.Model;
using System;
using System.Globalization;

namespace Br.Scania.ExternalAGV.Business
{
    public class Coordinator2Business
    {
        static Int32 LAG = 0;
        static Int32 LAM = 0;
        static double LAS = 0.0;

        static Int32 LOG = 0;
        static Int32 LOM = 0;
        static double LOS = 0.0;

        public void gyroscope(Int32 LatGraus, Int32 LatMin, double LatSec, Int32 LonGraus, Int32 LonMin, double LonSec)
        {
            if (LAG != LatGraus)
            {
                LAG = LatGraus;
            }
            if (LAM != LatMin)
            {
                LAM = LatMin;
            }
            if (LAS != LatSec)
            {
                LAS = LatSec;
            }
            if (LOG != LonGraus)
            {
                LOG = LonGraus;
            }
            if (LOM != LonMin)
            {
                LOM = LonMin;
            }
            if (LOS != LonSec)
            {
                LOS = LonSec;
            }
        }

        public string ConvertDecimalToDegrees(double Coordenada)
        {
            int Graus = 0;
            double tempMinutos = 0;
            int Minutos = 0;
            double Segundos = 0;
            try
            {
                if (Coordenada < 0)
                {
                    Coordenada = Coordenada * -1;
                    Graus = Convert.ToInt32(Math.Floor(Coordenada));
                    tempMinutos = (Coordenada - Graus) * 60;
                    Minutos = Convert.ToInt32(Math.Floor(tempMinutos));
                    Segundos = Math.Round((tempMinutos - Minutos) * 60, 6);
                    Graus = Graus * -1;
                }
                else
                {
                    Graus = Convert.ToInt32(Math.Floor(Coordenada));
                    tempMinutos = (Coordenada - Graus) * 60;
                    Minutos = Convert.ToInt32(Math.Floor(tempMinutos));
                    Segundos = Math.Round((tempMinutos - Minutos) * 60, 6);
                }
                string sCoordenada = Graus + ":" + Minutos + ":" + Segundos;
                return sCoordenada;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public string ConvertDatunToDegrees(string sCoordinate)
        {
            double Coordenada;
            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = ".", NegativeSign = "\u2212", NumberNegativePattern = 1 };
            const NumberStyles style = NumberStyles.AllowLeadingSign | NumberStyles.Number | NumberStyles.AllowDecimalPoint;
            double.TryParse(sCoordinate, style, numberFormatInfo, out Coordenada);
            int GrausMin = Convert.ToInt32(Coordenada);
            int Graus = GrausMin / 100;
            double Minutos = (GrausMin - (Graus * 100));
            double Segundos = 0;
            Segundos = ObterCasasDecimais(Coordenada);
            string sCoordenada = Graus + ":" + Minutos + ":" + Segundos;
            return sCoordenada;
        }

        static int ObterCasasDecimais(double numero)
        {
            decimal resultado = Convert.ToDecimal(numero) - Math.Floor(Convert.ToDecimal(numero));
            int parteDecimal = Convert.ToInt32((double)resultado * Math.Pow(10, BitConverter.GetBytes(decimal.GetBits(resultado)[3])[2]));
            return parteDecimal;
        }

        public double ConvertDegreesToDecimal(string coordinate)
        {
            string[] sCoordenada = coordinate.Split(':');
            if (sCoordenada.Length > 3)
            {
                double degrees = Double.Parse(sCoordenada[0]);
                double minutes = Double.Parse(sCoordenada[1]) / 60;
                double seconds = Double.Parse(sCoordenada[2]) / 360000;

                if (degrees > 0)
                {
                    return Math.Round(degrees + minutes + seconds, 8);
                }
                else
                {
                    return Math.Round(degrees - minutes - seconds, 8);
                }
            }
            return 0;
        }


        public Int32 DistanceBetweenPlaces(CoordinatorModel coordinatorModel)
        {
            double DLA = (coordinatorModel.InLatitude - coordinatorModel.FiLatitude) * 1852;//milimetros
            double DLO = (coordinatorModel.InLongitude - coordinatorModel.FiLongitude) * 1852;//milimetros
            //DT = √((DLA∗1.852)2 + (DLO∗1.852))2
            double DT = Math.Round(Math.Sqrt(Math.Pow(DLA, 2) + Math.Pow(DLO, 2)), 8);
            if (DT < 32000)
            {
                return Convert.ToInt32(DT * 100);
            }
            else
            {
                return 0;
            }

        }

        public double calcDistancia(CoordinatorModel coordinatorModel)
        {
            double lat_inicial = coordinatorModel.InLatitude;
            double long_inicial = coordinatorModel.InLongitude;
            double lat_final = coordinatorModel.FiLatitude;
            double long_final = coordinatorModel.FiLongitude;

            double d2r = 0.017453292519943295769236;

            double dlong = (long_final - long_inicial) * d2r;
            double dlat = (lat_final - lat_inicial) * d2r;

            double temp_sin = Math.Sin(dlat / 2.0);
            double temp_cos = Math.Cos(lat_inicial * d2r);
            double temp_sin2 = Math.Sin(dlong / 2.0);

            double a = (temp_sin * temp_sin) + (temp_cos * temp_cos) * (temp_sin2 * temp_sin2);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            return 6368.1 * c;
        }

        public double ConvertCoordinateToMinutes(string Coordinate)
        {
            string[] sCoordinate = Coordinate.Split(':');
            double[] dCoordinate = new double[3];
            dCoordinate[0] = Double.Parse(sCoordinate[0]);
            dCoordinate[1] = Double.Parse(sCoordinate[1]);
            dCoordinate[2] = Double.Parse(sCoordinate[2]) / 100;
            if (sCoordinate[3].ToUpper() == "S" || sCoordinate[3].ToUpper() == "O")
            {
                dCoordinate[0] = dCoordinate[0] * -1;
                dCoordinate[1] = dCoordinate[1] * -1;
                dCoordinate[2] = dCoordinate[2] * -1;
            }
            return (dCoordinate[0] * 60) + dCoordinate[1] + (dCoordinate[2] / 60);
        }

        public AzimuteModel RouteCalculate(CoordinatorModel coordinatorModel)
        {
            double DeltaLat = coordinatorModel.FiLatitude - coordinatorModel.InLatitude;
            double DeltaLong = coordinatorModel.FiLongitude - coordinatorModel.InLongitude;
            AzimuteModel azimuteModel = new AzimuteModel();
            double DeltaLong2 = 0;
            double DeltaLat2 = 0;
            double Hipote = 0;
            double Sin = 0;
            double Degree = 0;
            double CalcFinal = 0;

            if (DeltaLong != 0)
            {
                DeltaLong2 = Math.Pow(DeltaLong, 2);
                DeltaLat2 = Math.Pow(DeltaLat, 2);
                double ab = DeltaLat2 + DeltaLong2;
                Hipote = Math.Sqrt(ab);
                Sin = DeltaLat / Hipote;
                Degree = Math.Asin(Sin);
                CalcFinal = Degree * (180 / Math.PI);

                if (DeltaLong > 0)
                {
                    CalcFinal = CalcFinal - 90;
                }
                if (DeltaLong < 0)
                {
                    CalcFinal = 90 - CalcFinal;
                }
            }
            azimuteModel.B = CalcFinal;
            return azimuteModel;

        }



    }
}
