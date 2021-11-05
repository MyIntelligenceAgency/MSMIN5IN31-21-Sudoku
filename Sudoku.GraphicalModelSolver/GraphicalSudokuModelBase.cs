using System;
using Sudoku.Shared;

namespace GraphicModelSolver
{
    public abstract class GraphicalModelSudokuSolverBase : ISolverSudoku
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
            int[] sCells = T2Dto1D(s.Cells);

            SolveSudoku(sCells);

            var toReturn = new SudokuGrid() { Cells = T1Dto2D(sCells) };

            return toReturn;

        }

        protected abstract void SolveSudoku(int[] sCells);


        public static int[] T2Dto1D(int[][] array)
        {
            int[] OneDim = new int[array.Length * array[0].Length];
            int i = 0;
            foreach (var row in array)
            {
                foreach (var element in row)
                {
                    OneDim[i] = element;
                    i++;
                }
            }
            return OneDim;
        }

        public static int[][] T1Dto2D(int[] array)
        {
            int lengh = (int)Math.Sqrt(array.Length);
            int[][] TwoDim = new int[lengh][];
            // Initialise the jagged array to avoid  "Object reference not set to an instance of an object" exception
            for (int i = 0; i < TwoDim.Length; ++i)
            {
                TwoDim[i] = new int[lengh];
            }

            int RowIndex = 0;
            int ColIndex = 0;
            foreach (var element in array)
            {
                TwoDim[RowIndex][ColIndex] = element;
                ColIndex++;
                if (ColIndex == lengh)
                {
                    RowIndex++;
                    ColIndex = 0;
                }
            }
            return TwoDim;
        }

    }
}