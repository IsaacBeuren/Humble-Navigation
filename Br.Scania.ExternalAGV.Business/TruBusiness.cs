using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using Br.Scania.ExternalAGV.Model;
using Newtonsoft.Json;

namespace Br.Scania.ExternalAGV.Business
{
    public class TruBusiness
    {
        static SerialPort port;

        public TruBusiness(int portName)
        {
            try
            {
                port = new SerialPort();
                port.PortName = "COM"+ portName;
                port.BaudRate = 115200;
                port.Parity = Parity.None;
                port.DataBits = 8;
                port.StopBits = StopBits.One;
                port.Handshake = Handshake.None;
            }
            catch (Exception)
            {

            }
        }

        //GPS Thread
        public NMEAModel ReadGPS()
        {
            try
            {
                DateTime localDate = DateTime.Now;
                string pathName = @"D:\GPS\" + localDate.ToString() + ".txt";

                string msgAscII = null;
                byte[] rxBuff = new byte[2048];
                string[] result = null;
                if (!port.IsOpen)
                {
                    port.Open();
                }
                else
                {
                    DateTime hora = DateTime.Now;
                    if (port.BytesToRead > 0)
                    {
                        int numOfBytes = port.Read(rxBuff, 0, rxBuff.Length);
                        StringBuilder sb = new StringBuilder(8192);
                        msgAscII = msgAscII + Encoding.ASCII.GetString(rxBuff, 0, numOfBytes);
                        result = msgAscII.Split("$G");
                        NMEAModel nmea = new NMEAModel();
                        NmeaBusiness nmeaBusiness = new NmeaBusiness();
                        GGAModel ggaModel = null;
                        GSTModel gstModel = null;
                        ZDAModel zdaModel = null;
                        foreach (string item in result)
                        {
                            string ret="";
                            try
                            {
                                ret = item.Substring(1, item.Length - 1);
                                Console.WriteLine(ret);
                            }
                            catch (Exception)
                            {
                                //arquivo.WriteLine("Receive Failure");
                                //Console.WriteLine("Leitura Vazia");
                            }

                            if (item.Length > 5)
                            {
                                string Header = ret.Substring(0, 3);

                                switch (Header)
                                {
                                    case "GGA":
                                        ggaModel = nmeaBusiness.ConvertNmea2GGA(ret);
                                        break;
                                    case "GST":
                                        gstModel = nmeaBusiness.ConvertNmea2GST(ret);
                                        break;
                                    case "ZDA":
                                        zdaModel = nmeaBusiness.ConvertNmea2ZDA(ret);
                                        break;
                                }
                            }
                        }
                        if (nmea.GGAModel == null) { nmea.GGAModel = ggaModel; };
                        if (nmea.GSTModel == null) { nmea.GSTModel = gstModel; };
                        if (nmea.ZDAModel == null) { nmea.ZDAModel = zdaModel; };
                        return nmea;
                    }
                    else
                    {
                        return null;
                    }
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