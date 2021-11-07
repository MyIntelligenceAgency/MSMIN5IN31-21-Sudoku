using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using System.Linq;
using Sudoku.Shared;
using System.Collections.Generic;
using System.Text;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Compound;
using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace Sudoku.GeneticAlgorithmSolver
{
    public class GeneticAlgorithmSimpleIslandPermutationSolver : GeneticAlgorithmSolverBase
    {

        public int IslandNb { get; set; } = 5;


        public int MigrationGenerationPeriod { get; set; } = 5;


        public double MigrationRate { get; set; } = IslandMetaHeuristic.SmallMigrationRate;





        protected override GeneticAlgorithm GetGeneticAlgorithm(IPopulation population, IFitness fitness, ISelection selection,
            ICrossover crossover, IMutation mutation)
        {

            ICompoundMetaheuristic targetCompoundHeuristic;
            var defaultGA = new DefaultMetaHeuristic();
            targetCompoundHeuristic = new SimpleCompoundMetaheuristic(defaultGA);

            var islandCompound = new IslandCompoundMetaheuristic(population.MinSize / IslandNb, IslandNb,
                 targetCompoundHeuristic);
            islandCompound.MigrationsGenerationPeriod = MigrationGenerationPeriod;
            islandCompound.GlobalMigrationRate = MigrationRate;

            var metaHeuristics = islandCompound.Build();

            return new MetaGeneticAlgorithm(population, fitness, selection, crossover, mutation, metaHeuristics);

        }


        protected override IChromosome GetSudokuChromosome(SudokuGrid puzzle)
        {
            return new SudokuPermutationsChromosome(puzzle);
        }


    }
}
