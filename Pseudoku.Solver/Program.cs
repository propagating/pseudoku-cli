using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Pseudoku.Solver.Methods;
using Pseudoku.Solver.Validators;

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
                            Console.WriteLine("Knight's move Constraint");
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

    public class PseudoSolver
    {
        public List<IConstraintValidator> BoardValidators { get; set; } = new List<IConstraintValidator>();
        public List<ISolveMethod> SolverMethods { get; set; } = new List<ISolveMethod>();
        public List<SolverStep> SolverSteps { get; set; } = new List<SolverStep>();
        public SolverStep CurrentStep { get; set; }
        public PseudoBoard CurrentBoard { get; set; }

        public PseudoSolver(List<PuzzleConstraint> constraints, PseudoBoard board)
        {

            CurrentStep = new SolverStep
            {
                SolverStepId = 1,
                StepComment = "Starting Board State",
                BoardState = new SolverState
                {
                    BoardCells = board.BoardCells,
                    SolvedState = false
                }
            };
            CurrentBoard = board;

            SolverMethods.Add(new HiddenSingle());
            SolverMethods.Add(new IntersectionRemoval());


            foreach (var constraint in constraints)
            {
                switch (constraint)
                {
                    case PuzzleConstraint.BoxUnique:
                        BoardValidators.Add(new BoxUnique());
                        break;
                    case PuzzleConstraint.ColumnUnique:
                        BoardValidators.Add( new ColumnUnique());
                        break;
                    case PuzzleConstraint.RowUnique:
                        BoardValidators.Add(new RowUnique());
                        break;
                    case PuzzleConstraint.KnightUnique:
                        BoardValidators.Add(new KnightUnique());
                        break;
                    case PuzzleConstraint.KingUnique:
                        BoardValidators.Add(new KingUnique());
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void StepSolve()
        {
            var boardStateChanged = false;
            var solvableCells = CurrentBoard.BoardCells.Where(x => !x.SolvedCell).ToList();
            var noChangeMade = 0;
            var solveMessage = "";
            while (!boardStateChanged && noChangeMade <= 100)
            {
                var baseDifficulty = 1;
                var validatorSuccess = false;

                foreach (var validator in BoardValidators.Where(x => x.ValidatorDifficulty <= baseDifficulty).ToList())
                {
                    if (validatorSuccess) break;
                    foreach (var cell in solvableCells)
                    {
                        if (validator.ValidatePotentialCellValues(cell, CurrentBoard, out solveMessage))
                        {
                            validatorSuccess = true;
                            break;
                        }
                    }
                }

                if (validatorSuccess)
                {
                    boardStateChanged = true;
                    noChangeMade = 0;
                    continue;
                }

                var methodSuccess = false;
                foreach (var method in SolverMethods.Where(x => x.MethodDifficulty <= baseDifficulty).ToList())
                {
                    if (methodSuccess) break;
                    foreach (var cell in solvableCells)
                    {
                        if (method.ApplyMethod(cell, CurrentBoard, out solveMessage))
                        {
                            methodSuccess = true;
                            break;
                        }
                    }
                }

                if (methodSuccess)
                {
                    boardStateChanged = true;
                    noChangeMade = 0;
                    continue;
                }

                baseDifficulty++;
                noChangeMade++;
            }

            if (boardStateChanged)
            {
                SolverSteps.Add(CurrentStep);
                var nextStep = new SolverStep
                {
                    SolverStepId = CurrentStep.SolverStepId+1,
                    StepComment  = solveMessage,
                    BoardState   = new SolverState
                    {
                        BoardCells  = CurrentBoard.BoardCells.ToList(),
                        SolvedState = false
                    }
                };
                CurrentStep  = nextStep;
                CurrentBoard = new PseudoBoard(CurrentBoard.BoardCells.ToList());
                CurrentBoard.PrintBoard();
            }
            if(!string.IsNullOrEmpty(solveMessage)) Console.WriteLine(solveMessage);
            Console.WriteLine($"Total Steps = {CurrentStep.SolverStepId}");
        }
        public void SolveComplete()
        {
            var totalFailures   = 0;
            var totalSteps      = 0;
            var methodSteps     = 0;
            var validatorSteps  = 0;
            var timer           = new Stopwatch();

            timer.Start();
            Console.WriteLine(CurrentBoard.SerialiseBoardToPuzzleString());
            Helpers.WriteBreak();
            while (!CurrentBoard.PuzzleSolved)
            {
                var solveMethod = "";
                var solvableCells = CurrentBoard.BoardCells.Where(x => !x.SolvedCell).ToList();
                foreach (var cell in solvableCells)
                {

                    var validatorSuccess = false;
                    var methodSuccess    = false;
                    foreach (var validator in BoardValidators.OrderBy(x=> x.ValidatorDifficulty))
                    {
                        validatorSuccess = validator.ValidatePotentialCellValues(cell, CurrentBoard, out solveMethod);
                        if (!validatorSuccess)
                        {
                            totalFailures++;
                        }
                        else
                        {
                        }

                        validatorSteps++;
                        totalSteps++;
                    }

                    if (!validatorSuccess)
                    {
                        foreach (var method in SolverMethods.OrderBy(x=> x.MethodDifficulty))
                        {
                            methodSuccess = method.ApplyMethod(cell, CurrentBoard, out solveMethod);
                            if (!methodSuccess)
                            {
                                totalFailures++;
                            }
                            else
                            {
                            }
                            methodSteps++;
                            totalSteps++;
                        }
                    }
                }

                CurrentBoard.PuzzleSolved = !solvableCells.Any(x => !x.SolvedCell);
            }

            timer.Stop();
            CurrentBoard.PrintBoard();
            Helpers.WriteBreak();
            Console.WriteLine($"Completed Puzzle String");
            Console.WriteLine(CurrentBoard.SerialiseBoardToPuzzleString());
            Helpers.WriteBreak();
            Console.WriteLine($"Puzzle Solved in {timer.Elapsed}\nTotal Steps Taken (Validators & Solve Methods) {totalSteps}" +
                              $"\nTotal Validator Steps Taken {validatorSteps}"+
                              $"\nTotal Method Steps Taken {methodSteps}"+
                              $"\nTotal Actions Taken (Validators & Solve Methods w/ Legal Move Available) {totalSteps-totalFailures}");
        }
    }

    public static class Helpers
    {
        public static void WriteBreak()
        {
            Console.WriteLine("------------------------------------------------------------------------");
        }
    }


    public class PseudoCell
    {

        public int CellRow { get; set; }
        public int CellColumn { get; set; }
        public int CellBox { get; set; }
        public List<int> PossibleValues { get; set; }
        public int CurrentValue { get; set; }
        public bool SolvedCell { get; set; }

        public void FindBox()
        {
            //assume 9x9 grid for now until custom boxes are allowed
            if(CellColumn <= 3 && CellRow <=3) CellBox = 1;
            else if(CellColumn <= 6 && CellRow <=3) CellBox = 2;
            else if(CellColumn <= 9 && CellRow <=3) CellBox = 3;
            else if(CellColumn <= 3 && CellRow <=6) CellBox = 4;
            else if(CellColumn <= 6 && CellRow <=6) CellBox = 5;
            else if(CellColumn <= 9 && CellRow <=6) CellBox = 6;
            else if(CellColumn <= 3 && CellRow <=9) CellBox = 7;
            else if(CellColumn <= 6 && CellRow <=9) CellBox = 8;
            else if(CellColumn <= 9 && CellRow <=9) CellBox = 9;

        }
    }

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
