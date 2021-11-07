using Sudoku.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.DlxlibSolvers
{
    public class DlxlibPremierSolver:ISolverSudoku
    {

        public SudokuGrid Solve(SudokuGrid s)
        {

            s = DlxFonction.Solver(s);
            return s;
        }
    }
}
