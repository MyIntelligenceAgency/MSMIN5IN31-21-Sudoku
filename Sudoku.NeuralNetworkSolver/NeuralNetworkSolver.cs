using System;
using System.IO;
using System.Text;
using Keras.Models;
using Sudoku.Shared;

namespace Sudoku.NeuralNetworkSolver
{
	public class NeuralNetworkSolver:ISolverSudoku
	{
        public string ModelPath { get; set; } = @"Models\sudoku.model";
        
        private BaseModel model;

        public BaseModel Model
        {
            get {
                if (model == null)
                {
                    string fullPath = Path.Combine(Environment.CurrentDirectory, ModelPath);
                    model = NeuralNetHelper.LoadModel(fullPath);
                }

                return model;
            }
        }

        public SudokuGrid Solve(SudokuGrid s)
		{
			return NeuralNetHelper.SolveSudoku(s, Model);
		}

        static string GetLocalPath(string relativePath)
        {
            return System.IO.Path.Combine(Environment.CurrentDirectory, relativePath);
        }
	}
}