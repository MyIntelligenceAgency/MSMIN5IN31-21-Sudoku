using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CsvHelper;
using Humanizer;
using Keras;
using Keras.Models;
using Numpy;
using Python.Runtime;

namespace Sudoku.NeuralNetworkSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestProjectModel();
            TrainAndTest();

        }

		static string GetFullPath(string relativePath)
		{
			return System.IO.Path.Combine(Environment.CurrentDirectory, @"..\..\..\" + relativePath);

        }

        static void TestProjectModel()
        {
            var strPathCSV = GetFullPath(@"Dataset\sudoku.csv.gz");
            var strPathModel = GetFullPath(@"Models\sudoku.model");
            var nbSudokus = 1000;

            var stopW = Stopwatch.StartNew();


            var sudokus = DataSetHelper.ParseCSV(strPathCSV, nbSudokus);

            var testSudoku = sudokus[0];
            Console.Write($"Sudoku to solve:\n{testSudoku.Quiz.ToString()}");
            Console.Write($"Given Solution :\n{testSudoku.Solution.ToString()}");
            var preTrainedModel = NeuralNetHelper.LoadModel(strPathModel);

            var solvedWithNeuralNet = NeuralNetHelper.SolveSudoku(testSudoku.Quiz, preTrainedModel);
            Console.Write($"Solved with Neural Net :\n{solvedWithNeuralNet.ToString()}");


            Console.WriteLine($"Time Elpased: {stopW.Elapsed.Humanize(5)}");
            Console.ReadLine();
        }

        static void TrainAndTest()
        {

            string datasetPath = GetFullPath(@"Dataset\sudoku.csv.gz");
            Console.WriteLine($"Generating model");
            var model = NeuralNetHelper.GenerateModel();
            double accuracy;

            (model, accuracy) = NeuralNetHelper.TrainAndTest(model, datasetPath);

            Console.WriteLine($"Accuracy : {accuracy}%");

            var savePath = Path.Combine(Environment.CurrentDirectory, @"Models\sudoku2.model");

            Console.WriteLine(@$"saving model to {savePath}");

			model.Save(savePath);
		}

    }
}
