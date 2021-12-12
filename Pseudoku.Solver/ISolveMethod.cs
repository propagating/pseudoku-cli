namespace Pseudoku.Solver
{
    public interface ISolveMethod
    {
        public int MethodDifficulty { get; set; }
        public bool ApplyMethod(PseudoCell cell, PseudoBoard board, out string solveMessage );
    }
}
