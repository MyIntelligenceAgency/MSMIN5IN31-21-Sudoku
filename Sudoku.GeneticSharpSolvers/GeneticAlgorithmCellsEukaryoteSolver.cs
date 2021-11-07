using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using Sudoku.Shared;

namespace Sudoku.GeneticAlgorithmSolver
{
    public class GeneticAlgorithmCellsEukaryoteSolver : GeneticAlgorithmSolverBase
    {

        protected override IChromosome GetSudokuChromosome(SudokuGrid puzzle)
        {
            return new SudokuCellsChromosome(puzzle){InitWithPermutations = true};

        }

        public override bool SupportsTplStrategy()
        {
            return false;
        }

        protected override GeneticAlgorithm GetGeneticAlgorithm(IPopulation population, IFitness fitness, ISelection selection,
            ICrossover crossover, IMutation mutation)
        {

            // We split the original 81 genes/cells chromosome into a 9x9genes chromosomes Karyotype
            var metaHeuristics = new EukaryoteMetaHeuristic(9, 9, new DefaultMetaHeuristic()) { Scope = EvolutionStage.Crossover | EvolutionStage.Mutation };
            //Since we used rows permutations at init and the solution is also a row permutation, we used ordered crossovers and mutations to keep yielding permutations
            crossover = new CycleCrossover();
            mutation = new TworsMutation();

            return new MetaGeneticAlgorithm(population, fitness, selection, crossover, mutation, metaHeuristics);

        }
    }
}