using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using Sudoku.GeneticSharpSolvers;
using Sudoku.Shared;

namespace Sudoku.GeneticAlgorithmSolver
{
    /// <summary>
    /// This simple chromosome simply represents each cell by a gene with value between 1 and 9, accounting for the target mask if given
    /// </summary>
    public class SudokuCellsChromosome : SudokuChromosomeBase, ISudokuChromosome
    {
       

        /// <summary>
        /// Defines if the chromosome cell should be initialized with row permutations, allowing for ordered subcrossovers with metaheuristics
        /// </summary>
        public bool InitWithPermutations { get; set; }


        public SudokuCellsChromosome() : this(null)
        {
        }

        /// <summary>
        /// Constructor that accepts a Sudoku to solve
        /// </summary>
        /// <param name="targetCore.Sudoku">the target sudoku to solve</param>
        public SudokuCellsChromosome(SudokuGrid targetSudoku) : base(targetSudoku, 81)
        {
        }

        /// <summary>
        /// Generates genes with digits for each index within the 81 Sudoku cells
        /// </summary>
        /// <param name="geneIndex"></param>
        /// <returns>a gene with a digit for the corresponding cell index</returns>
        public override Gene GenerateGene(int geneIndex)
        {
            var rowIndex = geneIndex / 9;
            var colIndex = geneIndex % 9;
            //If a target mask exist and has a digit for the cell, we use it.
            if (TargetSudoku != null && TargetSudoku.Cells[rowIndex][colIndex] != 0)
            {
                return new Gene(TargetSudoku.Cells[rowIndex][colIndex]);
            }
            var rnd = RandomizationProvider.Current;
            // otherwise we use a random digit.
            return new Gene(rnd.GetInt(1, 10));
        }

        public override IChromosome CreateNew()
        {
            return new SudokuCellsChromosome(TargetSudoku) {InitWithPermutations = InitWithPermutations};
        }

        /// <summary>
        /// Builds a single Sudoku from the 81 genes
        /// </summary>
        /// <returns>A Sudoku board built from the 81 genes</returns>
        public IList<SudokuGrid> GetSudokus()
        {
            var cellsArray = GetGenes().Select(g => (int)g.Value).ToArray().ToJaggedArray(9);
            var sudoku = new SudokuGrid(){Cells = cellsArray };
            return new List<SudokuGrid>(new[] { sudoku });
        }




        /// <summary>
        /// Creates the initial cell genes, either random accounting for the target Sudoku Mask, or according to row permutations with the same constraint
        /// </summary>
        protected override void CreateGenes()
        {
            if (InitWithPermutations)
            {
                for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    var rowPerms = TargetRowsPermutations[rowIndex];
                    var rndIndx = RandomizationProvider.Current.GetInt(0, rowPerms.Count);
                    var rowPerm = rowPerms[rndIndx];
                    for (int colIndex = 0; colIndex < 9; colIndex++)
                    {
                        ReplaceGene(9 * rowIndex + colIndex, new Gene(rowPerm[colIndex]));
                    }
                }
            }
            else
            {
                base.CreateGenes();
            }

        }




    }
}
