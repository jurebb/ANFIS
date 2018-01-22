using System;

namespace ANFIS
{
    internal class ConcreteFunction : IFunction
    {
        Func<int, int, double> _doFunction;

        public ConcreteFunction(Func<int, int, double> functionA)
        {
            _doFunction = functionA;
        }

        public double ValueAt(int x, int y)
        {
            return _doFunction(x, y);
        }
    }
}