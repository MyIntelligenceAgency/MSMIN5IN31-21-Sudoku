# représentation d'un sudoku https://github.com/aimacode/aima-python/blob/master/csp.py#L726
# puzzle = '..3.2.6..9..3.5..1..18.64....81.29..7.......8..67.82....26.95..8..2.3..9..5.1.3..' fonctionne également avec des 0 à la place des .
import aima3
from aima3.csp import *
s = Sudoku(puzzle)
result = min_conflicts(s, max_steps=1000000)
#s.display(result)
solution =   [[result.get(cell) for cell in row] for row in s.rows]
#print(solution)