using System.Collections.Generic;
using System.Linq;

namespace Pseudoku.Solver.Methods
{
    public class IntersectionRemoval : ISolveMethod
    {
        public int MethodDifficulty { get; set; } = 1;

        public bool ApplyMethod(PseudoCell cell, PseudoBoard board, out string solveMessage)
        {
            var startCount = cell.PossibleValues.Count;
            solveMessage = "";
            foreach (var value in cell.PossibleValues)
            {
                var sharedBoxCount = 0;
                var sharedBoxCells = board.BoardCells.Where(x => x.CellBox == cell.CellBox && cell.PossibleValues.Contains(value)).ToList();
                sharedBoxCount = sharedBoxCells.Count;
                if (sharedBoxCells.Any())
                {
                    if (sharedBoxCells.Where(x => x.CellRow == cell.CellRow).Count() == sharedBoxCount)
                    {
                        foreach (var boardCell in board.BoardCells.Where(x => x.CellRow == cell.CellRow && x.CellBox != cell.CellBox))
                        {
                            boardCell.PossibleValues.Remove(value);
                            solveMessage = $"{solveMessage}\nRemoved {value} from R{cell.CellRow} C{cell.CellColumn} for conflict with R{boardCell.CellRow} C{boardCell.CellColumn} : Intersection Removal";
                        }
                    }

                    if (sharedBoxCells.Where(x => x.CellColumn == cell.CellColumn).Count() == sharedBoxCount)
                    {
                        foreach (var boardCell in board.BoardCells.Where(x => x.CellColumn == cell.CellColumn && x.CellBox != cell.CellBox))
                        {
                            boardCell.PossibleValues.Remove(value);
                            solveMessage = $"{solveMessage}\nRemoved {value} from R{cell.CellRow} C{cell.CellColumn} for conflict with R{boardCell.CellRow} C{boardCell.CellColumn} : Intersection Removal";

                        }
                    }
                }
            }


            if (cell.PossibleValues.Count == 1)
            {

                cell.CurrentValue   = cell.PossibleValues.First(); //only 1 value remains.
                solveMessage = $"{solveMessage}\nSolved for {cell.CurrentValue} in R{cell.CellRow} C{cell.CellColumn} : Naked Single Intersection Removal";
                cell.PossibleValues = new List<int>();
                cell.SolvedCell     = true;
            }
            return cell.PossibleValues.Count != startCount;
        }
    }
}
