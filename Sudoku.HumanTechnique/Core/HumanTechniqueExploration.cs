using System.Collections.Generic;
using Sudoku.Shared;
using Kermalis.SudokuSolver.Core;
using System.Linq;
using System.Diagnostics;

namespace Sudoku.HumanTechniqueExploration
{
    public class HumanTechniqueExploration : ISolverSudoku
    {
       

        public Puzzle ConvertSudokuGridToPuzzle(SudokuGrid s)
        {

            Puzzle p = new Puzzle(s.Cells, false);

            return p;
        }

        public SudokuGrid ConvertPuzzleToSudokuGrid(Puzzle p)
        {
            return new SudokuGrid() { Cells = p.Columns.Select(r => r.Select(c => c.Value).ToArray()).ToArray() };
        }

        public SudokuGrid Solve(SudokuGrid s)
        {
            Puzzle p = ConvertSudokuGridToPuzzle(s);

            bool solved = false; // If this is true after a segment, the puzzle is solved and we can break
            bool full = false;
            List<Cell> allCells = null;
            bool deadEnd = false;
            Stack<BackTrackingState> exploredCellValues = null;
            p.RefreshCandidates();

            do
            {
                deadEnd = false;

                // First we do human inference
                do
                {
                    full = true;

                    bool changed = false;
                    // Check for naked singles or a completed puzzle
                    for (int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            Cell cell = p[x, y];
                            if (cell.Value == 0)
                            {
                                full = false;
                                // Check for naked singles
                                int[] a = cell.Candidates.ToArray(); // Copy
                                if (a.Length == 1)
                                {
                                    cell.Set(a[0]);
                                    changed = true;
                                }
                            }
                        }
                    }
                    // Solved or failed to solve
                    if (full || (!changed && !SolverTechnique.RunTechnique(p)))
                    {
                        break;
                    }

                } while (true);

                full = p.Rows.All(row => row.All(c => c.Value != 0));

                if (!full)
                {
                    // Les Sudokus les plus difficiles ne peuvent pas être résolus avec un stylo bille, c'est à dire en inférence pure.
                    // Il va falloir lacher le stylo bille et prendre le crayon à papier et la gomme pour commencer une exploration fondée sur des hypothèses avec possible retour en arrière
                    if (allCells == null)
                    {
                        allCells = p.Rows.SelectMany((row, rowIdx) => row).ToList();
                        exploredCellValues = new Stack<BackTrackingState>();
                    }
                    //puzzle.RefreshCandidates();

                    // Pour accélérer l'exploration et éviter de traverser la feuille en gommant trop souvent, on va utiliser les heuristiques des problèmes à satisfaction de contraintes
                    // cf. les slides et le problème du "coffre de voiture" abordé en cours

                    //heuristique MRV
                    var minCandidates = allCells.Min(cell => cell.Candidates.Count > 0 ? cell.Candidates.Count : int.MaxValue);

                    if (minCandidates != int.MaxValue)
                    {
                        // Utilisation de l'heuristique Deg: de celles qui ont le moins de candidats à égalité, on choisi celle la plus contraignante, celle qui a le plus de voisins (on pourrait faire mieux avec le nombre de candidats en commun avec ses voisins)
                        var candidateCells = allCells.Where(cell => cell.Candidates.Count == minCandidates);
                        //var degrees = candidateCells.Select(candidateCell => new {Cell = candidateCell, Degree = candidateCell.GetCellsVisible().Aggregate(0, (sum, neighbour) => sum + neighbour.Candidates.Count) });
                        var degrees = candidateCells.Select(candidateCell => new { Cell = candidateCell, Degree = candidateCell.GetCellsVisible().Count(c => c.Value == 0) }).ToList();
                        //var targetCell = list_cell.First(cell => cell.Candidates.Count == minCandidates);
                        var maxDegree = degrees.Max(deg1 => deg1.Degree);
                        var targetCell = degrees.First(deg => deg.Degree == maxDegree).Cell;

                        //dernière exploration pour ne pas se mélanger les pinceaux

                        BackTrackingState currentlyExploredCellValues;
                        if (exploredCellValues.Count == 0 || !exploredCellValues.Peek().Cell.Equals(targetCell))
                        {
                            currentlyExploredCellValues = new BackTrackingState() { Board = p.GetBoard(), Cell = targetCell, ExploredValues = new List<int>() };
                            exploredCellValues.Push(currentlyExploredCellValues);
                        }
                        else
                        {
                            currentlyExploredCellValues = exploredCellValues.Peek();
                        }


                        //utilisation de l'heuristique LCV: on choisi la valeur la moins contraignante pour les voisins
                        var candidateValues = targetCell.Candidates.Where(i => !currentlyExploredCellValues.ExploredValues.Contains(i));
                        var neighbourood = targetCell.GetCellsVisible();
                        var valueConstraints = candidateValues.Select(v => new
                        {
                            Value = v,
                            ContraintNb = neighbourood.Count(neighboor => neighboor.Candidates.Contains(v))
                        }).ToList();
                        var minContraints = valueConstraints.Min(vc => vc.ContraintNb);
                        var exploredValue = valueConstraints.First(vc => vc.ContraintNb == minContraints).Value;
                        currentlyExploredCellValues.ExploredValues.Add(exploredValue);
                        targetCell.Set(exploredValue);
                        //targetCell.Set(exploredValue, true);

                    }
                    else
                    {
                        //Plus de candidats possibles, on atteint un cul-de-sac
                        if (p.IsValid())
                        {
                            solved = true;
                        }
                        else
                        {
                            deadEnd = true;
                        }


                        //deadEnd = true;
                    }
                }
                else
                {
                    //If puzzle is full, it's either solved or a deadend
                    if (p.IsValid())
                    {
                        solved = true;
                    }
                    else
                    {
                        deadEnd = true;
                    }
                }


                if (deadEnd)
                {
                    //On se retrouve bloqué, il faut gommer et tenter d'autres hypothèses
                    BackTrackingState currentlyExploredCellValues = exploredCellValues.Peek();
                    //On annule la dernière assignation
                    currentlyExploredCellValues.Backtrack(p);
                    var targetCell = currentlyExploredCellValues.Cell;
                    //targetCell.Set(0, true);
                    while (targetCell.Candidates.All(i => currentlyExploredCellValues.ExploredValues.Contains(i)))
                    {
                        //on a testé toutes les valeurs possibles, On est à un cul de sac, il faut revenir en arrière
                        exploredCellValues.Pop();
                        if (exploredCellValues.Count == 0)
                        {
                            Debug.WriteLine("bug in the algorithm techniques humaines");
                        }
                        currentlyExploredCellValues = exploredCellValues.Peek();
                        //On annule la dernière assignation
                        currentlyExploredCellValues.Backtrack(p);
                        targetCell = currentlyExploredCellValues.Cell;
                        //targetCell.Set(0, true);
                    }
                    // D'autres valeurs sont possible pour la cellule courante, on les tente
                    //utilisation de l'heuristique LCV
                    var candidateValues = targetCell.Candidates.Where(i => !currentlyExploredCellValues.ExploredValues.Contains(i));
                    var neighbourood = targetCell.GetCellsVisible();
                    var valueConstraints = candidateValues.Select(v => new
                    {
                        Value = v,
                        ContraintNb = neighbourood.Count(neighboor => neighboor.Candidates.Contains(v))
                    }).ToList();
                    var minContraints = valueConstraints.Min(vc => vc.ContraintNb);
                    var exploredValue = valueConstraints.First(vc => vc.ContraintNb == minContraints).Value;
                    currentlyExploredCellValues.ExploredValues.Add(exploredValue);
                    targetCell.Set(exploredValue);
                }


            } while (!solved);
            return ConvertPuzzleToSudokuGrid(p);

        }

        public struct BackTrackingState
        {
            public Cell Cell { get; set; }

            public List<int> ExploredValues { get; set; }

            public int[][] Board { get; set; }


            public void Backtrack(Puzzle objPuzzle)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (objPuzzle[i, j].Value != Board[i][j])
                        {
                            objPuzzle[i, j].Set(Board[i][j]);
                        }
                    }
                }
                objPuzzle.RefreshCandidates();
            }
        }

       
    }


}

   