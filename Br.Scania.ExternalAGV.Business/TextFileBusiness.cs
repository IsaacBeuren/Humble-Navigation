using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Br.Scania.ExternalAGV.Business
{
    public class TextFileBusiness
    {

        public List<string> ReadText()
        {
            string arquivo = @"D:\GPS\Leitura.txt";
            if (File.Exists(arquivo))
            {
                List<string> lista = new List<string>();
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo))
                    {
                        string linha;
                        string LinhaCompleta="";
                        // Lê linha por linha até o final do arquivo
                        while ((linha = sr.ReadLine()) != null)
                        {
                            if(linha.Length > 6)
                            {
                                string head = linha.Substring(3, 3);

                                if ((head == "GGA" || head == "ZDA" || head == "GST"))
                                {
                                    LinhaCompleta = LinhaCompleta + linha;
                                }
                            }
                        }

                        string[] List = LinhaCompleta.Split("$");
                        
                        foreach (string item in List)
                        {
                            string listItem = "$" + item;
                            lista.Add(listItem);
                        }
                    }
                    return lista;
                }
                catch (Exception)
                {
                    return lista;
                }
            }
            else
            {
                return null;
            }
        }

        public void WriteText(string message)
        {
            string AssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
            string date = DateTime.Now.Date.ToString().Substring(0, 10).Replace('/', '_');
            string pathName = "LOG_" + date + "_GGA.log";

            if (!File.Exists(AssemblyPath + @"\" + pathName))
            {
                File.Create(AssemblyPath + @"\" + pathName).Close();
                TextWriter arquivo = File.AppendText(AssemblyPath + @"\" + pathName);
                arquivo.WriteLine(message);
                arquivo.Close();
            }
            else
            {
                TextWriter arquivo = File.AppendText(AssemblyPath + @"\" + pathName);
                arquivo.WriteLine(message);
                arquivo.Close();
            }
        }
    }
}
