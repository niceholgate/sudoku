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

		public bool SameRow(Element otherElement) {
			return this.row == otherElement.row;
		}

        public bool SameCol(Element otherElement) {
            return this.col == otherElement.col;
        }

        public bool SameSubGrid(Element otherElement) {
            return this.SubRow == otherElement.SubRow && this.SubCol == otherElement.SubCol;
        }

		public bool AffectsCandidatesForOther(Element otherElement) {
			return this.SameRow(otherElement) || this.SameCol(otherElement) || this.SameSubGrid(otherElement);
        }
    }
}

