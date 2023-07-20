namespace Sudoku.Models {
	public class Element : IEquatable<Element> {
		public readonly int row;

		public readonly int col;

		public int SubRow { get { return row / Grid.SUDOKU_SUBGRID; } }

        public int SubCol { get { return col / Grid.SUDOKU_SUBGRID; } }

        public List<int> Candidates { get; set; }

        public int FinalValue { get {
                return Candidates.Count == 1 ? Candidates.ElementAt(0) : 0; } }

        // finalValue = 0 if unknown
        public Element(int row, int col, int finalValue) {
            if (row < 0 || row > 8 || col < 0 || col > 8) {
                throw new ArgumentOutOfRangeException($"Illegal row or col value: ({row}, {col})");
            }
            this.row = row;
            this.col = col;
            Candidates = finalValue == 0 ? new List<int>() : new List<int>() { finalValue };
        }

        /*
         * Check if this Element's current value is safe on a given grid Elements
         */
        public bool IsSafe(Element[,] gridElements) {
            return IsSafe(gridElements, FinalValue);
        }

        /*
         * Add the candidates to an element given the InitialValues grid.
         */
        public void RefreshCandidates(Element[,] initialValues) {
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

        private bool IsSafe(Element[,] gridElements, int candidate) {
            if (candidate == 0) return false;
            if (candidate < 0 || candidate > 9) {
                throw new ArgumentOutOfRangeException($"Illegal Sudoku value: {candidate}");
            };

            // Check the row and column for this candidate
            for (int i = 0; i < Grid.SUDOKU_ROWS_COLS; i++) {
                if ((i != col && gridElements[row, i].FinalValue == candidate) ||
                    (i != row && gridElements[i, col].FinalValue == candidate)) return false;
            }

            // Check the subgrid for this candidate
            int startRow = row - row % Grid.SUDOKU_SUBGRID;
            int startCol = col - col % Grid.SUDOKU_SUBGRID;
            for (int i = startRow; i < startRow + Grid.SUDOKU_SUBGRID; i++) {
                for (int j = startCol; j < startCol + Grid.SUDOKU_SUBGRID; j++) {
                    if (gridElements[i, j].FinalValue == candidate && !(i == row && j == col)) return false;
                }
            }

            return true;
        }

        public bool Equals(Element? other) {
            return other != null &&
                other.row.Equals(row) &&
                other.col.Equals(col) && 
                other.Candidates.SequenceEqual(Candidates);
        }

        public override bool Equals(object? obj) {
            return Equals(obj as Element);
        }

        public override int GetHashCode() {
            return HashCode.Combine(row, col, Candidates);
        }
    }
}

