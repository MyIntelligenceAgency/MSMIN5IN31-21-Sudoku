using System;
using System.Collections.Generic;
using Sudoku.Shared;
using Kermalis.SudokuSolver.Core;
using System.Linq;
using System.Collections.ObjectModel;

namespace Sudoku.HumanTechnique
{
    public class HumanTechniquePremierSolver:ISolverSudoku
    {

        public Puzzle ConvertSudokuGridToPuzzle(SudokuGrid s)
        {

            Puzzle p = new Puzzle(s.Cells, false);

            return p;
        }

        public SudokuGrid ConvertPuzzleToSudokuGrid(Puzzle p)
        {
            return new SudokuGrid() { Cells = p.Columns.Select(r => r.Select(c => c.Value).ToArray()).ToArray()};
        }

        public SudokuGrid Solve(SudokuGrid s)
        {
            Puzzle p = ConvertSudokuGridToPuzzle(s);

            bool solved; // If this is true after a segment, the puzzle is solved and we can break

            p.RefreshCandidates();

            do
            {
                solved = true;

                bool changed = false;
                // Check for naked singles or a completed puzzle
                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        Cell cell = p[x, y];
                        if (cell.Value == 0)
                        {
                            solved = false;
                            // Check for naked singles
                            int[] a = cell.Candidates.ToArray(); // Copy
                            if (a.Length == 1)
                            {
                                cell.Set(a[0]);
                                changed = true;
                            }
                        }
                    }
                }
                // Solved or failed to solve
                if (solved || (!changed && ! SolverTechnique.RunTechnique(p)))
                {
                    break;
                }
            } while (true);

            return ConvertPuzzleToSudokuGrid(p);
        }
      
    }
}