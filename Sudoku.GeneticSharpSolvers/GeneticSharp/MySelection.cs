/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Selections
{
    /// <summary>
    /// Selects the chromosomes with the best fitness.
    /// </summary>
    /// <remarks>
    /// Also know as: Truncation Selection.
    /// </remarks>    
    [DisplayName("EliteRandom")]
    public class MySelection : SelectionBase
    {
        public IChromosome EveChromosome;
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Selections.EliteSelection"/> class.
        /// </summary>
        public MySelection() : base(2)
        {
        }
        #endregion

        #region ISelection implementation
        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>The select chromosomes.</returns>
        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
        {
            
            var ordered = generation.Chromosomes.OrderByDescending(c => c.Fitness);

            IList<IChromosome> returnList = ordered.Take(number / 2).ToList();

            for (int i = 0; i < number / 2; i++)
            {
                var c = EveChromosome.CreateNew();

                if (c == null)
                {
                    throw new InvalidOperationException("The Eve chromosome's 'CreateNew' method generated a null chromosome. This is a invalid behavior, please, check your chromosome code.");
                }

                c.ValidateGenes();
                returnList.Add(c);
            }

            return returnList;
        }



        #endregion
    }
}*/