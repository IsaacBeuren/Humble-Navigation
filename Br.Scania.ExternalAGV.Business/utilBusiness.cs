using Br.Scania.ExternalAGV.Model;
using System;

namespace Br.Scania.ExternalAGV.Business
{
    public class utilBusiness
    {
        public float? Convert2Float(object obj)
        {
            try
            {
                string value = obj.ToString();
                float number;
                string temp = value.Replace(".", ",");
                bool success = float.TryParse(temp, out number);
                if (success == true)
                {
                    return number;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Convert2Time(object obj)
        {
            try
            {
                string value = obj.ToString();
                return value.Substring(0,2)+":"+value.Substring(2, 2) + ":" + value.Substring(4, 2);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public double? Convert2Double(object obj)
        {
            try
            {
                string value = obj.ToString();
                double number;
                string temp = value.Replace(".", ",");
                bool success = double.TryParse(temp, out number);
                if (success == true)
                {
                    return number;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int? Convert2Int(object obj)
        {
            try
            {
                string value = obj.ToString();
                int number;
                string temp = value.Replace(".", ",");
                bool success = int.TryParse(temp, out number);
                if (success == true)
                {
                    return number;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Convert2String(object obj)
        {
            string value = obj.ToString();
            return value.Trim();
        }

        public DateTime? Convert2DateTime(ZDAModel zda)
        {
            //"05/01/2009 14:57:32.8"
            try
            {
                DateTime number;
                string temp = zda.Day + "/" + zda.Month + "/" + zda.Year + " " + zda.UTC;
                bool success = DateTime.TryParse(temp, out number);
                if (success == true)
                {
                    return number;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public double ConvertAngle(double value)
        {
            if (value < 0)
            {
                return value + 360;
            }
            if (value >= 0 && value <= 360)
            {
                return value;
            }
            if (value > 360)
            {
                return value - 360;
            }
            return 0;
        }

        public double FormatCoordinate(double value)
        {
            string sLAt = value.ToString("R");
            string[] arr = sLAt.Split(",");
            double retValue = Convert.ToDouble(arr[0].Substring(0, arr[0].Length - 2) + "," + arr[0].Substring(arr[0].Length - 2, 2) + arr[1]);
            return retValue;
        }


    }
}
