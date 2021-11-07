using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Sudoku.GeneticSharpSolvers;
using Sudoku.Shared;

namespace Sudoku.GeneticAlgorithmSolver
{
    public abstract class GeneticAlgorithmSolverBase: ISolverSudoku
    {

        static GeneticAlgorithmSolverBase()
        {
            var init = SudokuPermutationsChromosome.AllPermutations;
        }


        protected virtual GeneticAlgorithm GetGeneticAlgorithm(
            IPopulation population,
            IFitness fitness,
            ISelection selection,
            ICrossover crossover,
            IMutation mutation)
        {
            return new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
        }

        public virtual bool SupportsTplStrategy()
        {
            return true;
        }


        protected abstract IChromosome GetSudokuChromosome(SudokuGrid puzzle);
       

        public SudokuGrid Solve(SudokuGrid s)
        {
            

            var initialPopulationSize = 5000;

            var stagnationGenerationNb = 20;

            IChromosome sudokuChromosome = GetSudokuChromosome(s);
            //IChromosome sudokuChromosome = new SudokuCellsChromosome(s);
            var fitnessThreshold = 0;
            //var generationNb = 50;
            var crossoverProbability = 0.75f;
            var mutationProbability = 0.2f;
            var fitness = new SudokuFitness(s);
            var selection = new EliteSelection();
            var crossover = new UniformCrossover();
            var mutation = new UniformMutation();
            IChromosome bestIndividual;
            var solution = s;
            int nbErrors = int.MaxValue;

            var termination = new OrTermination(new ITermination[]
            {
                new FitnessThresholdTermination(fitnessThreshold),
                new FitnessStagnationTermination(stagnationGenerationNb),
                //new GenerationNumberTermination(generationNb)
                //new TimeEvolvingTermination(TimeSpan.FromSeconds(10)), 
            });

            IOperatorsStrategy operatorsStrategy ;
            if (SupportsTplStrategy())
            {
                operatorsStrategy = new TplOperatorsStrategy();
            }
            else
            {
                operatorsStrategy = new DefaultOperatorsStrategy();
            }
                

            do
            {
                var population = new Population(initialPopulationSize, initialPopulationSize, sudokuChromosome);
                var ga = GetGeneticAlgorithm(population, fitness, selection, crossover, mutation);
                ga.Termination = termination;
                ga.MutationProbability = mutationProbability;
                ga.CrossoverProbability = crossoverProbability;
                //ga.OperatorsStrategy = operatorsStrategy;

                ga.GenerationRan+= delegate(object sender, EventArgs args)
                {
                    bestIndividual = (ga.Population.BestChromosome);
                    solution = ((ISudokuChromosome)bestIndividual).GetSudokus()[0];
                    nbErrors = solution.NbErrors(s);
                    Console.WriteLine($"Generation #{ga.GenerationsNumber}: best individual has {nbErrors} errors");
                };
                ga.Start();

                //bestIndividual = (ga.Population.BestChromosome);
                //solution = ((ISudokuChromosome)bestIndividual).GetSudokus()[0];
                //nbErrors = solution.NbErrors(s);
                if (nbErrors == 0) 
                {
                    break;
                }
                else
                {
                    initialPopulationSize *= 2;
                    Console.WriteLine($"Genetic search failed with {nbErrors} resulting errors, doubling population to {initialPopulationSize}");
                }
                

            } while (true);

            return solution;

        }
    }
}