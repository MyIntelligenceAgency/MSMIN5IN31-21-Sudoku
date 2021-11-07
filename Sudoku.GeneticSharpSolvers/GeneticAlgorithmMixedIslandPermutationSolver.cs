using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Compound;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Infrastructure.Framework.Commons;
using Sudoku.Shared;

namespace Sudoku.GeneticAlgorithmSolver
{
    public class GeneticAlgorithmMixedIslandPermutationSolver : GeneticAlgorithmSimpleIslandPermutationSolver
    {





        protected override GeneticAlgorithm GetGeneticAlgorithm(IPopulation population, IFitness fitness, ISelection selection,
            ICrossover crossover, IMutation mutation)
        {

            ICompoundMetaheuristic defaultCompound;
            var defaultGA = new DefaultMetaHeuristic();
            defaultCompound = new SimpleCompoundMetaheuristic(defaultGA);

            

            var noEmbeddingConverter = new GeometricConverter<int>
            {
                IsOrdered = false,
                DoubleToGeneConverter = (geneIndex, geomValue) => Math.Round(geomValue)
                    .PositiveMod(((SudokuChromosomeBase)population.CurrentGeneration.Chromosomes[0]).TargetRowsPermutations[geneIndex].Count), 
                GeneToDoubleConverter = (genIndex, geneValue) => (double) geneValue,
                Embedding = new IdentityEmbedding<int>()
            };
            var geometricConverter = new TypedGeometricConverter();
            geometricConverter.SetTypedConverter(noEmbeddingConverter);
          
            
            var woaIsland = new WhaleOptimisationAlgorithm(){MaxGenerations = 50, GeometricConverter = geometricConverter };


            var islandCompound = new IslandCompoundMetaheuristic(population.MinSize / IslandNb, IslandNb,
                defaultCompound, defaultCompound, defaultCompound, woaIsland, woaIsland);
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