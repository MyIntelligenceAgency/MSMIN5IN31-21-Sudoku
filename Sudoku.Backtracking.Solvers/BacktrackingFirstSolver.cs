using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.Shared;

namespace Sudoku.Backtracking.Solvers
{
   public class BacktrackingFirstSolver : ISolverSudoku
    {
        public SudokuGrid Solve(SudokuGrid s)
        {

           var solved = Backtrack(s, 0, 0);
           if (!solved)
           {
               Console.WriteLine("Could not solve Sudoku");
           }
            return s;
        }

        public bool Backtrack(SudokuGrid s, int x, int y)
        {
            if (x == 8 && y == 8 && s.Cells[8][8] == 0)
            {
                int[] possibilie = s.GetPossibilities(x, y);
                //Attention, on peut bloquer à la dernière case !
                // En l'occurence, le sudoku Medium 3 présente ce cas, l'ajout des lignes suivantes empêche le déclanchement d'une exception mais le sudoku demeure irrésolu.
                if (possibilie.Length == 0)
                {
                    return false;
                }
                s.Cells[x][y] = possibilie[0];
                return true;
            }
            if (x == 8 && y == 8 && s.Cells[8][8] != 0)
            {
                return true;
            }
            if (x > 8)
            {
                return Backtrack(s, 0, y + 1);
            }
            if (s.Cells[x][y] != 0)
            {
                return Backtrack(s, x + 1, y);
            }
            int[] possibilies = s.GetPossibilities(x, y);
            foreach (int i in possibilies)
            {
                s.Cells[x][y] = i;
                if (Backtrack(s, x + 1, y))
                {
                    return true;
                }
                else
                {
                    s.Cells[x][y] = 0;
                }
            }
            return false;
        }

    }

    /// <summary>
    /// Ce solver alternatif est issu du travail d'autres élèves que j'ai rapidemment adapté.
    /// Il résout correctement le Sudoku Medium 3, et de façon intéressante que vous ne manquerez pas d'analyser, il est beaucoup plus rapide que le vôtre sur certains Sudoku, et beaucoup plus lent sur d'autres (!!!)
    /// </summary>
    public class BacktrackingAltSolver : ISolverSudoku
    {
        private const int Size = 9;

        public SudokuGrid Solve(SudokuGrid s)
        {
            Resolve(s);
            return s;
        }

        public bool Resolve(SudokuGrid s)  // La fonction solve va pemettre de choisir la solution valide satsifaisant les contraintes de sodoku
        {
            for (int ligne = 0; ligne < Size; ligne++)  // parcourt les lignes
            {
                for (int col = 0; col < Size; col++)  // parcourt les colonnes 
                {
                    // Ce double for parcourt chaque colonne pour une ligne donnée
                    if (s.Cells[ligne][col] == 0) // empty: si la cellule est vide alors on va rentrer dans la boucle for
                    {
                        for (int value = 1; value <= 9; value++) // qui va tester les valeur de 1 à 9
                        {
                            if (IsValid(s, ligne, col, value)) // Si la valeur est valide avec les conditions de validité qu'on va définirapres dans le code
                            {

                                s.Cells[ligne][col]= value; // Alors on remplie la case vide avec la valeur valide

                                if (Resolve(s))  // Et on appelle la fonction elle-meme pour la récursivité : Principe de backtracking
                                {
                                    return true; // On on valide la solution
                                }
                                else
                                {
                                    s.Cells[ligne][col] = 0;  // Sinon on réinitialise la valeur à 0
                                }
                            }
                        }

                        return false; // On refait le même processus
                    }
                }
            }

            return true; // Jusqu'à ce qu'on trouve une solution valide et on la garde
        }

        private bool IsValid(SudokuGrid s, int ligne, int col, int value)
        {
            return IsLigneValid(s, ligne, value) &&
                   IsCarreValid(s, ligne, col, value) &&
                   IsColValid(s, col, value);
        }


        private bool IsLigneValid(SudokuGrid s, int ligne, int value)
        //Vérifie que chaque nombre n'apparait qu'une fois sur la ligne
        {

            for (var col = 0; col < Size; col++)
            {
                if (s.Cells[ligne][col] == value)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsColValid(SudokuGrid s, int col, int value)
        //Vérifie que chaque nombre n'apparait qu'une fois sur la colonne
        {
            for (var ligne = 0; ligne < Size; ligne++)
            {
                if (s.Cells[ligne][col] == value)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsCarreValid(SudokuGrid s, int ligne, int col, int value)
        //Vérifie que chaque valeur n'apparait qu'une fois dans le carré 
        {
            var l = ligne - ligne % 3;
            var c = col - col % 3;

            for (var i = l; i < l + 3; i++)
            {
                for (var j = c; j < c + 3; j++)
                {
                    if (s.Cells[i][j] == value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }


}
