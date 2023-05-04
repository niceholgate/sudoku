using System;
using System.Diagnostics;
using System.Linq;
namespace Sudoku.Models {

	public class Grid {
        public static readonly int SUDOKU_ROWS_COLS = 9;

        public static readonly int SUDOKU_SUBGRID = 3;

        public static readonly int MAX_SOLUTIONS_LIMIT = 500;

        private static Random RANDOM_NUMBER_GENERATOR = new Random();

        public int[,] InitialValues { get; }

        public List<int[,]> Solutions { get; } = new List<int[,]>();

        private int[,] workingValues = new int[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

        private List<Element> graph = new List<Element>();

        public Grid(int[,] initialValues) {
            InitialValues = initialValues;
            workingValues = (int[,])initialValues.Clone();
            InitializeGraph();
        }

        public bool IsSolved(int solutionNumber) { 
            for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                if (!IsColumnSafe(col, Solutions[solutionNumber])) return false;
            }
            return true;
        }

        private static bool IsColumnSafe(int col, int[,] grid) {
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                if (!IsSafe(grid, row, col, grid[row, col])) {
                    return false;
                }
            }
            return true;
        }

        private static bool IsColumnThirdSafe(int col, int colThird, int[,] grid) {
            int[] validThirds = { 0, 1, 2 };
            if (!validThirds.Contains(colThird)) {
                throw new ArgumentException($"Must specify a column third from within {validThirds}");
            }
            for (int row = colThird*3; row <= colThird*3 + 2; row++) {
                if (!IsSafe(grid, row, col, grid[row, col])) {
                    return false;
                }
            }
            return true;
        }

        public static bool IsSafe(int[,] grid, int row, int col, int num) {
            if (num < 1 || num > 9) return false;


            for (int i = 0; i < SUDOKU_ROWS_COLS; i++) {
                if (i != col && grid[row, i] == num) return false;
                if (i != row && grid[i, col] == num) return false;
            }

            int startRow = row - row % SUDOKU_SUBGRID;
            int startCol = col - col % SUDOKU_SUBGRID;
            for (int i = startRow; i < startRow + SUDOKU_SUBGRID; i++) {
                for (int j = startCol; j < startCol + SUDOKU_SUBGRID; j++) {
                    if (grid[i, j] == num && !(i == row && j == col)) return false;
                }
            }

            return true;
        }

        public static Grid GenerateRandomFilled() {

            int[,] values = new int[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

            // Create a random row and insert it randomly
            int[] randomRow = Enumerable.Range(1, 9).OrderBy(a => RANDOM_NUMBER_GENERATOR.Next()).ToArray();
            int randomRowIndex = RANDOM_NUMBER_GENERATOR.Next(0, 9);
            for (int i = 0; i < 9; i++) {
                values[randomRowIndex, i] = randomRow[i];
            }

            // Add a random column - must shuffle it until it fits with existing row
            // TODO: make it faster by only checking 3 critical cells
            int randomColIndex = RANDOM_NUMBER_GENERATOR.Next(0, 9);
            int[] randomCol;
            while (!IsColumnThirdSafe(randomColIndex, randomRowIndex / 3, values)) {
                randomCol = Enumerable.Range(1, 9).OrderBy(a => RANDOM_NUMBER_GENERATOR.Next()).ToArray();
                for (int j = 0; j < 9; j++) {
                    values[j, randomColIndex] = randomCol[j];
                }
                PrintValues(values);
            }

            Grid grid = new Grid(values);
            grid.Solve(1);
            return grid;
        }

        public bool Solve() {
            return Solve(MAX_SOLUTIONS_LIMIT);
        }

        // TODO: flag if all solutions were found and added to Solutions
        public bool Solve(int maxSolutions) {
            if (maxSolutions > MAX_SOLUTIONS_LIMIT) {
                throw new ArgumentException($"Must specify a maximum number of solutions <= {MAX_SOLUTIONS_LIMIT}");
            } 

            if (graph.Count == 0) {
                Solutions.Add((int[,])workingValues.Clone());
                // Return false unless decide to finish because have enough solutions (or all?)
                if (Solutions.Count >= maxSolutions) return true;
                return false;
            }

            graph.Sort((Element a, Element b) => b.Candidates.Count - a.Candidates.Count);
            Element el = graph.ElementAt(graph.Count - 1);

            for (int i = 0; i < el.Candidates.Count; ++i) { // i++ ?
                int num = el.Candidates.ElementAt(i);
                if (IsSafe(workingValues, el.row, el.col, num)) {
                    workingValues[el.row, el.col] = num;
                    graph.RemoveAt(graph.Count - 1);
                    if (Solve(maxSolutions)) return true;
                    graph.Add(el);
                    graph.Sort((Element a, Element b) => b.Candidates.Count - a.Candidates.Count);
                    workingValues[el.row, el.col] = 0;
                }
            }
            return false;
        }

        private void InitializeGraph() {
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                    if (InitialValues[row, col] == 0) {
                        Element e = new Element(row, col);
                        for (int num = 1; num <= SUDOKU_ROWS_COLS; num++) {
                            if (IsSafe(workingValues,row, col, num)) {
                                e.Candidates.Add(num);
                            }
                        }
                        graph.Add(e);
                    }
                }
            }
        }

        public static void PrintValues(int[,] grid) {
            for (int i = 0; i < grid.GetLength(0); i++) {
                for (int j = 0; j < grid.GetLength(1); j++) {
                    Debug.Write(grid[i, j] + " ");
                }
                Debug.WriteLine("");
            }
            Debug.WriteLine("");
        }

        public bool CheckValuesEqual(int[,] comparisonValues, int solutionNumber) {
            if (Solutions[solutionNumber].Rank != comparisonValues.Rank ||
                Solutions[solutionNumber].GetLength(0) != comparisonValues.GetLength(0) ||
                Solutions[solutionNumber].GetLength(1) != comparisonValues.GetLength(1)) {
                return false;
            }
            for (int i = 0; i < Solutions[solutionNumber].GetLength(0); i++) {
                for (int j = 0; j < Solutions[solutionNumber].GetLength(1); j++) {
                    if (Solutions[solutionNumber][i, j] != comparisonValues[i, j]) return false;
                }
            }
            return true;
        }
	}
}

