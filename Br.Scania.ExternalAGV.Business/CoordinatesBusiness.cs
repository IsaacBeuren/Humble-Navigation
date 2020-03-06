using Br.Scania.ExternalAGV.Model;
using CoordinateSharp;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Br.Scania.ExternalAGV.Business
{
    public class CoordinatesBusiness
    {

        // Reference https://coordinatesharp.com/DeveloperGuide#cartesian-format

        public DegreeDecimalMinuteModel SplitDegree2Minute(double? coordinate)
        {
            try
            {
                DegreeDecimalMinuteModel degreeDecimal = new DegreeDecimalMinuteModel();

                string sCoordinate = coordinate.ToString();
                string[] array = sCoordinate.Split(",");
                int index = 0;
                if (sCoordinate.Substring(0, 1) == "-")
                {
                    index = 1;
                    degreeDecimal.Degree = Convert.ToInt32("-" + sCoordinate.Substring(index, 2));
                    degreeDecimal.DecimalMinute = Convert.ToDouble(sCoordinate.Substring(index + 2, 2) + "," + array[1]);
                }
                else
                {
                    degreeDecimal.Degree = Convert.ToInt32(sCoordinate.Substring(index, 2));
                    degreeDecimal.DecimalMinute = Convert.ToDouble(sCoordinate.Substring(index + 2, 2) + "," + array[1]);
                }

                return degreeDecimal;
            }
            catch (Exception)
            {
                return null;
                throw;
            }

        }

        public Coordinate DegreeDecimalMinute2Coordinate(double? Latitude, double? Longitude)
        {
            try
            {
                Coordinate coordinateActual = new Coordinate();
                DegreeDecimalMinuteModel retActualLatCoordinate = SplitDegree2Minute(Latitude);
                DegreeDecimalMinuteModel retActualLngCoordinate = SplitDegree2Minute(Longitude);

                coordinateActual.Latitude.Degrees = retActualLatCoordinate.Degree;
                coordinateActual.Latitude.DecimalMinute = retActualLatCoordinate.DecimalMinute;
                coordinateActual.Longitude.Degrees = retActualLngCoordinate.Degree;
                coordinateActual.Longitude.DecimalMinute = retActualLngCoordinate.DecimalMinute;
                return coordinateActual;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }

        }

        private double GetQuad(double value)
        {
            if (value >= 0 && value <= 90)
            {
                return 1;
            }
            if (value > 90 && value <= 180)
            {
                return 2;
            }
            if (value > 180 && value <= 270)
            {
                return 3;
            }
            if (value > 270 && value <= 360)
            {
                return 4;
            }
            return 0;
        }

        public double AngularCalculation(double Rumo, double Destino)
        {
            //Console.Write("Rumo " + Start);
            //Console.Write(" Destino " + End);
            double quadRumo = GetQuad(Rumo);
            double quadDestino = GetQuad(Destino);
            double angulo = 0;

            if (quadDestino == quadRumo)
            {
                angulo = Destino - Rumo;
            }
            else
            {
                if ((quadDestino - quadRumo) >= -2 && (quadDestino - quadRumo) <= 2)
                {
                    angulo = Destino - Rumo;
                    if (angulo < -180)
                    {
                        angulo = 360 + angulo;
                    }
                    else
                    {
                        if (angulo > 180)
                        {
                            angulo = (360 - angulo) * -1;

                        }
                    }
                }
                else
                {
                    angulo = Rumo - Destino;
                    if (angulo < 180)
                    {
                        angulo = (360 + angulo) * -1;
                    }
                    else
                    {
                        angulo = (360 - angulo);
                    }
                }
            }
            //Console.WriteLine(" Diferença " + angulo);

            if (angulo < -180 || angulo > 180)
            {
                Console.WriteLine("Erro");
            }


            return angulo;
        }

        public Coordinate Decimal(double lat, double lng)
        {
            Coordinate c = new Coordinate(lat, lng);

            c.FormatOptions.Format = CoordinateFormatType.Decimal;
            c.FormatOptions.Display_Leading_Zeros = true;
            c.FormatOptions.Round = 3;

            return c;
        }


        public Coordinate Decimal_Degree(double lat, double lng)
        {
            Coordinate c = new Coordinate(lat, lng);

            c.FormatOptions.Format = CoordinateFormatType.Decimal_Degree;
            c.FormatOptions.Display_Leading_Zeros = true;
            c.FormatOptions.Round = 3;

            return c;
        }

        public Coordinate Degree_Decimal_Minutes(double lat, double lng)
        {
            Coordinate c = new Coordinate(lat, lng);

            c.FormatOptions.Format = CoordinateFormatType.Degree_Decimal_Minutes;
            c.FormatOptions.Display_Leading_Zeros = true;
            c.FormatOptions.Round = 3;

            return c;
        }

        public Coordinate Degree_Minutes_Seconds(double lat, double lng)
        {
            Coordinate c = new Coordinate(lat, lng);

            c.FormatOptions.Format = CoordinateFormatType.Degree_Minutes_Seconds;
            c.FormatOptions.Display_Leading_Zeros = true;
            c.FormatOptions.Round = 3;

            return c;
        }

        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        // Calculating Distance
        static List<Coordinate> coordinate = new List<Coordinate>();

        public Distance Calculating_Distance(Coordinate Start, Coordinate End)
        {
            if (Start != null && End != null)
            {
                if (Start.Latitude.DecimalDegree != 0 && Start.Longitude.DecimalDegree != 0 && End.Latitude.DecimalDegree != 0 && End.Longitude.DecimalDegree != 0)
                {
                    Distance d = new Distance(Start, End, Shape.Ellipsoid);

                    return d;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public void GeoFencingPolygon()
        {
            List<GeoFence.Point> points = new List<GeoFence.Point>();

            //Points specified manually to create a square in the USA.
            //First and last points should be identical if creating a polygon boundary.
            points.Add(new GeoFence.Point(31.65, -106.52));
            points.Add(new GeoFence.Point(31.65, -84.02));
            points.Add(new GeoFence.Point(42.03, -84.02));
            points.Add(new GeoFence.Point(42.03, -106.52));
            points.Add(new GeoFence.Point(31.65, -106.52));

            GeoFence gf = new GeoFence(points);


            Coordinate c = new Coordinate(36.67, -101.51);

            //Determine if Coordinate is within polygon
            gf.IsPointInPolygon(c);

            //Determine if Coordinate is within specific range of shapes line.
            gf.IsPointInRangeOfLine(c, 1000); //Method 1 specify meters.

            Distance d = new Distance(1, DistanceType.Kilometers);
            gf.IsPointInRangeOfLine(c, d); //Method 2 specify Distance object.
        }

        public bool GeoFencingPoints(Coordinate Start, Coordinate End, double Range)
        {

            // https://www.latlong.net/lat-long-dms.html

            double lat_inicial = Convert.ToDouble(Start.Latitude.DecimalDegree);
            double long_inicial = Convert.ToDouble(Start.Longitude.DecimalDegree);
            double lat_final = Convert.ToDouble(End.Latitude.DecimalDegree);
            double long_final = Convert.ToDouble(End.Longitude.DecimalDegree);

            double d2r = 0.017453292519943295769236;

            double dlong = (long_final - long_inicial) * d2r;
            double dlat = (lat_final - lat_inicial) * d2r;

            double temp_sin = Math.Sin(dlat / 2.0);
            double temp_cos = Math.Cos(lat_inicial * d2r);
            double temp_sin2 = Math.Sin(dlong / 2.0);

            double a = (temp_sin * temp_sin) + (temp_cos * temp_cos) * (temp_sin2 * temp_sin2);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            double result = 6368.1 * c;

            if (result < Range)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public double CourseBetweenPoints(Coordinate Start, Coordinate End)
        {
            double lat1 = Start.Latitude.DecimalDegree;
            double lat2 = End.Latitude.DecimalDegree;
            double lon1 = Start.Longitude.DecimalDegree;
            double lon2 = End.Longitude.DecimalDegree;

            Distance distance = Calculating_Distance(Start, End); // Distance

            double d = distance.Kilometers;
            double pi = 3.14159265359;

            double a1 = Math.Sin(lat2);
            double b1 = Math.Sin(lat1);
            double c1 = Math.Cos(d);
            double d1 = Math.Sin(d);
            double e1 = Math.Cos(lat1);
            double f1 = Math.Acos(a1 - b1 * c1) / (d1 * e1);

            if (Math.Sin(lon2 - lon1) < 0)
            {
                return f1;
            }
            else
            {
                return 2 * pi - f1;
            }
        }

        double DEG_PER_RAD = (180.0 / Math.PI);
        // Return Bearing (degrees)

        public double GetBearing(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = lon2 - lon1;
            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            return DEG_PER_RAD * Math.Atan2(y, x);
        }


        // Convert Degree-Minute-Second to Degree.ddd
        public double ParseCoordinate(string coordinate)
        {
            var dms = coordinate.Split(' ');
            var rVal = 0.0;
            var i = 0;
            foreach (var s in dms)
            {
                double d;
                double.TryParse(s, out d);
                rVal += (d / (Math.Pow(60.0, i++)));
            }
            rVal /= DEG_PER_RAD;
            return rVal;
        }

        public double RouteCalculate(Coordinate Start, Coordinate End)
        {
            double DLA = End.Latitude.DecimalDegree - Start.Latitude.DecimalDegree;
            double DLO = End.Longitude.DecimalDegree - Start.Longitude.DecimalDegree;

            Distance distance = Calculating_Distance(Start, End); // Distance

            double tanA = DLA / DLO;

            double senA = DLA / distance.Kilometers;
            double cosA = DLO / distance.Kilometers;

            double graus = RadianToDegree(Math.Atan(DLA / DLO));

            return 0;
        }

        public double GetAzimuth(Coordinate Start, Coordinate End)
        {
            double lat1 = Start.Latitude.DecimalDegree;
            double lat2 = End.Latitude.DecimalDegree;
            double lon1 = Start.Longitude.DecimalDegree;
            double lon2 = End.Longitude.DecimalDegree;

            return RadianToDegree(Math.Asin(Math.Sin(lon1 - lon2) * Math.Cos(lat2) / Math.Sin(Math.Acos(Math.Sin(lat2) * Math.Sin(lat1) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1)))));
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        Regex reg = new Regex(@"^((?<D>\d{1,2}(\.\d+)?)(?<W>[SN])|(?<D>\d{2})(?<M>\d{2}(\.\d+)?)(?<W>[SN])|(?<D>\d{2})(?<M>\d{2})(?<S>\d{2}(\.\d+)?)(?<W>[SN])|(?<D>\d{1,3}(\.\d+)?)(?<W>[WE])|(?<D>\d{3})(?<M>\d{2}(\.\d+)?)(?<W>[WE])|(?<D>\d{3})(?<M>\d{2})(?<S>\d{2}(\.\d+)?)(?<W>[WE]))$");

        private double DMS2Decimal(string dms)
        {
            double result = double.NaN;

            var match = reg.Match(dms);

            if (match.Success)
            {
                var degrees = double.Parse("0" + match.Groups["D"]);
                var minutes = double.Parse("0" + match.Groups["M"]);
                var seconds = double.Parse("0" + match.Groups["S"]);
                var direction = match.Groups["W"].ToString();
                var dec = (Math.Abs(degrees) + minutes / 60d + seconds / 3600d) * (direction == "S" || direction == "W" ? -1 : 1);
                var absDec = Math.Abs(dec);

                if ((((direction == "W" || direction == "E") && degrees <= 180 & absDec <= 180) || (degrees <= 90 && absDec <= 90)) && minutes < 60 && seconds < 60)
                {
                    result = dec;
                }

            }

            return result;

        }


        public double ConvertDM2DMS(double value)
        {
            string[] latArray = value.ToString().Split(",");
            double degree = Convert.ToDouble(latArray[0].Substring(0, latArray[0].Length - 2));
            double minute = Convert.ToDouble(latArray[0].Substring(latArray[0].Length - 2, 2) + "," + latArray[1]) / 60;
            if (degree > 0)
            {
                return degree + minute;
            }
            else
            {
                return degree - minute;
            }
        }

    }
}


