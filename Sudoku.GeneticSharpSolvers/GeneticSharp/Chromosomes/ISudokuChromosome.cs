using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.Shared;
// SudokuChromosome représentant pour la librairie GeneticSharp un Sudoku défini par des gènes

namespace Sudoku.GeneticSharpSolvers
{
    public interface ISudokuChromosome
    {
        IList<SudokuGrid> GetSudokus();
    }
}
