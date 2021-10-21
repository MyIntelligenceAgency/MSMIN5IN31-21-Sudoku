using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Octokit;
using System.IO;

namespace NorvigSecondSolver
{


	internal static class VectorHelper
	{
		public static void Resize<T>(this List<T> list, int newSize, T value = default(T))
		{
			if (list.Count > newSize)
				list.RemoveRange(newSize, list.Count - newSize);
			else if (list.Count < newSize)
			{
				for (int i = list.Count; i < newSize; i++)
				{
					list.Add(value);
				}
			}
		}

		public static void Swap<T>(this List<T> list1, List<T> list2)
		{
			List<T> temp = new List<T>(list1);
			list1.Clear();
			list1.AddRange(list2);
			list2.Clear();
			list2.AddRange(temp);
		}

		public static List<T> InitializedList<T>(int size, T value)
		{
			List<T> temp = new List<T>();
			for (int count = 1; count <= size; count++)
			{
				temp.Add(value);
			}

			return temp;
		}

		public static List<List<T>> NestedList<T>(int outerSize, int innerSize)
		{
			List<List<T>> temp = new List<List<T>>();
			for (int count = 1; count <= outerSize; count++)
			{
				temp.Add(new List<T>(innerSize));
			}

			return temp;
		}

		public static List<List<T>> NestedList<T>(int outerSize, int innerSize, T value)
		{
			List<List<T>> temp = new List<List<T>>();
			for (int count = 1; count <= outerSize; count++)
			{
				temp.Add(InitializedList(innerSize, value));
			}

			return temp;
		}
	}

	internal static class StringFunctions
	{

		public static string ChangeCharacter(string sourceString, int charIndex, char newChar)
		{
			return (charIndex > 0 ? sourceString.Substring(0, charIndex) : "")
				+ newChar.ToString() + (charIndex < sourceString.Length - 1 ? sourceString.Substring(charIndex + 1) : "");
		}

		public static bool IsXDigit(char character)
		{
			if (char.IsDigit(character))
				return true;
			else if ("ABCDEFabcdef".IndexOf(character) > -1)
				return true;
			else
				return false;
		}

		public static string StrChr(string stringToSearch, char charToFind)
		{
			int index = stringToSearch.IndexOf(charToFind);
			if (index > -1)
				return stringToSearch.Substring(index);
			else
				return null;
		}

		public static string StrRChr(string stringToSearch, char charToFind)
		{
			int index = stringToSearch.LastIndexOf(charToFind);
			if (index > -1)
				return stringToSearch.Substring(index);
			else
				return null;
		}

		public static string StrStr(string stringToSearch, string stringToFind)
		{
			int index = stringToSearch.IndexOf(stringToFind);
			if (index > -1)
				return stringToSearch.Substring(index);
			else
				return null;
		}

		private static string activeString;
		private static int activePosition;
		public static string StrTok(string stringToTokenize, string delimiters)
		{
			if (stringToTokenize != null)
			{
				activeString = stringToTokenize;
				activePosition = -1;
			}

			if (activeString == null)
				return null;

			if (activePosition == activeString.Length)
				return null;

			activePosition++;
			while (activePosition < activeString.Length && delimiters.IndexOf(activeString[activePosition]) > -1)
			{
				activePosition++;
			}

			//only delimiters were left, so return null:
			if (activePosition == activeString.Length)
				return null;

			//get starting position of string to return:
			int startingPosition = activePosition;

			//read until next delimiter:
			do
			{
				activePosition++;
			} while (activePosition < activeString.Length && delimiters.IndexOf(activeString[activePosition]) == -1);

			return activeString.Substring(startingPosition, activePosition - startingPosition);
		}
	}


	public class Possible
	{
		private List<bool> _b = new List<bool>();
		public Possible(Possible possible)
		{
			this._b = VectorHelper.InitializedList(9, true);
		}

		public bool is_on(int i)
		{
			return _b[i - 1];
		}

		public int count()
		{
			return count(_b.GetEnumerator(), _b.end(), true);
		}
		public void eliminate(int i)
		{
			_b[i - 1] = false;
		}

		public int val()
		{
			var it = Find(_b.GetEnumerator(), _b.end(), true);
			return (it != _b.end() ? 1 + (it - _b.GetEnumerator()) : -1);
		}
		public string str(int width)
		{
			string s = new string(' ', width);
			int k = 0;
			for (int i = 1; i <= 9; i++)
			{
				if (is_on(i))
				{
					s = StringFunctions.ChangeCharacter(s, k++, (char)('0' + i));
				}
			}
			return s;
		}
	}

	public class Sudoku
	{
		private List<Possible> _cells = new List<Possible>();
		private static List<List<int>> _group = new List<List<int>>();
		private static List<List<int>> _neighbors = new List<List<int>>(81);
		private static List<List<int>> _groups_of = new List<List<int>>(81);

		public Possible possible(int k)
		{

			return new Possible(_cells[k]);
		}

		public bool is_solved()
		{
			for (int k = 0; k < _cells.Count; k++)
			{
				if (_cells[k].count() != 1)
				{
					return false;
				}
			}
			return true;
		}

		public void write(Stream o)
		{
			int width = 1;
			for (int k = 0; k < _cells.Count; k++)
			{
				width = Math.Max(width, 1 + _cells[k].count());
			}
			string sep = new string('-', 3 * width);
			for (int i = 0; i < 9; i++)
			{
				if (i == 3 || i == 6)
				{
					Console.WriteLine(o);
					
				}
				for (int j = 0; j < 9; j++)
				{
					if (j == 3 || j == 6)
					{
						o;
					}
					o << _cells[i * 9 + j].str(width);
				}
				
			}
		}

		List<List<int>> Sudoku._group(27);

		public static void init()
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
			for (int k = 0; k < _neighbors.Count; k++)
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

		private bool eliminate(int k, int val)
		{
			if (!_cells[k].is_on(val))
			{
				return true;
			}
			_cells[k].eliminate(val);
			int N = _cells[k].count();
			if (N == 0)
			{
				return false;
			}
			else if (N == 1)
			{
				int v = _cells[k].val();
				for (int i = 0; i < _neighbors[k].Count; i++)
				{
					if (!eliminate((new List<List<int>>(_neighbors[k][i]), 10), v))
					{
						return false;
					}
				}
			}
			for (int i = 0; i < _groups_of[k].Count; i++)
			{
				int x = _groups_of[k][i];
				int n = 0;
				int ks = 0;
				for (int j = 0; j < 9; j++)
				{
					int p = _group[x][j];
					if (_cells[p].is_on(val))
					{
						int v = n++; ks = p;
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
			int min = 0;
			for (int i = 0; i < _cells.Count; i++)
			{
				int m = _cells[i].count();
				if (m > 1 && (k == -1 || m < min))
				{
					min = m; k = i;
				}
			}
			return k;
		}

		public Sudoku(string s)
		{
			this._cells = 81;
			int k = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] >= '1' && s[i] <= '9')
				{
					if (!assign(k, s[i] - '0'))
					{
						Console.WriteLine("ERROR");
						return;
					}
					k++;
				}
				else if (s[i] == '0' || s[i] == '.')
				{
					k++;
				}
			}
		}



	}


}

	            

           

    
	

	








	
