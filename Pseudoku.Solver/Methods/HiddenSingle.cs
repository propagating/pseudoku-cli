using System.Collections.Generic;
using System.Linq;

namespace Pseudoku.Solver.Methods
{
    public class HiddenSingle : ISolveMethod
    {
        public int MethodDifficulty { get; set; } = 0;

        public bool ApplyMethod(PseudoCell cell, PseudoBoard board, out string solveMessage)
        {
            var startCount = cell.PossibleValues.Count;
            solveMessage = "";
            foreach (var value in cell.PossibleValues)
            {
                var rowCells = board.BoardCells.Where(x => x.CellRow == cell.CellRow).Where(x => x.PossibleValues.Contains(value)).Count();
                var colCells = board.BoardCells.Where(x=> x.CellColumn == cell.CellColumn).Where(x => x.PossibleValues.Contains(value)).Count();
                var boxCells = board.BoardCells.Where(x=> x.CellBox == cell.CellBox).Where(x => x.PossibleValues.Contains(value)).Count();

                if (rowCells == 1 || colCells == 1 || boxCells == 1)
                {
                    cell.CurrentValue = value;
                    solveMessage = $"{solveMessage}\nSolved for {cell.CurrentValue} in R{cell.CellRow} C{cell.CellColumn} : Hidden Single";
                    cell.PossibleValues = new List<int>();
                    cell.SolvedCell = true;
                    return true;
                }
            }
            return cell.PossibleValues.Count != startCount;
        }

    }
}
