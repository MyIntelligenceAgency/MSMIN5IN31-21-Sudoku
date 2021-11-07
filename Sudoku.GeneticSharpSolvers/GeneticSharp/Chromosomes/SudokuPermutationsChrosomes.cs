using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using Sudoku.GeneticSharpSolvers;
using Sudoku.Shared;

namespace Sudoku.GeneticAlgorithmSolver
{
    /// <summary>
    /// This more elaborated chromosome manipulates rows instead of cells, and each of its 9 gene holds an integer for the index of the row's permutation amongst all that respect the target mask.
    /// Permutations are computed once when a new Sudoku is encountered, and stored in a static dictionary for further reference.
    /// </summary>
    public class SudokuPermutationsChromosome : SudokuChromosomeBase, ISudokuChromosome
    {
       

        /// <summary>
        /// This constructor assumes no mask
        /// </summary>
        public SudokuPermutationsChromosome() : this(null)
        {
        }

        /// <summary>
        /// Constructor with a mask sudoku to solve, assuming a length of 9 genes
        /// </summary>
        /// <param name="targetCore.Sudoku">the target sudoku to solve</param>
        public SudokuPermutationsChromosome(SudokuGrid targetSudoku) : base(targetSudoku, 9)
        {

        }


        /// <summary>
        /// generates a chromosome gene from its index containing a random row permutation
        /// amongst those respecting the target mask. 
        /// </summary>
        /// <param name="geneIndex">the index for the gene</param>
        /// <returns>a gene generated for the index</returns>
        public override Gene GenerateGene(int geneIndex)
        {

            var rnd = RandomizationProvider.Current;
            int permIdx;
            do
            {
                //we randomize amongst the permutations that account for the target mask.
                permIdx = rnd.GetInt(0, TargetRowsPermutations[geneIndex].Count);
                var perm = GetPermutation(geneIndex, permIdx);

                //We pick a random previous row to check compatibility with
                if (geneIndex == 0)
                {
                    //we skip first row
                    break;
                }
                var checkRowIdx = rnd.GetInt(0, geneIndex);
                var checkPerm = GetPermutation(checkRowIdx);
                if (Range9.All(i => perm[i] != checkPerm[i]))
                {
                    break;
                }

            } while (true);



            return new Gene(permIdx);
        }

        public override IChromosome CreateNew()
        {
            var toReturn = new SudokuPermutationsChromosome(TargetSudoku);
            return toReturn;
        }


        /// <summary>
        /// builds a single Sudoku from the given row permutation genes
        /// </summary>
        /// <returns>a list with the single Sudoku built from the genes</returns>
        public virtual IList<SudokuGrid> GetSudokus()
        {
            var cellsArray = new int[9][];
            for (int i = 0; i < 9; i++)
            {
                var perm = GetPermutation(i);
                cellsArray[i] = perm.ToArray();
            }
            var sudoku = new SudokuGrid(){Cells = cellsArray};
            return new List<SudokuGrid>(new[] { sudoku });
        }

        /// <summary>
        /// Gets the permutation to apply from the index of the row concerned
        /// </summary>
        /// <param name="rowIndex">the index of the row to permute</param>
        /// <returns>the index of the permutation to apply</returns>
        protected virtual List<int> GetPermutation(int rowIndex)
        {
            int permIDx = GetPermutationIndex(rowIndex);
            return GetPermutation(rowIndex, permIDx);
        }

        protected virtual List<int> GetPermutation(int rowIndex, int permIDx)
        {

            // we use a modulo operator in case the gene was swapped:
            // It may contain a number higher than the number of available permutations. 
            var perm = TargetRowsPermutations[rowIndex][permIDx % TargetRowsPermutations[rowIndex].Count].ToList();
            return perm;


        }

        /// <summary>
        /// Gets the permutation to apply from the index of the row concerned
        /// </summary>
        /// <param name="rowIndex">the index of the row to permute</param>
        /// <returns>the index of the permutation to apply</returns>
        protected virtual int GetPermutationIndex(int rowIndex)
        {
            return (int)GetGene(rowIndex).Value;
        }


       



    }
}