using System.Collections.Generic;
using System.Linq;

namespace Pseudoku.Solver.Validators
{
    public class NoneConsecutive : IConstraintValidator
    {
        public int ValidatorDifficulty { get; set; } = 2;
        public static readonly List<(int,int)> AdjacentCells = new List<(int, int)>{(1 , 1), (1 , -1), (-1, 1), (-1, -1)};
        public bool ValidatePotentialCellValues(PseudoCell cell, PseudoBoard board, out string solveMessage)
        {
            solveMessage = "";
            var startCount = cell.PossibleValues.Count; //can be used in case we need to implement guessing as a way to rollback changes
            foreach (var move in AdjacentCells.ToList())
            {
                var moveVertical   = cell.CellRow + move.Item1;
                var moveHorizontal = cell.CellColumn + move.Item2;

                if(moveVertical < 1 || moveHorizontal < 1) {continue;}

                var existingValues = board.BoardCells.Where(x => x.CellRow == moveVertical
                                                                 && x.CellColumn == moveHorizontal
                                                                 && x.CellBox != cell.CellBox
                                                                 && x.SolvedCell
                                                                 && cell.PossibleValues.Contains(x.CurrentValue)).ToList();
                foreach (var eCell in existingValues)
                {
                    var consecutiveValues = new List<int> { eCell.CurrentValue + 1, eCell.CurrentValue - 1 };
                    foreach (var value in consecutiveValues)
                    {
                        solveMessage = $"{solveMessage}\nRemoved {value} from R{cell.CellRow} C{cell.CellColumn} for conflict with R{eCell.CellRow} C{eCell.CellColumn} : None Consecutive Constraint";
                        cell.PossibleValues.Remove(value);
                    }
                }

                if (cell.PossibleValues.Count == 1)
                {
                    cell.CurrentValue   = cell.PossibleValues.First(); //only 1 value remains.
                    solveMessage = $"{solveMessage}\nSolved for {cell.CurrentValue} in R{cell.CellRow} C{cell.CellColumn} : Naked Single Knight's Move";
                    cell.PossibleValues = new List<int>();
                    cell.SolvedCell     = true;
                    return true;
                }

            }

            return cell.PossibleValues.Count != startCount;
        }
    }
}
