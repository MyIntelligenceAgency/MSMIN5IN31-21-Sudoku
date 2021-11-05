using Sudoku.Shared;
using GeneticSharp.Domain;
using GeneticSharp.Extensions.Sudoku;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain.Chromosomes;
using System.Linq;

namespace Sudoku.GeneticSharpSolvers
{


    public class GeneticSharpSlowSolver : GeneticSharpPremierSolver
    {
        public GeneticSharpSlowSolver()
        {
            PopulationSize = 5000;
            crossover = new UniformCrossover();
            mutation= new UniformMutation();
    }

}

    public class GeneticSharpPremierSolver : ISolverSudoku
    {

        private const double fitnessThreshold = 0;
        
        //Plutôt que de jouer sur le nombre de générations, j'utilise la notion de stagnation: une fois que la population s'est effondrée sur un minimum local, la fitness ne progresse plus, quelque soit le nombre de générations auquel cela se produit
        private const int fitnessStagnationNb  = 100;


        // J'ai passé les paramètres suivants sous forme de propriétés afin de pouvoir facilement créer des solvers alternatifs pour trouver le bon paramétrage

        public int PopulationSize { get; set; } = 500;
        public float CrossoverProbability { get; set; } = 0.75f;
        public float MutationProbability { get; set; } = 0.2f;

        // rend public les objets qui caractérisent AG
        // instancier 
        public SelectionBase selection { get; set; }= new MySelection();
        public CrossoverBase crossover { get; set; }= new UniformCrossover() ;
        public MutationBase mutation { get; set; }= new UniformMutation();


        public SudokuGrid Solve(SudokuGrid s)
        {
            var sudoku = new SudokuBoard();

            for (int ridx = 0; ridx < 9; ridx++)
            {
                for (int cidx = 0; cidx < 9; cidx++)
                {
                    sudoku.SetCell(ridx, cidx, s.Cells[ridx][cidx]);
                }
            }

            IChromosome chromosome = new SudokuPermutationsChromosome(sudoku);
           
            //var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 50, 0, 10);
            var fitness = new SudokuFitness(sudoku);

            //var selection = new EliteSelection();
            var selection = new MySelection();
            selection.EveChromosome = new SudokuPermutationsChromosome(sudoku);
            var crossover = new UniformCrossover();
            var mutation = new UniformMutation();


            //var mutation = new InsertionMutation();
            var population = new Population(PopulationSize, PopulationSize, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new OrTermination(new ITermination[]
                {
                    new FitnessThresholdTermination(fitnessThreshold),
                    new FitnessStagnationTermination(fitnessStagnationNb)
                }),

                //Termination = new FitnessThresholdTermination(fitnessThreshold),
                MutationProbability = MutationProbability,
                CrossoverProbability = CrossoverProbability,

                // J'ai rajouté l'opérateur d'exécution en parallèle, qui permet de gagner un peu en performance en utilisant tous les cores de vos cpus

               OperatorsStrategy = new TplOperatorsStrategy()
               

            };

            ga.Start();

            var bestIndividual = ((ISudokuChromosome)ga.Population.BestChromosome);

            var solution = bestIndividual.GetSudokus().First();


  

            // faire une eval de la solution avec fitness (si fitness 0 = fini) de bestindividul
            // si =! 0 je prends les 250 pop meilleurs et je remets 250 random 
            // refaire tourner l'algo

            //return solutions.Max(solutionSudoku => fitness.Evaluate(solutionSudoku));

            var toReturn = s.CloneSudoku();

            for (int ridx = 0; ridx < 9; ridx++)
            {
                for (int cidx = 0; cidx < 9; cidx++)
                {
                    toReturn.Cells[ridx][cidx] = solution.GetCell(ridx, cidx);
                }
            }

            return toReturn;
        }


    }


}
