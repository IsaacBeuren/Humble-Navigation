using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model.DataBase;
using CoordinateSharp;
using System;
using System.Collections.Generic;

namespace Br.Scania.ExternalAGV.Teste
{
    class Program
    {
        static void Main()
        {
            PointsBusiness pointsBusiness = new PointsBusiness();
            pointsBusiness.GetNextToExecute(1);





            Console.WriteLine("Tecle parta finalizar!");
            Console.ReadKey();
        }

    }
}