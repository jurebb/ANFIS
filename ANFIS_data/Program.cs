using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFIS_data
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateData(-4, 4, ANFIS.Functions.Funkcija1());
            Console.ReadKey();
        }

        private static void GenerateData(int donja, int gornja, ANFIS.IFunction f)
        {
            double result = 0;

            using (FileStream fs = File.Open("data.txt", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                for (int i = donja; i <= gornja; i++)
                {
                    for (int j = donja; j <= gornja; j++)
                    {
                        result = f.ValueAt(i, j);
                        sw.Write("{0} {1} {2}", i, j, result.ToString(CultureInfo.InvariantCulture));
                        sw.WriteLine();
                    }
                }
            }
            Console.WriteLine("--> Data written to {0}", 
                Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "data.txt"));

        }
    }
}
