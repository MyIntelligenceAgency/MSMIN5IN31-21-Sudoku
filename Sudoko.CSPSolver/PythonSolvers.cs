using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Python.Included;
using Python.Runtime;
using Sudoko.CSPSolver;
using Sudoku.Shared;

namespace Sudoku.CSPSolvers
{

    public abstract class PythonSolverBase : ISolverSudoku
    {

        public PythonSolverBase()
        {
            SetupPython();
        }

        protected void SetupPython()
        {
            InstallPythonComponents();
            InitializePythonComponents();

        }

        protected virtual void InstallPythonComponents()
        {
            Installer.SetupPython().Wait();
        }

        protected virtual void InitializePythonComponents()
        {
            PythonEngine.Initialize();
            dynamic sys = PythonEngine.ImportModule("sys");
            Console.WriteLine("Python version: " + sys.version);
        }



        public abstract SudokuGrid Solve(SudokuGrid s);

    }

    public abstract class CspPipSolverBase : PythonSolverBase
    {


        public override SudokuGrid Solve(SudokuGrid s)
        {

            //using (Py.GIL())
            //{
            // create a Python scope
            using (PyScope scope = Py.CreateScope())
            {

                //Create one liner string sudoku as consumed by the pip Sudoku class
                var strSudoku = s.Cells.Aggregate("",
                    (sRows, row) => sRows + row.Aggregate("",
                        (sCells, cell) => sCells + cell.ToString(CultureInfo.InvariantCulture)));
                // convert the string object to a PyObject

                PyObject pyCells = strSudoku.ToPython();

                // create a Python variable "puzzle" according to the python script
                scope.Set("puzzle", pyCells);

                // the puzzle object may now be used in Python
                string code = GetPythonScript();
                scope.Exec(code);
                var pySolution = scope.Get("solution");
                var managedSolution = pySolution.As<int[][]>();
                var toReturn = new SudokuGrid() { Cells = managedSolution };
                return toReturn;
            }
            //}

        }

        protected abstract string GetPythonScript();

        protected override void InstallPythonComponents()
        {
            base.InstallPythonComponents();
            Installer.TryInstallPip();

            Installer.PipInstallModule("aima3");

        }


    }

    // Global search with no heuristic and no inference
  

    public class CspAimaFuvUdvPythonPipSolver : CspPipSolverBase
    {
        //Solver with basic AIMA Pip parameters

        protected override string GetPythonScript()
        {
            return Resources.CspAimaFuvUdvPipSolver_py;
        }

    }


    // Adding search heuristics


    public class CspAimaMrvUdvPythonPipSolver : CspPipSolverBase
    {
        //Solver to evaluate the influence of the mrv (minimum-remaining-values heuristic) parameter

        protected override string GetPythonScript()
        {
            return Resources.CspAimaMrvUdvPipSolver_py;
        }
    }




    public class CspAimaFuvLcvPythonPipSolver : CspPipSolverBase
    {
        //Solver to evaluate the influence of the  mrv (minimum-remaining-values heuristic) and lcv (least constraining-values heursitic) parameter

        protected override string GetPythonScript()
        {
            return Resources.CspAimaFuvLcvPipSolver_py;
        }

    }

    public class CspAimaMrvLcvPythonPipSolver : CspPipSolverBase
    {
        //Solver to evaluate the influence of the lcv (least constraining-values heursitic) parameter

        protected override string GetPythonScript()
        {
            return Resources.CspAimaMrvLcvPipSolver_py;
        }

    }

    // Adding inference (domain reduction)

    public class CspAimaFuvUdvMacPythonPipSolver : CspPipSolverBase
    {
        //Solver to evaluate the influence of the mac (Maintain arc consistency inference AC-3) parameter
        protected override string GetPythonScript()
        {
            return Resources.CspAimaFuvUdvMacPipSolver_py;
        }


    }

    public class CspAimaFuvUdvFcPythonPipSolver : CspPipSolverBase
    {
        //Solver to evaluate the influence of the forward_checking parameter (inference : Prune neighbor values inconsistent with var=value)

        protected override string GetPythonScript()
        {
            return Resources.CspAimaFuvUdvFcPipSolver_py;
        }

    }


    // Combining search heuristics and inference

    public class CspAimaMrvLcvMacPythonPipSolver : CspPipSolverBase
    {

        //Solver combining mrv (minimum-remaining-values heuristic) LCV (least-constraint-value heuristic) and Mac inference (AC-3) parameters

        protected override string GetPythonScript()
        {
            return Resources.CspAimaMrvLcvMacPipSolver_py;
        }


    }

    // Best results (see Norvig method for an analog)

    public class CspAimaMrvLcvFcPythonPipSolver : CspPipSolverBase
    {

        //Solver combining mrv (minimum-remaining-values heuristic) LCV (least-constraint-value heuristic) and Forward Checking infernece parameters

        protected override string GetPythonScript()
        {
            return Resources.CspAimaMrvLcvFcPipSolver_py;
        }


    }

    //Switching to local search: poor result because of non dense solutions in state landscape unlike classical planning or NQueens problem
    public class CspAimaMinConflictsPythonPipSolver : CspPipSolverBase
    {
        //Solver using local search with MinConflicts heuristics instead of backtracking global search

        protected override string GetPythonScript()
        {
            return Resources.CspAimaMinConflictsPipSolver_py;
        }

    }


}
