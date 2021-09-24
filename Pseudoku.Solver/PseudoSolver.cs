using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Pseudoku.Solver.Methods;
using Pseudoku.Solver.Validators;

namespace Pseudoku.Solver
{
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
                var solvableCells = CurrentBoard.BoardCells.Where(x => !x.SolvedCell).ToList();
                foreach (var cell in solvableCells)
                {

                    var validatorSuccess = false;
                    var methodSuccess    = false;
                    foreach (var validator in BoardValidators.OrderBy(x=> x.ValidatorDifficulty))
                    {
                        validatorSuccess = validator.ValidatePotentialCellValues(cell, CurrentBoard, out _);
                        if (!validatorSuccess)
                        {
                            totalFailures++;
                        }

                        validatorSteps++;
                        totalSteps++;
                    }

                    if (!validatorSuccess)
                    {
                        foreach (var method in SolverMethods.OrderBy(x=> x.MethodDifficulty))
                        {
                            methodSuccess = method.ApplyMethod(cell, CurrentBoard, out _);
                            if (!methodSuccess)
                            {
                                totalFailures++;
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
}
