using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Sudoku.NorvigSolver
{
	//public class Possible
	//{
	//	private List<bool> _b = new List<bool>();
	//	private Possible possible;

	//	public Possible()
	//	{
	//		this._b = Sudoku.VectorHelper.InitializedList(9, true);
	//	}

	//	public Possible(Possible possible)
	//	{
	//		this.possible = possible;
	//	}

	//	//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
	//	//ORIGINAL LINE: bool is_on(int i) const
	//	public bool is_on(int i)
	//	{
	//		return _b[i - 1];
	//	}
	//	//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
	//	//ORIGINAL LINE: int count() const
	//	public int count()
	//	{
	//		return count(_b.GetEnumerator(), _b.LastIndexOf, true);
	//	}
	//	public void eliminate(int i)
	//	{
	//		_b[i - 1] = false;
	//	}
	//	//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
	//	//ORIGINAL LINE: int val() const
	//	public int val()
	//	{
	//		var it = find(_b.GetEnumerator(), _b.LastIndexOf, true);
	//		return (it != _b.LastIndexOf ? 1 + (it - _b.GetEnumerator()) : -1);
	//	}
	//	//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
	//	//ORIGINAL LINE: string str(int width) const
	//	public string str(int width)
	//	{
	//		string s = new string(' ', width);
	//		int k = 0;
	//		for (int i = 1; i <= 9; i++)
	//		{
	//			if (is_on(i))
	//			{
	//				s = Sudoku.StringFunctions.ChangeCharacter(s, k++, (char)('0' + i));
	//			}
	//		}
	//		return s;
	//	}
	//}

	//public class Sudoku
	//{
	//	public List<Possible> _cells = new List<Possible>();
	//	private static List<List<int>> _group = new List<List<int>>();
	//	private static List<List<int>> _neighbors = new List<List<int>>(81);
	//	private static List<List<int>> _groups_of = new List<List<int>>(81);

	//	public Sudoku(Sudoku s)
	//	{
	//		S = s;
	//	}

	//	public Sudoku S { get; }

	//	//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//	//   bool eliminate(int k, int val);
	//	//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//	//   Sudoku(string s);
	//	public static void init()
	//	{
	//		for (int i = 0; i < 9; i++)
	//		{
	//			for (int j = 0; j < 9; j++)
	//			{
	//				int k = i * 9 + j;
	//				int[] x = { i, 9 + j, 18 + (i / 3) * 3 + j / 3 };
	//				for (int g = 0; g < 3; g++)
	//				{
	//					_group[x[g]].Add(k);
	//					_groups_of[k].Add(x[g]);
	//				}
	//			}
	//		}
	//		for (int k = 0; k < _neighbors.Count; k++)
	//		{
	//			for (int x = 0; x < _groups_of[k].Count; x++)
	//			{
	//				for (int j = 0; j < 9; j++)
	//				{
	//					int k2 = _group[_groups_of[k][x]][j];
	//					if (k2 != k)
	//					{
	//						_neighbors[k].Add(k2);
	//					}
	//				}
	//			}
	//		}
	//	}

	//	//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
	//	//ORIGINAL LINE: Possible possible(int k) const
	//	public Possible possible(int k)
	//	{
	//		//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
	//		//ORIGINAL LINE: return _cells[k];
	//		return new Possible(_cells[k]);
	//	}
	//	//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
	//	//ORIGINAL LINE: bool is_solved() const
	//	public bool is_solved()
	//	{
	//		for (int k = 0; k < _cells.Count; k++)
	//		{
	//			if (_cells[k].count() != 1)
	//			{
	//				return false;
	//			}
	//		}
	//		return true;
	//	}

	//	public bool eliminate(int k, int val)
	//	{
	//		if (!_cells[k].is_on(val))
	//		{
	//			return true;
	//		}
	//		_cells[k].eliminate(val);
	//		int N = _cells[k].count();
	//		if (N == 0)
	//		{
	//			return false;
	//		}
	//		else if (N == 1)
	//		{
	//			int v = _cells[k].val();
	//			for (int i = 0; i < _neighbors[k].size(); i++)
	//			{
	//				if (!eliminate(_neighbors[k][i], v))
	//				{
	//					return false;
	//				}
	//			}
	//		}
	//		for (int i = 0; i < _groups_of[k].size(); i++)
	//		{
	//			int x = _groups_of[k][i];
	//			int n = 0;
	//			int ks = 0;
	//			for (int j = 0; j < 9; j++)
	//			{
	//				int p = _group[x][j];
	//				if (_cells[p].is_on(val))
	//				{
	//					n++; ks = p;
	//				}
	//			}
	//			if (n == 0)
	//			{
	//				return false;
	//			}
	//			else if (n == 1)
	//			{
	//				if (!assign(ks, val))
	//				{
	//					return false;
	//				}
	//			}
	//		}
	//		return true;
	//	}

	//	public Sudoku(string s)
	//	{
	//		this._cells = 81;
	//		int k = 0;
	//		for (int i = 0; i < s.Length; i++)
	//		{
	//			if (s[i] >= '1' && s[i] <= '9')
	//			{
	//				if (!assign(k, s[i] - '0'))
	//				{
	//					Console.WriteLine("ERROR");
	//					return;
	//				}
	//				k++;
	//			}
	//			else if (s[i] == '0' || s[i] == '.')
	//			{
	//				k++;
	//			}
	//		}
	//	}


	//	public int least_count()
	//	{
	//		int k = -1;
	//		int min = -1;
	//		for (int i = 0; i < _cells.size(); i++)
	//		{
	//			int m = _cells[i].count();
	//			if (m > 1 && (k == -1 || m < min))
	//			{
	//				min = m; k = i;
	//			}
	//		}
	//		return k;
	//	}




	//	public bool assign(int k, int val)
	//	{
	//		for (int i = 1; i <= 9; i++)
	//		{
	//			if (i != val)
	//			{
	//				if (!eliminate(k, i))
	//				{
	//					return false;
	//				}
	//			}
	//		}
	//		return true;
	//	}

	//	//public static Sudoku Solve(Sudoku S)
	//	//{
	//	//	if (S == null || S.is_solved())
	//	//	{
	//	//		return new Sudoku(S);
	//	//	}
	//	//	int k = S.least_count();
	//	//	Possible p = S.possible(k);
	//	//	for (int i = 1; i <= 9; i++)
	//	//	{
	//	//		if (p.is_on(i))
	//	//		{
	//	//			Sudoku S1 = new Sudoku(S);
	//	//			if (S1.assign(k, i))
	//	//			{
	//	//				if (Solve(S1))
	//	//				{
	//	//					return S1;
	//	//				}
	//	//			}
	//	//		}
	//	//	}

	//	//internal static void Main()
	//	//{
	//	//	Sudoku.init();
	//	//	string line;
	//	//	while (line = Console.ReadLine())
	//	//	{
	//	//		if (S = Solve(Sudoku(new Sudoku(line))))
	//	//		{

	//	//			Console.WriteLine(S.write);


	//	//				else
	//	//			{
	//	//				Console.Write("No solution");
	//	//			}
	//	//			Console.Write("\n");
	//	//		}
	//	//	}
	//	//}



	//	//}





	//	//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
	//	//ORIGINAL LINE: int least_count() const;
	//	//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//	//   int least_count();
	//	//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
	//	//ORIGINAL LINE: void write(ostream& o) const
	//	public void write(IStream o)
	//	{
	//		int width = 1;
	//		for (int k = 0; k < _cells.Count; k++)
	//		{
	//			width = Math.Max(width, 1 + _cells[k].count());
	//		}
	//		string sep = new string('-', 3 * width);
	//		for (int i = 0; i < 9; i++)
	//		{
	//			if (i == 3 || i == 6)
	//			{
	//				o << sep << "+-" << sep << "+" << sep << "\n";
	//			}
	//			for (int j = 0; j < 9; j++)
	//			{
	//				if (j == 3 || j == 6)
	//				{
	//					o << "| ";
	//				}
	//				o << _cells[i * 9 + j].str(width);
	//			}
	//			o << "\n";
	//		}
	//	}


	//	//C++ TO C# CONVERTER TODO TASK: The following statement was not recognized, possibly due to an unrecognized macro:
	//	List<List<int>> Sudoku._group(27);

	//	//Helper class added by C++ to C# Converter:

	//	//----------------------------------------------------------------------------------------
	//	//	Copyright © 2006 - 2021 Tangible Software Solutions, Inc.
	//	//	This class can be used by anyone provided that the copyright notice remains intact.
	//	//
	//	//	This class is used to convert some of the C++ std::vector methods to C#.
	//	//----------------------------------------------------------------------------------------


	//	internal static class VectorHelper
	//	{
	//		public static void Resize<T>(this List<T> list, int newSize, T value = default(T))
	//		{
	//			if (list.Count > newSize)
	//				list.RemoveRange(newSize, list.Count - newSize);
	//			else if (list.Count < newSize)
	//			{
	//				for (int i = list.Count; i < newSize; i++)
	//				{
	//					list.Add(value);
	//				}
	//			}
	//		}

	//		public static void Swap<T>(this List<T> list1, List<T> list2)
	//		{
	//			List<T> temp = new List<T>(list1);
	//			list1.Clear();
	//			list1.AddRange(list2);
	//			list2.Clear();
	//			list2.AddRange(temp);
	//		}

	//		public static List<T> InitializedList<T>(int size, T value)
	//		{
	//			List<T> temp = new List<T>();
	//			for (int count = 1; count <= size; count++)
	//			{
	//				temp.Add(value);
	//			}

	//			return temp;
	//		}

	//		public static List<List<T>> NestedList<T>(int outerSize, int innerSize)
	//		{
	//			List<List<T>> temp = new List<List<T>>();
	//			for (int count = 1; count <= outerSize; count++)
	//			{
	//				temp.Add(new List<T>(innerSize));
	//			}

	//			return temp;
	//		}

	//		public static List<List<T>> NestedList<T>(int outerSize, int innerSize, T value)
	//		{
	//			List<List<T>> temp = new List<List<T>>();
	//			for (int count = 1; count <= outerSize; count++)
	//			{
	//				temp.Add(InitializedList(innerSize, value));
	//			}

	//			return temp;
	//		}
	//	}

	//	//Helper class added by C++ to C# Converter:

	//	//----------------------------------------------------------------------------------------
	//	//	Copyright © 2006 - 2021 Tangible Software Solutions, Inc.
	//	//	This class can be used by anyone provided that the copyright notice remains intact.
	//	//
	//	//	This class provides the ability to replicate various classic C string functions
	//	//	which don't have exact equivalents in the .NET Framework.
	//	//----------------------------------------------------------------------------------------
	//	internal static class StringFunctions
	//	{
	//		//------------------------------------------------------------------------------------
	//		//	This method allows replacing a single character in a string, to help convert
	//		//	C++ code where a single character in a character array is replaced.
	//		//------------------------------------------------------------------------------------
	//		public static string ChangeCharacter(string sourceString, int charIndex, char newChar)
	//		{
	//			return (charIndex > 0 ? sourceString.Substring(0, charIndex) : "")
	//				+ newChar.ToString() + (charIndex < sourceString.Length - 1 ? sourceString.Substring(charIndex + 1) : "");
	//		}

	//		//------------------------------------------------------------------------------------
	//		//	This method replicates the classic C string function 'isxdigit' (and 'iswxdigit').
	//		//------------------------------------------------------------------------------------
	//		public static bool IsXDigit(char character)
	//		{
	//			if (char.IsDigit(character))
	//				return true;
	//			else if ("ABCDEFabcdef".IndexOf(character) > -1)
	//				return true;
	//			else
	//				return false;
	//		}

	//		//------------------------------------------------------------------------------------
	//		//	This method replicates the classic C string function 'strchr' (and 'wcschr').
	//		//------------------------------------------------------------------------------------
	//		public static string StrChr(string stringToSearch, char charToFind)
	//		{
	//			int index = stringToSearch.IndexOf(charToFind);
	//			if (index > -1)
	//				return stringToSearch.Substring(index);
	//			else
	//				return null;
	//		}

	//		//------------------------------------------------------------------------------------
	//		//	This method replicates the classic C string function 'strrchr' (and 'wcsrchr').
	//		//------------------------------------------------------------------------------------
	//		public static string StrRChr(string stringToSearch, char charToFind)
	//		{
	//			int index = stringToSearch.LastIndexOf(charToFind);
	//			if (index > -1)
	//				return stringToSearch.Substring(index);
	//			else
	//				return null;
	//		}

	//		//------------------------------------------------------------------------------------
	//		//	This method replicates the classic C string function 'strstr' (and 'wcsstr').
	//		//------------------------------------------------------------------------------------
	//		public static string StrStr(string stringToSearch, string stringToFind)
	//		{
	//			int index = stringToSearch.IndexOf(stringToFind);
	//			if (index > -1)
	//				return stringToSearch.Substring(index);
	//			else
	//				return null;
	//		}

	//		//------------------------------------------------------------------------------------
	//		//	This method replicates the classic C string function 'strtok' (and 'wcstok').
	//		//	Note that the .NET string 'Split' method cannot be used to replicate 'strtok' since
	//		//	it doesn't allow changing the delimiters between each token retrieval.
	//		//------------------------------------------------------------------------------------
	//		private static string activeString;
	//		private static int activePosition;
	//		public static string StrTok(string stringToTokenize, string delimiters)
	//		{
	//			if (stringToTokenize != null)
	//			{
	//				activeString = stringToTokenize;
	//				activePosition = -1;
	//			}

	//			//the stringToTokenize was never set:
	//			if (activeString == null)
	//				return null;

	//			//all tokens have already been extracted:
	//			if (activePosition == activeString.Length)
	//				return null;

	//			//bypass delimiters:
	//			activePosition++;
	//			while (activePosition < activeString.Length && delimiters.IndexOf(activeString[activePosition]) > -1)
	//			{
	//				activePosition++;
	//			}

	//			//only delimiters were left, so return null:
	//			if (activePosition == activeString.Length)
	//				return null;

	//			//get starting position of string to return:
	//			int startingPosition = activePosition;

	//			//read until next delimiter:
	//			do
	//			{
	//				activePosition++;
	//			} while (activePosition < activeString.Length && delimiters.IndexOf(activeString[activePosition]) == -1);

	//			return activeString.Substring(startingPosition, activePosition - startingPosition);
	//		}
	//	}



	//}
}
