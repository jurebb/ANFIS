using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFIS
{
    public class Functions
    {
        public static IFunction Funkcija1()
        {
            ConcreteFunction Obj1 = new ConcreteFunction((x, y) =>
            {
                return (Math.Pow(x - 1, 2) + Math.Pow(y + 2, 2) - 5*x*y + 3) * Math.Pow(Math.Cos(Math.PI / 5), 2);
            });
            return Obj1;
        }
    }
}
