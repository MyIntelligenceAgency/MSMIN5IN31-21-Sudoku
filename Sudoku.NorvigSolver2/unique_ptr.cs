namespace Sudoku.NorvigSolver2
{
    public class unique_ptr<T>
    {
        public unique_ptr(unique_ptr<Sudoku> s)
        {
            S = s;
        }

        public unique_ptr<Sudoku> S { get; }
    }
}