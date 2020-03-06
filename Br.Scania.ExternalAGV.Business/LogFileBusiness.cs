using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Br.Scania.ExternalAGV.Business
{
    public class LogFileBusiness
    {
        string AssemblyPath;
        string pathName;

        public LogFileBusiness()
        {
            AssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
            pathName = "LogRegister.log";
        }

        public void Write(string msg)
        {
            string date = DateTime.Now.Date.ToString().Substring(0, 10) + " - " + DateTime.Now.TimeOfDay.ToString().Substring(0, 8);

            if (!File.Exists(AssemblyPath + @"\" + pathName))
            {
                File.Create(AssemblyPath + @"\" + pathName).Close();
                TextWriter arquivo = File.AppendText(date + " - " + AssemblyPath + @"\" + pathName);
                arquivo.WriteLine(msg);
                arquivo.Close();
            }
            else
            {
                TextWriter arquivo = File.AppendText(date + " - " + AssemblyPath + @"\" + pathName);
                arquivo.WriteLine(msg);
                arquivo.Close();
            }
        }
    }
}
