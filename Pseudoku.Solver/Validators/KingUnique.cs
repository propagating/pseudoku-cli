using System.Collections.Generic;
using System.Linq;

namespace Pseudoku.Solver.Validators
{
    public class KingUnique : IConstraintValidator
    {
        public int ValidatorDifficulty { get; set; } = 1;
        public static readonly List<(int, int)> KingMoves = new List<(int, int)> {(1, 1), (1, -1), (-1, 1), (-1, -1)};
        public bool ValidatePotentialCellValues(PseudoCell cell, PseudoBoard board, out string solveMessage)
        {
            var startCount = cell.PossibleValues.Count;
            solveMessage = "";
            foreach (var move in KingMoves.ToList())
            {
                var moveVertical   = cell.CellRow + move.Item1;
                var moveHorizontal = cell.CellColumn + move.Item2;

                var existingValues = board.BoardCells.Where(x => x.CellRow == moveVertical
                                                                 && x.CellColumn == moveHorizontal
                                                                 && x.SolvedCell
                                                                 && cell.PossibleValues.Contains(x.CurrentValue)).ToList();
                foreach (var eCell in existingValues)
                {
                    solveMessage = $"{solveMessage}\nRemoved {eCell.CurrentValue} from R{cell.CellRow} C{cell.CellColumn} for conflict with R{eCell.CellRow} C{eCell.CellColumn} : King's Move Constraint";
                    cell.PossibleValues.Remove(eCell.CurrentValue);
                }

                if (cell.PossibleValues.Count == 1)
                {
                    cell.CurrentValue   = cell.PossibleValues.First(); //only 1 value remains.
                    solveMessage = $"{solveMessage}\nSolved for {cell.CurrentValue} in R{cell.CellRow} C{cell.CellColumn} : Naked Single King's Move";
                    cell.PossibleValues = new List<int>();
                    cell.SolvedCell     = true;
                    return true;
                }
            }

            return cell.PossibleValues.Count != startCount;
        }
    }
}
