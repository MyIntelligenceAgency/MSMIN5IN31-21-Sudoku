using GeneticSharp.Domain.Chromosomes;
using Sudoku.Shared;

namespace Sudoku.GeneticAlgorithmSolver
{
    public class GeneticAlgorithmPermutationSolver : GeneticAlgorithmSolverBase
    {

        protected override IChromosome GetSudokuChromosome(SudokuGrid puzzle)
        {
            return new SudokuPermutationsChromosome(puzzle);
        }

    }
}