using System;
using System.Linq;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Math;
using Microsoft.ML.Probabilistic.Models;
using Sudoku.Shared;

namespace GraphicModelSolver
{

    public class GraphicalModelIterativeSolver : GraphicalModelRobustSolver
    {

        public int NbIterationCells { get; set; } = 2;

        protected override void DoInference(Dirichlet[] dirArray, int[] sudokuCells)
        {

            int cellDiscovered = sudokuCells.Count(c => c > 0);

            // Iteration tant que l'on a pas découvert toutes les cases
            while (cellDiscovered < CellIndices.Count - 1)
            {

                Dirichlet[] cellsProbsPosterior = InferenceEngine.Infer<Dirichlet[]>(ProbCells);

                int[] bestCellsProbsPosteriorIndex = getBestDirichletSubArrayIndex(cellsProbsPosterior, NbIterationCells, sudokuCells);

                foreach (var index in bestCellsProbsPosteriorIndex)
                {
                    var mode = cellsProbsPosterior[index].GetMode();
                    var value = mode.IndexOf(mode.Max()) + 1;

                    Vector v = Vector.Constant(CellDomain.Count, EpsilonProba);
                    v[value - 1] = FixedValueProba;

                    dirArray[index] = Dirichlet.PointMass(v);

                    sudokuCells[index] = value;
                    cellDiscovered++;
                }

                CellsPrior.ObservedValue = dirArray;
            }
        }

        private int[] getBestDirichletSubArrayIndex(Dirichlet[] dirichletArray, int N, int[] sudokuCells)
        {
            // Initialise la liste des N meilleurs index avec les N premiers index de dirichletArray pour les cellules vides
            int[] bestDirIndex = sudokuCells.Select((cell, index) => cell == 0 ? index : -1).Where(index => index != -1).Take(N).ToArray();

            // Pour chaque cellule == 0 du sudoku
            foreach (var cellIndex in CellIndices)
            {
                if (sudokuCells[cellIndex] == 0)
                {
                    var currentMode = dirichletArray[cellIndex].GetMode();

                    int minDirIndex = bestDirIndex[0];

                    // Récupère l'index du Dirichlet le plus petit de la liste d'index des meilleurs Dirichlet
                    foreach (var index in bestDirIndex)
                    {
                        var currentDirMode = dirichletArray[index].GetMode();
                        var minDirMode = dirichletArray[minDirIndex].GetMode();

                        if (currentDirMode.Max() < minDirMode.Max())
                        {
                            minDirIndex = index;
                        }
                    }
                    // Remplace ce Dirichlet si la valeurs max du Dirichlet de la cellule actuelle est supèrieur
                    if (dirichletArray[minDirIndex].GetMode().Max() < currentMode.Max())
                    {
                        bestDirIndex[Array.IndexOf(bestDirIndex, minDirIndex)] = cellIndex;
                    }
                }
            }
            return bestDirIndex;
        }

    }





    /// <summary>
    /// Tentative de redéfinition de la méthode itérative combinant des simplifications diverses et une nouvelle définition de la confiance.
    /// </summary>
    public class GraphicalModelAlernateIterativeSolver : GraphicalModelIterativeSolver
    {
        protected override void DoInference(Dirichlet[] dirArray, int[] sudokuCells)
        {
            int cellDiscovered = sudokuCells.Count(c => c > 0);

            // Iteration tant que l'on a pas découvert toutes les cases
            while (cellDiscovered < CellIndices.Count - 1)
            {

                Dirichlet[] cellsProbsPosterior = InferenceEngine.Infer<Dirichlet[]>(ProbCells);

                var bestCells = GetBestCondidenceSubArray(cellsProbsPosterior, NbIterationCells, sudokuCells);


                foreach (var cellPair in bestCells)
                {

                    var mode = cellsProbsPosterior[cellPair.cellIndex].GetMode();


                    var value = mode.IndexOf(mode.Max()) + 1;
                    Vector v = Vector.Constant(CellDomain.Count, EpsilonProba);
                    v[value - 1] = FixedValueProba;

                    dirArray[cellPair.cellIndex] = Dirichlet.PointMass(v);

                    sudokuCells[cellPair.cellIndex] = value;
                    cellDiscovered++;
                }

                CellsPrior.ObservedValue = dirArray;
            }
        }


        private (int cellIndex, double confidence)[] GetBestCondidenceSubArray(Dirichlet[] dirichletArray, int nbBests, int[] sudokuCells)
        {
            var toReturn = new (int cellIndex, double confidence)[nbBests];
            var minIndex = nbBests - 1;

            foreach (var cellIndex in CellIndices)
            {
                if (sudokuCells[cellIndex] == 0)
                {
                    var confidence = GetDirichletConfidence(dirichletArray[cellIndex]);
                    if (confidence > toReturn[minIndex].confidence)
                    {
                        toReturn[minIndex] = (cellIndex, confidence);
                        // recompute minIndex
                        minIndex = 0;
                        for (int i = 0; i < toReturn.Length; i++)
                        {
                            if (toReturn[i].confidence < toReturn[minIndex].confidence) { minIndex = i; }
                        }
                    }
                }
            }
            return toReturn;
        }



        /// <summary>
        /// Provides a confidence index by the difference between best and second best probability 
        /// </summary>
        private double GetDirichletConfidence(Dirichlet distribution)
        {
            var dMode = distribution.GetMode();
            // Récupération des deux valeurs max
            var maxValues = dMode.Aggregate<double, (double max1, double max2)>((dMode[0], dMode[0]), (currentMaxes, val) =>
                val > currentMaxes.max1 ? (val, currentMaxes.max2) : val > currentMaxes.max2 ? (currentMaxes.max1, val) : currentMaxes);
            return maxValues.max1 - maxValues.max2;
        }
    }






}