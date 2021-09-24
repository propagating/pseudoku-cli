using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Pseudoku.Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            var quitResponse = "";
            do
            { Console.WriteLine("Welcome to PsuedoSolver");
                var sudokuGridString  = "005003060000580104300900050450039700100075000930000540500104093000358072003200600";
                var knightsGridString = "003608100040000070200000003600090008000102000700060004400000001060000020005409800";
                var kingsGridString   = "070003000009000507010070020800205000006000400000908005050030040201000800000500090";
                var puzzleConstraints = new List<PuzzleConstraint>();

                Helpers.WriteBreak();
                var numColumn = GetGridColumns();
                var numRow = GetGridRows();
                var gridString = GetGridString(numColumn, numRow, sudokuGridString, knightsGridString, kingsGridString, puzzleConstraints);
                if(!puzzleConstraints.Any()) GetConstraints(puzzleConstraints);
                Helpers.WriteBreak();

                Console.WriteLine("Please verify the dimensions, constraints, and puzzle string are correct.");
                Console.WriteLine($"Puzzle Dimension : {numRow} Rows by {numColumn} Columns");
                Helpers.WriteBreak();
                Console.WriteLine($"Constrains Chosen:");

                foreach (var c in puzzleConstraints)
                {
                    switch (c)
                    {
                        case PuzzleConstraint.RowUnique:
                            Console.WriteLine("Row Unique Sudoku Rules");
                            break;
                        case PuzzleConstraint.ColumnUnique:
                            Console.WriteLine("Column Unique Sudoku Rules");
                            break;
                        case PuzzleConstraint.BoxUnique:
                            Console.WriteLine("Box Unique Sudoku Rules");
                            break;
                        case PuzzleConstraint.KnightUnique:
                            Console.WriteLine("Knight's Move Constraint");
                            break;
                        case PuzzleConstraint.KingUnique:
                            Console.WriteLine("King's Move Constraint");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Helpers.WriteBreak();
                Console.WriteLine("Puzzle String");
                Console.WriteLine(gridString);
                Helpers.WriteBreak();
                Console.WriteLine("Enter any key to continue, except for (S).\nEnter (S) to start from the beginning.");
                if (Console.ReadLine().ToUpper() == "S")
                {
                    continue;
                }

                Helpers.WriteBreak();

                Console.WriteLine("Current Board State");
                var board = new PseudoBoard(numRow, numColumn, gridString);
                board.PrintBoard();
                Helpers.WriteBreak();


                Console.WriteLine("(S)tep Solve or (N)ormal Solve?");
                var solver = new PseudoSolver(puzzleConstraints, board);

                if (Console.ReadLine().ToUpperInvariant() != "S")
                {
                    Console.WriteLine("You selected Normal Solve, or an invalid method was chosen.\nNormal Solve will be used.\n \nPress any key to begin");
                    Console.ReadLine();
                    solver.SolveComplete();
                }

                while (true)
                {
                    solver.StepSolve();
                    if (solver.CurrentBoard.PuzzleSolved) break; //do this to capture the last solved step in the list of states
                }


                Console.WriteLine("Enter any key to select a different puzzle, except for (Q).\nEnter (Q) to quit");
                quitResponse = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(quitResponse) || quitResponse.ToUpper() != "Q") ;

        }

        private static int GetGridColumns()
        {
            int numColumn;
            Console.WriteLine("How many columns does your Grid have?");
            Console.WriteLine("Current Grid Size is limited to any combination of values between 1x1 and 9x9");

            while (!int.TryParse(Console.ReadLine(), out numColumn) || numColumn == 0 || numColumn > 9)
            {
                Console.WriteLine("Please enter a numerical value between 1 and 9 for number of columns.");
                Console.WriteLine("Current Grid Size is limited to any combination of values between 1x1 and 9x9");
                Console.WriteLine();
            }

            Helpers.WriteBreak();
            return numColumn;
        }

        private static int GetGridRows()
        {
            int numRow;
            Console.WriteLine("How many Rows does your Grid have?");
            while (!int.TryParse(Console.ReadLine(), out numRow) || numRow == 0 || numRow > 9)
            {
                Console.WriteLine("Please enter a numerical value between 1 and 9 for number of rows.");
                Console.WriteLine("Current Grid Size is limited to any combination of values between 1x1 and 9x9");
                Console.WriteLine();
            }

            Helpers.WriteBreak();
            return numRow;
        }

        private static string GetGridString(int numColumn, int numRow, string sudokuGridString, string knightsGridString,
                                            string kingsGridString, List<PuzzleConstraint> puzzleConstraints)
        {
            Console.WriteLine("Please enter the string of numbers to represent your grid.");

            Console.WriteLine("Your Grid will be inserted from left to right, then top to bottom.\n" +
                              "A value of 0 will be treated as an empty cell.");
            Console.WriteLine();

            Console.WriteLine("For example, a 2x2 grid with empty values in R1C1 and R2C2 are entered as:" +
                              "\n0220");

            Helpers.WriteBreak();
            Console.WriteLine(
                "Alternative, enter one of the following to use a pre-defined puzzle:" +
                "\n1. Standard Sudoku" +
                "\n2. Knights Move Sudoku" +
                "\n3. King's Move Sudoku");
            Helpers.WriteBreak();

            var gridString = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(gridString) || gridString.Length != (numColumn * numRow) || !gridString.All(char.IsDigit))
            {
                switch (gridString)
                {
                    case "1":
                        puzzleConstraints.Add(PuzzleConstraint.RowUnique);
                        puzzleConstraints.Add(PuzzleConstraint.ColumnUnique);
                        puzzleConstraints.Add(PuzzleConstraint.BoxUnique);
                        return sudokuGridString;
                    case "2":
                        puzzleConstraints.Add(PuzzleConstraint.RowUnique);
                        puzzleConstraints.Add(PuzzleConstraint.ColumnUnique);
                        puzzleConstraints.Add(PuzzleConstraint.BoxUnique);
                        puzzleConstraints.Add(PuzzleConstraint.KnightUnique);
                        return knightsGridString;
                    case "3":
                        puzzleConstraints.Add(PuzzleConstraint.RowUnique);
                        puzzleConstraints.Add(PuzzleConstraint.ColumnUnique);
                        puzzleConstraints.Add(PuzzleConstraint.BoxUnique);
                        puzzleConstraints.Add(PuzzleConstraint.KingUnique);
                        return kingsGridString;
                }

                Console.WriteLine("Please enter a value containing only numbers of equal length to your grid size (Columns * Rows).");
                Console.WriteLine();
                gridString = Console.ReadLine();
            }

            Helpers.WriteBreak();
            return gridString;
        }

        private static void GetConstraints(List<PuzzleConstraint> puzzleConstraints)
        {
            Console.WriteLine("Please select the types of constraints your puzzle has:");
            Console.WriteLine("Constraints can are entered as single letter keys in any order.");
            Helpers.WriteBreak();

            Console.WriteLine("For Example, a puzzle with both King and Knight's Move Constraints can be entered as KN, NK, kn, nK, etc.");
            Console.WriteLine("Solver requires that Standard Sudoku Rules are followed at a minimum.");
            Helpers.WriteBreak();

            Console.WriteLine("To use ONLY Standard Sudoku rules, leave blank.");
            Console.WriteLine("If you selected a pre-defined puzzle, you may not select additional constraints.");
            Helpers.WriteBreak();

            Console.WriteLine("Currently Supported Constraints :");
            Console.WriteLine("K: King's Rule - Numbers cannot be within a chess king's move of the same number.");
            Console.WriteLine("N: Knight's Move - Numbers cannot be within a chess knight's move of the same number.");
            Console.WriteLine("");
            var constraints = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(constraints))
            {
                while (!constraints.ToUpper().Contains("K") && !constraints.ToUpper().Contains("N"))
                {
                    if (string.IsNullOrWhiteSpace(constraints))
                    {
                        break;
                    }

                    Console.WriteLine();
                    Console.WriteLine("Invalid constraint selection. Leave empty for standard Sudoku only. enter K for King's Move, or N for Knight's Move");
                    Console.WriteLine("To use ONLY traditional Sudoku rules, enter a blank response.");

                    constraints = Console.ReadLine();
                }
            }

            Helpers.WriteBreak();
            puzzleConstraints.Add(PuzzleConstraint.RowUnique);
            puzzleConstraints.Add(PuzzleConstraint.ColumnUnique);
            puzzleConstraints.Add(PuzzleConstraint.BoxUnique);
            if (constraints != null && constraints.ToUpper().Contains("K")) puzzleConstraints.Add(PuzzleConstraint.KingUnique);
            if (constraints != null && constraints.ToUpper().Contains("N")) puzzleConstraints.Add(PuzzleConstraint.KnightUnique);
        }

    }

    public static class Helpers
    {
        public static void WriteBreak()
        {
            Console.WriteLine("------------------------------------------------------------------------");
        }
    }


    public class SolverStep
    {
        public uint SolverStepId { get; set; }
        public string StepComment { get; set; }
        public SolverState BoardState { get; set; }
    }

    public class SolverState
    {
        public List<PseudoCell> BoardCells { get; set; }
        public bool SolvedState { get; set; }
    }

    public enum PuzzleConstraint
    {
        RowUnique,
        ColumnUnique,
        BoxUnique,
        KnightUnique,
        KingUnique
    }

    enum ActionType
    {
        UpdatePossibleValue,
        TryPossibleValue,
        SetCurrentValue,
        SolveCell,
        SolveBoard
    }
}
