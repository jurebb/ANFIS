using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFIS
{
    class NeuralNetwork
    {
        double _learningRate;        //stopa ucenja
        int _M;                     //broj pravila
        int _algorithmDesignator;   //odreduje koristimo li backprop, stoch ili minibatch
        const int _Dint = 9;        //broj diskretnih vrijednosti u intervalu za x/y funkcije u skupu ozancenih primjera 
        RuleSet _rules;
        int _N;                     //broj primjera

        int[] Dx;                   //primjeri za ucenje
        int[] Dy;                   //primjeri za ucenje
        double[] Dz;                //primjeri za ucenje
        private double _learningRateSigm;
        private int v1;
        private string v2;

        public NeuralNetwork(int brojPravila, double learningRate, int algorithmDesignator, string filename)
        {
            _M = brojPravila;
            _learningRate = learningRate;
            _learningRateSigm = learningRate;
            _algorithmDesignator = algorithmDesignator;

            Dx = new int[_Dint * _Dint];
            Dy = new int[_Dint * _Dint];
            Dz = new double[_Dint * _Dint];
            InitialzieData(filename, ref Dx, ref Dy, ref Dz);
            _rules = new RuleSet(brojPravila);
            _rules.InitializeParams();     
        }

        public NeuralNetwork(int brojPravila, string filenamePar, string filename)      //ovaj konstruktor za plottanje fje pogreske
        {
            _M = brojPravila;
            Dx = new int[_Dint * _Dint];
            Dy = new int[_Dint * _Dint];
            Dz = new double[_Dint * _Dint];
            _rules = new RuleSet(brojPravila);
            InitializeWholeNetwork(filenamePar, filename, ref Dx, ref Dy, ref Dz);
            WriteErrorToFile();
        }

        private void InitialzieData(string filename, ref int[] dx, ref int[] dy, ref double[] dz)
        {
            try
            {
                using (StreamReader sr = File.OpenText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), filename)))
                {
                    int j = 0;
                    do
                    {
                        string[] redak = sr.ReadLine().Split(' ');
                        redak = redak.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                        dx[j] = int.Parse(redak[0]);
                        dy[j] = int.Parse(redak[1]);
                        dz[j] = double.Parse(redak[2], CultureInfo.InvariantCulture);

                        j++;

                    } while (sr.Peek() != -1);

                    _N = j;
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void InitializeWholeNetwork(string filenamePar, string filename, ref int[] dx, ref int[] dy, ref double[] dz)
        {
            try
            {
                using (StreamReader sr = File.OpenText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), filename)))
                {
                    int j = 0;
                    do
                    {
                        string[] redak = sr.ReadLine().Split(' ');
                        redak = redak.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                        dx[j] = int.Parse(redak[0]);
                        dy[j] = int.Parse(redak[1]);
                        dz[j] = double.Parse(redak[2], CultureInfo.InvariantCulture);

                        j++;

                    } while (sr.Peek() != -1);
                    _N = j;
                }


                using (StreamReader sr = File.OpenText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), filenamePar)))
                {
                    int j = 0;
                    do
                    {
                        string[] redak = sr.ReadLine().Split(' ');
                        redak = redak.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                        _rules.SetA1(j, double.Parse(redak[0], CultureInfo.InvariantCulture));
                        _rules.SetB1(j, double.Parse(redak[1], CultureInfo.InvariantCulture));
                        _rules.SetA2(j, double.Parse(redak[2], CultureInfo.InvariantCulture));
                        _rules.SetB2(j, double.Parse(redak[3], CultureInfo.InvariantCulture));

                        j++;

                    } while (sr.Peek() != -1);
                }

                using (StreamReader sr = File.OpenText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "lin_" + filenamePar)))
                {
                    int j = 0;
                    do
                    {
                        string[] redak = sr.ReadLine().Split(' ');
                        redak = redak.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                        _rules.SetP(j, double.Parse(redak[0], CultureInfo.InvariantCulture));
                        _rules.SetQ(j, double.Parse(redak[1], CultureInfo.InvariantCulture));
                        _rules.SetR(j, double.Parse(redak[2], CultureInfo.InvariantCulture));

                        j++;

                    } while (sr.Peek() != -1);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void EpochTraining(int numberOfEpochs)
        {
            for (int i = 0; i < numberOfEpochs; i++)
            {
                if (_algorithmDesignator == 2)
                {
                    TrainWholeSetStochastic();
                }
                else if (_algorithmDesignator == 1)
                {
                    TrainWholeSetBatch();
                }

                if (numberOfEpochs > 1000)
                {
                    if (i % 1000 == 0 && i > 0)
                    {
                        Console.Write(">>> Epoha {0}: ", i);
                        Console.Write("\tSrednja kvadratna greska: {0}\n", Error());
                    }
                }
            }
        }

        public void EpochTrainingWithErrorReport(int numberOfEpochs, string fileNameReport)
        {
            Console.WriteLine("> Ucenje i pisanje u file {0}", fileNameReport);
            using (FileStream fs = File.Open(fileNameReport, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                for (int i = 0; i < numberOfEpochs; i++)
                {

                    if (_algorithmDesignator == 2)
                    {
                        TrainWholeSetStochastic();
                    }
                    else if (_algorithmDesignator == 1)
                    {
                        TrainWholeSetBatch();
                    }

                    if (numberOfEpochs > 100)
                    {
                        if (i % 100 == 0)
                        {
                            sw.WriteLine("{0} {1}", i, Error().ToString(CultureInfo.InvariantCulture));
                        }
                    }

                }
            }
            Console.WriteLine("Done.");
        }

        public void TrainWholeSetStochastic()
        {
            for (int i = 0; i < _N; i++)
            {
                SingleTrainingIterationStoch(i);
            }
        }

        public void TrainWholeSetBatch()
        {
            SingleTrainingIterationBatch();
        }

        private void SingleTrainingIterationStoch(int exampleIndex)
        {
            double o;
            double brojnik = 0;
            double nazivnik = 0;                    //suma alfi
            double brojnik2;                        //za potrebe azuriranje parametara a i b
            for (int i = 0; i < _M; i++)
            {
                double alfa = _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]);
                nazivnik += alfa;
                brojnik += alfa * _rules.Konsekvens(i, Dx[exampleIndex], Dy[exampleIndex]);
            }
            //if (nazivnik < Math.Pow(10, -6))
            //{
            //    throw new Exception("nulu ili nesto drugo?");
            //    o = 0;
            //}
            o = brojnik / nazivnik;                 //rucno dosli do o (iako postoji fja) da sacuvamo medjurezultate

            for (int i = 0; i < _M; i++)
            {
                brojnik2 = 0;

                _rules.SetP(i, _rules.GetP(i) + _learningRate * (Dz[exampleIndex] - o) * (_rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * Dx[exampleIndex]) / nazivnik);
                _rules.SetQ(i, _rules.GetQ(i) + _learningRate * (Dz[exampleIndex] - o) * (_rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * Dy[exampleIndex]) / nazivnik);
                _rules.SetR(i, _rules.GetR(i) + _learningRate * (Dz[exampleIndex] - o) * (_rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex])) / nazivnik);

                for (int j = 0; j < _M; j++)
                {
                    if(i != j)
                    {
                        brojnik2 += _rules.Alpha(j, Dx[exampleIndex], Dy[exampleIndex]) 
                            * (_rules.Konsekvens(i, Dx[exampleIndex], Dy[exampleIndex]) - _rules.Konsekvens(j, Dx[exampleIndex], Dy[exampleIndex]));
                    }
                }

                _rules.SetA1(i, _rules.GetA1(i) + _learningRateSigm * (Dz[exampleIndex] - o) * (brojnik2) / Math.Pow(nazivnik, 2)
                    * _rules.GetB1(i) * _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * (1 - _rules.AntecedentA(i, Dx[exampleIndex])) );
                _rules.SetB1(i, _rules.GetB1(i) - _learningRateSigm * (Dz[exampleIndex] - o) * (brojnik2) / Math.Pow(nazivnik, 2)
                    * (Dx[exampleIndex] - _rules.GetA1(i)) * _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * (1 - _rules.AntecedentA(i, Dx[exampleIndex])));
                _rules.SetA2(i, _rules.GetA2(i) + _learningRateSigm * (Dz[exampleIndex] - o) * (brojnik2) / Math.Pow(nazivnik, 2)
                    * _rules.GetB2(i) * _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * (1 - _rules.AntecedentB(i, Dy[exampleIndex])));
                _rules.SetB2(i, _rules.GetB2(i) - _learningRateSigm * (Dz[exampleIndex] - o) * (brojnik2) / Math.Pow(nazivnik, 2)
                    * (Dy[exampleIndex] - _rules.GetA2(i)) * _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * (1 - _rules.AntecedentB(i, Dy[exampleIndex])));
            }
        }

        private void SingleTrainingIterationBatch()
        {
            double o;
            double brojnik = 0;
            double nazivnik = 0;                    //suma alfi
            double brojnik2;                        //za potrebe azuriranje parametara a i b

            List<double> gradijentP = new List<double>(new double[_M]);
            List<double> gradijentQ = new List<double>(new double[_M]);
            List<double> gradijentR = new List<double>(new double[_M]);
            List<double> gradijentA1 = new List<double>(new double[_M]);
            List<double> gradijentB1 = new List<double>(new double[_M]);
            List<double> gradijentA2 = new List<double>(new double[_M]);
            List<double> gradijentB2 = new List<double>(new double[_M]);

            for (int exampleIndex = 0; exampleIndex < _N; exampleIndex++)
            {
                brojnik = 0;
                nazivnik = 0;
                for (int i = 0; i < _M; i++)
                {
                    double alfa = _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]);
                    nazivnik += alfa;
                    brojnik += alfa * _rules.Konsekvens(i, Dx[exampleIndex], Dy[exampleIndex]);
                }
                o = brojnik / nazivnik;                 //rucno dosli do o (iako postoji fja) da sacuvamo medjurezultate

                //akumulirajmo gradijente
                for (int i = 0; i < _M; i++)
                {
                    brojnik2 = 0;

                    gradijentP[i] += (Dz[exampleIndex] - o) * (_rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * Dx[exampleIndex]) / nazivnik;
                    gradijentQ[i] += (Dz[exampleIndex] - o) * (_rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * Dy[exampleIndex]) / nazivnik;
                    gradijentR[i] += (Dz[exampleIndex] - o) * (_rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex])) / nazivnik;

                    for (int j = 0; j < _M; j++)
                    {
                        if (i != j)
                        {
                            brojnik2 += _rules.Alpha(j, Dx[exampleIndex], Dy[exampleIndex])
                                * (_rules.Konsekvens(i, Dx[exampleIndex], Dy[exampleIndex]) - _rules.Konsekvens(j, Dx[exampleIndex], Dy[exampleIndex]));
                        }
                    }

                    gradijentA1[i] += (Dz[exampleIndex] - o) * (brojnik2) / Math.Pow(nazivnik, 2)
                        * _rules.GetB1(i) * _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * (1 - _rules.AntecedentA(i, Dx[exampleIndex]));
                    gradijentB1[i] += (Dz[exampleIndex] - o) * (brojnik2) / Math.Pow(nazivnik, 2)
                        * (Dx[exampleIndex] - _rules.GetA1(i)) * _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * (1 - _rules.AntecedentA(i, Dx[exampleIndex]));
                    gradijentA2[i] += (Dz[exampleIndex] - o) * (brojnik2) / Math.Pow(nazivnik, 2)
                        * _rules.GetB2(i) * _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * (1 - _rules.AntecedentB(i, Dy[exampleIndex]));
                    gradijentB2[i] += (Dz[exampleIndex] - o) * (brojnik2) / Math.Pow(nazivnik, 2)
                        * (Dy[exampleIndex] - _rules.GetA2(i)) * _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]) * (1 - _rules.AntecedentB(i, Dy[exampleIndex]));
                }

            }

            for (int i = 0; i < _M; i++)
            {
                _rules.SetP(i, _rules.GetP(i) + _learningRate * gradijentP[i]);
                _rules.SetQ(i, _rules.GetQ(i) + _learningRate * gradijentQ[i]);
                _rules.SetR(i, _rules.GetR(i) + _learningRate * gradijentR[i]);
                
                _rules.SetA1(i, _rules.GetA1(i) + _learningRateSigm * gradijentA1[i]);
                _rules.SetB1(i, _rules.GetB1(i) - _learningRateSigm * gradijentB1[i]);
                _rules.SetA2(i, _rules.GetA2(i) + _learningRateSigm * gradijentA2[i]);
                _rules.SetB2(i, _rules.GetB2(i) - _learningRateSigm * gradijentB2[i]);
            }
        }

        public double Loss(double expectedOutput, double output)
        {
            double sum = 0;                  
            for (int i = 0; i < _M; i++)
            {
                sum += (expectedOutput - output) * (expectedOutput - output);
            }
            return sum / 2;
        }

        public double Error()
        {
            double sum = 0;
            for (int i = 0; i < _N; i++)
            {
                sum += Loss(Dz[i], NetworkOutput(i));
            }
            return sum / _N;
        }

        public double NetworkOutput(int exampleIndex)
        {
            double brojnik = 0;
            double nazivnik = 0;

            for (int i = 0; i < _M; i++)
            {
                double alfa = _rules.Alpha(i, Dx[exampleIndex], Dy[exampleIndex]);
                nazivnik += alfa;
                brojnik += alfa * _rules.Konsekvens(i, Dx[exampleIndex], Dy[exampleIndex]);
            }
            
            return brojnik / nazivnik;
        }

        public void DumpDataToFile(string fileName)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                for (int i = 0; i < _M; i++)
                {
                    sw.WriteLine("{0} {1} {2} {3}", _rules.GetA1(i).ToString(CultureInfo.InvariantCulture),
                                                     _rules.GetB1(i).ToString(CultureInfo.InvariantCulture),
                                                      _rules.GetA2(i).ToString(CultureInfo.InvariantCulture),
                                                       _rules.GetB2(i).ToString(CultureInfo.InvariantCulture));
                }
            }

            using (FileStream fs = File.Open("lin_" + fileName, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                for (int i = 0; i < _M; i++)
                {
                    sw.WriteLine("{0} {1} {2}", _rules.GetP(i).ToString(CultureInfo.InvariantCulture),
                                             _rules.GetQ(i).ToString(CultureInfo.InvariantCulture),
                                              _rules.GetR(i).ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public void WriteErrorToFile()
        {
            Console.WriteLine("> Srednja kvadratna greska mreze ucitane iz filea: {0}\n", Error());
            using (FileStream fs = File.Open("error.txt", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                for (int i = 0; i < _N; i++)
                {
                    sw.WriteLine("{0} {1} {2}", Dx[i],
                                                     Dy[i],
                                                       Loss(Dz[i], NetworkOutput(i)).ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
                }
            }
            Console.WriteLine("> Info dump u datoteku error.txt");
        }
    }
}
