using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFIS
{
    public class RuleSet
    {
        double[] _a1, _b1;          //parametri sigmoide od Ai
        double[] _a2, _b2;          //parametri sigmoide od Bi
        double[] _p, _q, _r;        //parametri linearnog konsekvensa
        private int _m;

        public RuleSet(int numOfRules)
        {
            _m = numOfRules;
            _a1 = new double[numOfRules];
            _b1 = new double[numOfRules];
            _a2 = new double[numOfRules];
            _b2 = new double[numOfRules];
            _p = new double[numOfRules];
            _q = new double[numOfRules];
            _r = new double[numOfRules];
        }

        public int NumOfRules
        {
            get
            {
                return _m;
            }
        }

        public double AntecedentA(int ruleIndex, double x)
        {
            return Sigmoid(_a1[ruleIndex], _b1[ruleIndex], x);
        }

        public double AntecedentB(int ruleIndex, double y)
        {
            return Sigmoid(_a2[ruleIndex], _b2[ruleIndex], y);
        }

        private double Sigmoid(double a, double b, double var)
        {
            return 1 / (1 + (double)Math.Exp( b * ( var - a )));
        }

        public double Alpha(int ruleIndex, double x, double y)
        {
            return AntecedentA(ruleIndex, x) * AntecedentB(ruleIndex, y);
        }

        public double Konsekvens(int ruleIndex, double x, double y)
        {
            return _p[ruleIndex] * x + _q[ruleIndex] * y + _r[ruleIndex];
        }
        
        internal void InitializeParams()            //TODO CHECK trebaju li svi biti između 0 i 1?
        {
            Random rand = new Random();
            RandomNum ran = new RandomNum();
            for (int i = 0; i < _m; i++)
            {
                _a1[i] = ran.GetDouble(1, -1);
                _b1[i] = ran.GetDouble(1, -1);
                _a2[i] = ran.GetDouble(1, -1);
                _b2[i] = ran.GetDouble(1, -1);
                _p[i] = ran.GetDouble(1, -1);
                _q[i] = ran.GetDouble(1, -1);
                _r[i] = ran.GetDouble(1, -1);
            }
        }

        public double GetA1(int ruleIndex)
        {
            return _a1[ruleIndex];
        }
        public void SetA1(int ruleIndex, double value)
        {
            _a1[ruleIndex] = value;
        }

        public double GetB1(int ruleIndex)
        {
            return _b1[ruleIndex];
        }
        public void SetB1(int ruleIndex, double value)
        {
            _b1[ruleIndex] = value;
        }

        public double GetA2(int ruleIndex)
        {
            return _a2[ruleIndex];
        }
        public void SetA2(int ruleIndex, double value)
        {
            _a2[ruleIndex] = value;
        }

        public double GetB2(int ruleIndex)
        {
            return _b2[ruleIndex];
        }
        public void SetB2(int ruleIndex, double value)
        {
            _b2[ruleIndex] = value;
        }

        public double GetP(int ruleIndex)
        {
            return _p[ruleIndex];
        }
        public void SetP(int ruleIndex, double value)
        {
            _p[ruleIndex] = value;
        }
        public double GetQ(int ruleIndex)
        {
            return _q[ruleIndex];
        }
        public void SetQ(int ruleIndex, double value)
        {
            _q[ruleIndex] = value;
        }
        public double GetR(int ruleIndex)
        {
            return _r[ruleIndex];
        }
        public void SetR(int ruleIndex, double value)
        {
            _r[ruleIndex] = value;
        }
    }
}
