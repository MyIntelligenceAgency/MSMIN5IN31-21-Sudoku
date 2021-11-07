using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Sudoku.NorvigSolver
{
    public class Possible
    {

        private static bool[] _AllPossible = Enumerable.Repeat(true, 9).ToArray();

        private bool[] _b;

        public Possible()
        {
            _b = new bool[9];
            _AllPossible.CopyTo(_b,0);
        }

        public Possible(Possible copy)
        {
            _b = new bool[9];
            copy._b.CopyTo(this._b,0);
        }

        public Possible(int cellValue)
        {
            _b = new bool[9];
            _b[cellValue - 1] = true;
        }

        public bool IsOn(int i)
        {
            return _b[i - 1];
        }

        public int Count()
        {
            return _b.Count(b=>b); // prblème à ce niveau
        }

        public void Eliminate(int i)
        {
            _b[i - 1] = false;
        }

        public int Val()
        {

            var it = Array.IndexOf(_b, true);
            if (it!=-1)
            {
                it++;
            }

            return it;
        }

        //public string str(int width)
        //{
        //    string s = new string(' ', width);
        //    int k = 0;
        //    for (int i = 1; i <= 9; i++)
        //    {
        //        if (is_on(i))
        //        {
        //            s = Sudoku.StringFunctions.ChangeCharacter(s, k++, (char)('0' + i));
        //        }
        //    }
        //    return s;
        //}
       
    }

    public class Sudoku
    {
        private static int[] _CellsRange = Enumerable.Range(1, 81).ToArray();
       
        private static List<int>[] _group = Enumerable.Range(1,27).Select(i=>new List<int>(9)).ToArray();
        private static List<int>[] _groups_of = _CellsRange.Select(i => new List<int>(3)).ToArray();
        private static List<int>[] _neighbors = _CellsRange.Select(i => new List<int>(20)).ToArray();





        public Possible[] _cells;


        public Sudoku()
        {
            _cells = _CellsRange.Select(i => new Possible()).ToArray();
        }

        public Sudoku(string s):this()
        {
            //this._cells = 81;
            int k = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] >= '1' && s[i] <= '9')
                {
                    if (!assign(k, s[i] - '0'))
                    {
                        throw new InvalidOperationException($"Cannot assign sudoku {s}");
                    }
                    k++;
                }
                else if (s[i] == '0' || s[i] == '.')
                {
                    k++;
                }
            }
        }

        public Sudoku(Sudoku copy)
        {
            _cells = copy._cells.Select(c => new Possible(c)).ToArray();
        }


        static Sudoku()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int k = i * 9 + j;
                    int[] x = { i, 9 + j, 18 + (i / 3) * 3 + j / 3 };
                    for (int g = 0; g < 3; g++)
                    {
                        _group[x[g]].Add(k);
                        _groups_of[k].Add(x[g]);
                    }
                }
            }
            for (int k = 0; k < _neighbors.Length; k++)
            {
                for (int x = 0; x < _groups_of[k].Count; x++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        int k2 = _group[_groups_of[k][x]][j];
                        if (k2 != k)
                        {
                            _neighbors[k].Add(k2);
                        }
                    }
                }
            }
        }


        public Possible Possible(int k)
        {

            return _cells[k];
        }

        public bool is_solved()
        {
            for (int k = 0; k < _cells.Length; k++)
            {
                if (_cells[k].Count() != 1)
                {
                    return false;
                }
            }
            return true;
        }

        public bool assign(int k, int val)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (i != val)
                {
                    if (!eliminate(k, i))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool eliminate(int k, int val)
        {
            if (!_cells[k].IsOn(val))
            {
                return true;
            }
            _cells[k].Eliminate(val);
            int N = _cells[k].Count();
            if (N == 0)
            {
                return false;
            }
            else if (N == 1)
            {
                int v = _cells[k].Val();
                for (int i = 0; i < _neighbors[k].Count(); i++)
                {
                    if (!eliminate(_neighbors[k][i], v))
                    {
                        return false;
                    }
                }
            }
            for (int i = 0; i < _groups_of[k].Count(); i++)
            {
                int x = _groups_of[k][i];
                int n = 0;
                int ks = 0;
                for (int j = 0; j < 9; j++)
                {
                    int p = _group[x][j];
                    if (_cells[p].IsOn(val))
                    {
                        n++; ks = p;
                    }
                }
                if (n == 0)
                {
                    return false;
                }
                else if (n == 1)
                {
                    if (!assign(ks, val))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

       


        public int least_count()
        {
            int k = -1;
            int min = -1;
            for (int i = 0; i < _cells.Count(); i++)
            {
                int m = _cells[i].Count();
                if (m > 1 && (k == -1 || m < min))
                {
                    min = m; k = i;
                }
            }
            return k;
        }



        public static Sudoku Solve(Sudoku toSolve)
        {
            if (toSolve == null || toSolve.is_solved())
            {
                return toSolve;
            }
            int k = toSolve.least_count();
            Possible p = toSolve.Possible(k);
            for (int i = 1; i <= 9; i++)
            {
                if (p.IsOn(i))
                {
                    Sudoku S1 = new Sudoku(toSolve);
                    if (S1.assign(k, i))
                    {
                        var s2 = Solve(S1);
                        return s2;
                    }
                }
            }
            return null;
        }


    }
}
