using System.Xml.Linq;

namespace Sudoku.Models {
	public class Element {
		public readonly int row;

		public readonly int col;

		public int SubRow { get { return row / Grid.SUDOKU_SUBGRID; } }

        public int SubCol { get { return col / Grid.SUDOKU_SUBGRID; } }

        public List<int> Candidates { get; }

		public Element(int row, int col) {
			if (row < 0 || row > 8 || col < 0 || col > 8) {
				throw new ArgumentOutOfRangeException($"Illegal row or col value: ({row}, {col})");
			}
			this.row = row;
			this.col = col;
			Candidates = new List<int>();
        }

        public bool IsSafe(int[,] grid, int num) {
            if (num == 0) return false;
            if (num < 0 || num > 9) {
                throw new ArgumentOutOfRangeException($"Illegal Sudoku value: {num}");
            };

            for (int i = 0; i < Grid.SUDOKU_ROWS_COLS; i++) {
                if (i != col && grid[row, i] == num) return false;
                if (i != row && grid[i, col] == num) return false;
            }

            int startRow = row - row % Grid.SUDOKU_SUBGRID;
            int startCol = col - col % Grid.SUDOKU_SUBGRID;
            for (int i = startRow; i < startRow + Grid.SUDOKU_SUBGRID; i++) {
                for (int j = startCol; j < startCol + Grid.SUDOKU_SUBGRID; j++) {
                    if (grid[i, j] == num && !(i == row && j == col)) return false;
                }
            }

            return true;
        }

        /*
         * Add the candidates to an element given the InitialValues grid.
         */
        public void RefreshCandidates(int[,] initialValues) {
            Candidates.Clear();
            for (int num = 1; num <= Grid.SUDOKU_ROWS_COLS; num++) {
                if (IsSafe(initialValues, num)) {
                    Candidates.Add(num);
                }
            }
        }

        public bool AffectsCandidatesForOther(Element otherElement) {
			return SameRow(otherElement) || SameCol(otherElement) || SameSubGrid(otherElement);
        }

        private bool SameRow(Element otherElement) {
            return this.row == otherElement.row;
        }

        private bool SameCol(Element otherElement) {
            return this.col == otherElement.col;
        }

        private bool SameSubGrid(Element otherElement) {
            return this.SubRow == otherElement.SubRow && this.SubCol == otherElement.SubCol;
        }
    }
}

