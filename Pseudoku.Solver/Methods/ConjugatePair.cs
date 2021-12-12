using System.Linq;

namespace Pseudoku.Solver.Methods
{
    public class ConjugatePair : ISolveMethod
    {
        public int MethodDifficulty { get; set; } = 1;

        public bool ApplyMethod(PseudoCell cell, PseudoBoard board, out string solveMessage)
        {
            var startCount = cell.PossibleValues.Count;
            solveMessage = "";
            var valueRemoved = false;
            switch (startCount)
            {
                case 2:
                    var matchedBox = board.BoardCells
                                           .Where(x=> x.CellBox == cell.CellBox && x.CellRow != cell.CellRow && x.CellColumn != cell.CellColumn)
                                           .Where(x => x.PossibleValues.Intersect(cell.PossibleValues).Count() == x.PossibleValues.Count()
                                                      && x.PossibleValues.Intersect(cell.PossibleValues).Count() == cell.PossibleValues.Count()).ToList();
                    var matchedRow = board.BoardCells
                                          .Where(x=>  x.CellRow == cell.CellRow && x.CellColumn != cell.CellColumn)
                                          .Where(x => x.PossibleValues.Intersect(cell.PossibleValues).Count() == x.PossibleValues.Count()
                                                      && x.PossibleValues.Intersect(cell.PossibleValues).Count() == cell.PossibleValues.Count()).ToList();
                    var matchedCol = board.BoardCells
                                          .Where(x=> x.CellColumn == cell.CellColumn && x.CellRow != cell.CellRow)
                                          .Where(x => x.PossibleValues.Intersect(cell.PossibleValues).Count() == x.PossibleValues.Count()
                                                      && x.PossibleValues.Intersect(cell.PossibleValues).Count() == cell.PossibleValues.Count()).ToList();
                    if (matchedBox.Any())
                    {
                        var nonPairs = board.BoardCells
                                            .Where(x=> x.CellBox == cell.CellBox)
                                            .Where(x=> x.CellColumn != cell.CellColumn && x.CellRow != cell.CellRow)
                                            .Where(x=>x.PossibleValues.Intersect(cell.PossibleValues).Count() > 0)
                                            .Except(matchedBox).ToList();
                        if (nonPairs.Any())
                        {
                            foreach (var nonPair in nonPairs)
                            {
                                foreach (var value in cell.PossibleValues)
                                {
                                    solveMessage = $"{solveMessage}\nRemoved {value} from R{nonPair.CellRow} C{nonPair.CellColumn} due to conflict with R{cell.CellRow} C{cell.CellColumn} | R{matchedBox.First().CellRow} C{matchedBox.First().CellColumn}: Conjugate Pair";
                                    nonPair.PossibleValues.Remove(value);
                                    valueRemoved = true;
                                }
                            }
                        }
                    }
                    if (matchedRow.Any())
                    {
                        var nonPairs = board.BoardCells
                                            .Where(x=> x.CellRow == cell.CellRow)
                                            .Where(x=> x.CellColumn != cell.CellColumn && x.CellBox != cell.CellBox)
                                            .Where(x=>x.PossibleValues.Intersect(cell.PossibleValues).Count() > 0)
                                            .Except(matchedRow).ToList();
                        if (nonPairs.Any())
                        {
                            foreach (var nonPair in nonPairs)
                            {
                                foreach (var value in cell.PossibleValues)
                                {
                                    solveMessage = $"{solveMessage}\nRemoved {value} from R{nonPair.CellRow} C{nonPair.CellColumn} due to conflict with R{cell.CellRow} C{cell.CellColumn} | R{matchedRow.First().CellRow} C{matchedRow.First().CellColumn}: Conjugate Pair";
                                    nonPair.PossibleValues.Remove(value);
                                    valueRemoved = true;
                                }
                            }
                        }
                    }
                    if (matchedCol.Any())
                    {
                        var nonPairs = board.BoardCells
                                            .Where(x=> x.CellColumn == cell.CellColumn)
                                            .Where(x=> x.CellRow != cell.CellRow && x.CellBox != cell.CellBox)
                                            .Where(x=>x.PossibleValues.Intersect(cell.PossibleValues).Count() > 0)
                                            .Except(matchedCol).ToList();
                        if (nonPairs.Any())
                        {
                            foreach (var nonPair in nonPairs)
                            {
                                foreach (var value in cell.PossibleValues)
                                {
                                    solveMessage = $"{solveMessage}\nRemoved {value} from R{nonPair.CellRow} C{nonPair.CellColumn} due to conflict with R{cell.CellRow} C{cell.CellColumn} | R{matchedCol.First().CellRow} C{matchedCol.First().CellColumn}: Conjugate Pair";
                                    nonPair.PossibleValues.Remove(value);
                                    valueRemoved = true;
                                }
                            }
                        }

                    }
                    break;
                case 3:
                    break;
                case 4:
                    break;
                default:
                    return false;

            }

            return valueRemoved;
        }
    }
}
