namespace NorvigSecondSolver
{
    public class SudokuBase
    {

        public void write(ostream o)
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
                    o << sep << "+-" << sep << "+" << sep << "\n";
                }
                for (int j = 0; j < 9; j++)
                {
                    if (j == 3 || j == 6)
                    {
                        o << "| ";
                    }
                    o << _cells[i * 9 + j].str(width);
                }
                o << "\n";
            }
        }
    }
}