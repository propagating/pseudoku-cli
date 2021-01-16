namespace Pseudoku.Solver
{
    public interface IConstraintValidator
    {
        int ValidatorDifficulty { get; set; }
        bool ValidatePotentialCellValues(PseudoCell cell, PseudoBoard board, out string solveMessage);
    }
}
