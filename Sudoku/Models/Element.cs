using System;
namespace Sudoku.Models {
	public class Element {
		public readonly int row;

		public readonly int col;

		public List<int> Candidates { get; }

		public Element(int row, int col) {
			this.row = row;
			this.col = col;
			this.Candidates = new List<int>();
        }
	}
}

