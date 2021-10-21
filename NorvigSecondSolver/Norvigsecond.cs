using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.Shared;
using NorvigSecondSolver;

namespace NorvigSecondSolver
{
    public class Norvigsecond : ISolverSudoku

    {
        public SudokuGrid Solve(SudokuGrid sorel)
        {
			if (S == null || S.is_solved())
			{
				return sorel;
			}
			int k = sorel.least_count();
			Possible p = sorel.possible(k);
			for (int i = 1; i <= 9; i++)
			{
				if (p.is_on(i))
				{
					Sudoku S1 = new Sudoku();
					S1= sorel*
					if (S1.assign(k, i))
					{ 
						if (auto S2 = solve(std::move(S1)))
				          {
				              return S2;
			              }
		             }
	             }
             }

           return (sorel);
        }
    }
}
