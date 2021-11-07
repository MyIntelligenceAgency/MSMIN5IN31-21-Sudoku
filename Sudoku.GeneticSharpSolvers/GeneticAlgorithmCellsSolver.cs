using GeneticSharp.Domain.Chromosomes;
using Sudoku.Shared;

namespace Sudoku.GeneticAlgorithmSolver
{
    public class GeneticAlgorithmCellsSolver : GeneticAlgorithmSolverBase
    {

        protected override IChromosome GetSudokuChromosome(SudokuGrid puzzle)
        {
            return new SudokuCellsChromosome(puzzle);

        }
    }
}