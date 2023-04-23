using System;
using System.Linq;
namespace Sudoku.Models {

	public class Grid {
        public static readonly int N = 9;

        public static readonly int n = 3;

        public int[,] Values { get; } = {
                         { 3, 0, 6, 5, 0, 8, 4, 0, 0 },
                         { 5, 2, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 8, 7, 0, 0, 0, 0, 3, 1 },
                         { 0, 0, 3, 0, 1, 0, 0, 8, 0 },
                         { 9, 0, 0, 8, 6, 3, 0, 0, 5 },
                         { 0, 5, 0, 0, 9, 0, 6, 0, 0 },
                         { 1, 3, 0, 0, 0, 0, 2, 5, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 7, 4 },
                         { 0, 0, 5, 2, 0, 6, 3, 0, 0 } };

        private List<Element> Graph { get; } = new List<Element>();

        public Grid(int[,] initialValues) {
            Values = initialValues;
            InitializeGraph();
        }

        public bool IsUnique() {
            return IsSolved() && Graph.Count == 0;
        }

        public bool IsSolved() {
            for (int row = 0; row < N; row++) {
                for (int col = 0; col < N; col++) {
                    if (!IsSafe(row, col, Values[row, col])) {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsSafe(int row, int col, int num) {
            if (num == 0) return false;


            for (int i = 0; i < N; i++) {
                if (i != col && Values[row, i] == num) return false;
                if (i != row && Values[i, col] == num) return false;
            }

            int startRow = row - row % n;
            int startCol = col - col % n;
            for (int i = startRow; i < startRow + n; i++) {
                for (int j = startCol; j < startCol + n; j++) {
                    if (Values[i, j] == num && !(i == row && j == col)) return false;
                }
            }

            return true;
        }

        public bool Solve() {
            
            if (Graph.Count == 0) return true;

            Graph.Sort((Element a, Element b) => b.Candidates.Count - a.Candidates.Count);
            Element el = Graph.ElementAt(Graph.Count - 1);

            for (int i = 0; i < el.Candidates.Count; ++i) {
                int num = el.Candidates.ElementAt(i);
                if (IsSafe(el.row, el.col, num)) {
                    Values[el.row, el.col] = num;
                    Graph.RemoveAt(Graph.Count - 1);
                    if (Solve()) {
                        return true;
                    }// else {
                    //    Console.WriteLine("Nested Solve call was false");
                    //}
                    Graph.Add(el);
                    Graph.Sort((Element a, Element b) => b.Candidates.Count - a.Candidates.Count);
                    Values[el.row, el.col] = 0;
                }
            }
            return false;
        }

        private void InitializeGraph() {
            for (int row = 0; row < N; row++) {
                for (int col = 0; col < N; col++) {
                    if (Values[row, col] == 0) {
                        Element e = new Element(row, col);
                        for (int num = 1; num <= N; num++) {
                            if (IsSafe(row, col, num)) {
                                e.Candidates.Add(num);
                            }
                        }
                        Graph.Add(e);
                    }
                }
            }
        }

        public void PrintValues() {
            for (int i = 0; i < Values.GetLength(0); i++) {
                for (int j = 0; j < Values.GetLength(1); j++) {
                    Console.Write(Values[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool CheckValuesEqual(int[,] comparisonValues) {
            if (Values.Rank != comparisonValues.Rank ||
                Values.GetLength(0) != comparisonValues.GetLength(0) ||
                Values.GetLength(1) != comparisonValues.GetLength(1)) {
                return false;
            }
            for (int i = 0; i < Values.GetLength(0); i++) {
                for (int j = 0; j < Values.GetLength(1); j++) {
                    if (Values[i, j] != comparisonValues[i, j]) return false;
                }
            }
            return true;
        }
	}
}

