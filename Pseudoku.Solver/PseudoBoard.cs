using System;
using System.Collections.Generic;
using System.Linq;

namespace Pseudoku.Solver
{
    public class PseudoBoard
    {
        public List<PseudoCell> BoardCells { get; set; } = new List<PseudoCell>();
        public int MaxRows { get; set; }
        public int MaxColumns { get; set; }
        public List<int> AllowedValues { get; set; }
        public bool ValidState { get; set; } = true;
        public bool PuzzleSolved { get; set; }
        public string PuzzleString { get; set; }
        public int RowBoxes { get; set; }
        public int ColumnBoxes { get; set; }
        public int TotalBoxes { get; set; }
        public string SolutionString { get; set; }

        public PseudoBoard(int rows, int cols, string inputValues)
        {
            MaxRows           = rows;
            MaxColumns        = cols;
            AllowedValues     = Enumerable.Range(1, Math.Max(MaxRows, MaxColumns)).Select(x => x).ToList();
            PuzzleString      = inputValues;

            var currentRow    = 1;
            var currentColumn = 1;
            var currentIndex  = 0;

            var values = inputValues.ToList();

            while (currentRow <= MaxRows)
            {
                while (currentColumn <= MaxColumns)
                {
                    var cellValue = Convert.ToInt32(values[currentIndex] - 48); //char of 0 enters as 48
                    var pseudoCell = new PseudoCell
                    {
                        CellRow        = currentRow,
                        CellColumn     = currentColumn,
                        CurrentValue   = cellValue,
                        PossibleValues = cellValue == 0 ? Enumerable.Range(1, Math.Max(MaxRows, MaxColumns)).Select(x=> x).ToList() : new List<int>(),
                        SolvedCell     = cellValue == 0 ? false : true
                    };

                    pseudoCell.FindBox();
                    BoardCells.Add(pseudoCell);
                    currentIndex++;
                    currentColumn++;
                }

                currentColumn = 1;
                currentRow++;
            }
        }

        public PseudoBoard(List<PseudoCell> boardCells)
        {
            BoardCells    = boardCells;
            MaxRows       = boardCells.OrderByDescending(x => x.CellRow).Select(x => x.CellRow).FirstOrDefault();
            MaxColumns    = boardCells.OrderByDescending(x => x.CellColumn).Select(x => x.CellColumn).FirstOrDefault();
            AllowedValues = Enumerable.Range(1, Math.Max(MaxRows, MaxColumns)).Select(x => x).ToList();
            ValidState    = true;
            PuzzleSolved  = !boardCells.Any(x=> !x.SolvedCell);
            PuzzleString  = this.SerialiseBoardToPuzzleString();
        }

        public string SerialiseBoardToPuzzleString()
        {
            var rowPosition = 1;

            var puzzleString = "";
            while (rowPosition <= this.MaxRows)
            {
                var colPosition = 1;
                while (colPosition <= this.MaxColumns)
                {
                    var cell = BoardCells.Where(x => x.CellRow == rowPosition && x.CellColumn == colPosition).FirstOrDefault();
                    if (cell == null)
                    {
                        puzzleString = $"{puzzleString}0";
                    }
                    else
                    {
                        puzzleString = $"{puzzleString}{cell.CurrentValue}";
                    }
                    colPosition++;
                }

                rowPosition++;
            }

            return puzzleString;
        }
        public void PrintBoard()
        {
            var currentRow = 1;
            Console.WriteLine("___________________________________________________________________________________________________________");
            while (currentRow <= MaxRows)
            {
                foreach (var box in BoardCells.Where(x=> x.CellRow == currentRow).GroupBy(x=> x.CellBox))
                {
                    foreach (var cell in box)
                    {
                        if(cell.CellColumn == 1){Console.Write("||");}

                        if (cell.CurrentValue == 0)
                        {
                            Console.Write($" ");
                            var values = "";
                            foreach (var value in cell.PossibleValues)
                            {
                                values = $"{values}{value}";
                            }

                            var valueToWrite = PadBoth(values, 9);

                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write(valueToWrite);
                            Console.Write($" ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                        else
                        {
                            Console.Write($"     {cell.CurrentValue}     ");

                        }
                    }
                    Console.Write("||");
                }

                Console.WriteLine();
                if (currentRow == 9)
                {
                    Console.Write("-----------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                }
                else if (currentRow % 3 == 0)
                {
                    Console.Write("||_________________________________||_________________________________||_________________________________||");
                    Console.WriteLine();
                }
                currentRow++;
            }
        }

        public string PadBoth(string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = spaces/2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);

        }
    }
}