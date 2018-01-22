using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFIS
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            Console.ReadKey();
        }

        private static void Zad8()
        {
            NeuralNetwork nn = new NeuralNetwork(7, 0.001, 2, "data.txt");
            
            NeuralNetwork nn2 = new NeuralNetwork(7, 0.001, 1, "data.txt");
            
            NeuralNetwork nna = new NeuralNetwork(7, 0.00002, 2, "data.txt");
            
            NeuralNetwork nn2a = new NeuralNetwork(7, 0.00002, 1, "data.txt");
            
            NeuralNetwork nnb = new NeuralNetwork(7, 0.0055, 2, "data.txt");
            
            NeuralNetwork nn2b = new NeuralNetwork(7, 0.0055, 1, "data.txt");
            

            //experimental
            Parallel.Invoke(
                () => nn.EpochTrainingWithErrorReport(100001, "z8report2001.txt"),
                () => nn2.EpochTrainingWithErrorReport(100001, "z8report1001.txt"),
                () => nna.EpochTrainingWithErrorReport(100001, "z8report200002.txt"),
                () => nn2a.EpochTrainingWithErrorReport(100001, "z8report100002.txt"),
                () => nnb.EpochTrainingWithErrorReport(100001, "z8report20055.txt"),
                () => nn2b.EpochTrainingWithErrorReport(100001, "z8report10055.txt"));
        }

        private static void Zad7()
        {
            NeuralNetwork nn = new NeuralNetwork(7, 0.001, 2, "data.txt");
            nn.EpochTrainingWithErrorReport(300001, "report2.txt");
            NeuralNetwork nn2 = new NeuralNetwork(7, 0.001, 1, "data.txt");
            nn2.EpochTrainingWithErrorReport(300001, "report1.txt");
        }

        private static void Zad6()
        {
            NeuralNetwork nn = new NeuralNetwork(7, "afterParams.txt", "data.txt");
        }

        private static void Zad5()
        {
            NeuralNetwork nn = new NeuralNetwork(7, 0.001, 2, "data.txt");
            nn.EpochTraining(350001);
            nn.DumpDataToFile("afterParams.txt");
        }

        private static void Test1()
        {
            //NeuralNetwork nn = new NeuralNetwork(1, 0.000005, 2, "data.txt");
            //NeuralNetwork nn = new NeuralNetwork(2, 0.0001, 2, "data.txt");
            //NeuralNetwork nn = new NeuralNetwork(4, 0.00045, 2, "data.txt");
            //NeuralNetwork nn = new NeuralNetwork(5, 0.00055, 2, "data.txt");

            //NeuralNetwork nn = new NeuralNetwork(10, 0.001, 2, "data.txt");

            NeuralNetwork nn = new NeuralNetwork(7, 0.005, 2, "data.txt");  //ekstremno velika
            nn.EpochTraining(100001);
        }
    }
}
