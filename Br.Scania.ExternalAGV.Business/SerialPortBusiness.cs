using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Br.Scania.ExternalAGV.Business
{
    public class SerialPortBusiness
    {
        public bool CheckSerialPort(string Port)
        {
            //Pegar o nomes das portas conectadas
            string[] Portas = SerialPort.GetPortNames();
            foreach (var item in Portas)
            {
                if (item == Port)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
