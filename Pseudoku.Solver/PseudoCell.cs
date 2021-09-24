using System.Collections.Generic;

namespace Pseudoku.Solver
{
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
}