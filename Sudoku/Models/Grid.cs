using System;
namespace Sudoku.Models {

	public class Grid {
        public static readonly int N = 9;

        public static readonly int n = 3;

        private int[,] Values { get; } = {
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

        public Grid() {
            InititalizeGraph();
        }

        public bool IsSafe(int row, int col, int num) {
            for (int i = 0; i < N; i++) {
                if (Values[row, i] == num ||
                    Values[i, col] == num) return false;
            }

            int startRow = row - row % n;
            int startCol = col - col % n;
            for (int i = startRow; i < startRow + n; i++) {
                for (int j = startCol; j < startCol + n; j++) {
                    if (Values[i, j] == num) return false;
                }
            }

            return true;
        }

        public bool Solve() {
            
            if (Graph.Count == 0) return true;

            Graph.Sort((Element a, Element b) => a.Candidates.Count - b.Candidates.Count);
            Element el = Graph.ElementAt(Graph.Count - 1);
            Graph.RemoveAt(Graph.Count - 1);

            for (int i = 0; i < el.Candidates.Count; ++i) {
                int num = el.Candidates.ElementAt(i);
                if (IsSafe(el.row, el.col, num)) {
                    Values[el.row, el.col] = num;
                    Graph.Remove(el);
                    if (Solve()) return true;
                    Graph.Add(el);
                    Graph.Sort((Element a, Element b) => a.Candidates.Count - b.Candidates.Count);
                    Values[el.row, el.col] = 0;
                }
            }
            return false;
        }

        private void InititalizeGraph() {
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    if (Values[i, j] == 0) {
                        Element e = new Element(i, j);
                        for (int num = 1; num <= N; num++) {
                            if (IsSafe(i, j, num)) {
                                e.Candidates.Add(num);
                            }
                        }
                        Graph.Add(e);
                    }
                }
            }
        }
	}
}

