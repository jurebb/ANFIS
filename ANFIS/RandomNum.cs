using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFIS
{
    class RandomNum
    {
        Random random = new Random();

        public double GetDouble(double gornja, double donja)
        {
            return random.NextDouble() * (gornja - (donja)) + (donja);
        }
    }
}
