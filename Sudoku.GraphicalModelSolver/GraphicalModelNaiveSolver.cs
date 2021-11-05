using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.Probabilistic.Algorithms;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Models;
using Sudoku.Shared;

namespace GraphicModelSolver
{
    /// <summary>
        /// Ce premier modèle est très faible: d'une part, il ne résout que quelques Sudokus faciles, d'autre part, le modèle est recompilé à chaque fois, ce qui prend beaucoup de temps
        /// </summary>
        public class GraphicalModelNaiveSolver : GraphicalModelSudokuSolverBase
    {

        private static List<int> CellDomain = Enumerable.Range(1, 9).ToList();
        private static List<int> CellIndices = Enumerable.Range(0, 81).ToList();


        protected override void SolveSudoku(int[] sCells)
        {

            var algo = new ExpectationPropagation();
            var engine = new InferenceEngine(algo);

            //Implémentation naïve: une variable aléatoire entière par cellule
            var cells = new List<Variable<int>>(CellIndices.Count);

            foreach (var cellIndex in CellIndices)
            {
                //On initialise le vecteur de probabilités de façon uniforme pour les chiffres de 1 à 9
                var baseProbas = Enumerable.Repeat(1.0, CellDomain.Count).ToList();
                //Création et ajout de la variable aléatoire
                var cell = Variable.Discrete(baseProbas.ToArray());
                cells.Add(cell);
            }

            //Ajout des contraintes de Sudoku (all diff pour tous les voisinages)
            foreach (var cellIndex in CellIndices)
            {
                foreach (var neighbourCellIndex in SudokuGrid.CellNeighbours[cellIndex / 9][cellIndex % 9])
                {
                    int neighbourCellId = neighbourCellIndex.row * 9 + neighbourCellIndex.column;
                    if (neighbourCellId > cellIndex)
                    {
                        Variable.ConstrainFalse(cells[cellIndex] == cells[neighbourCellId]);
                    }
                }
            }

            

            //On affecte les valeurs fournies par le masque à résoudre comme variables observées
            foreach (var cellIndex in CellIndices)
            {
                if (sCells[cellIndex] > 0)
                {
                    cells[cellIndex].ObservedValue = sCells[cellIndex] - 1;
                }
            }

            foreach (var cellIndex in CellIndices)
            {
                if (sCells[cellIndex] == 0)
                {
                    var result = (Discrete)engine.Infer(cells[cellIndex]);
                    sCells[cellIndex] = result.Point + 1;
                }
            }
           
        }


    }
}