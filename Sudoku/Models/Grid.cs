using System.Diagnostics;
namespace Sudoku.Models {

	public class Grid {
        public static readonly int SUDOKU_ROWS_COLS = 9;

        public static readonly int SUDOKU_SUBGRID = 3;

        public static readonly int MAX_SOLUTIONS_LIMIT = 500;

        private static Random RANDOM_NUMBER_GENERATOR = new Random();

        public int[,] InitialValues { get; }

        public List<int[,]> Solutions { get; } = new List<int[,]>();

        private int[,] workingValues = new int[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

        private List<Element> graph = new();

        public List<Element> nonEmptyElements = new();

        public Grid(int[,] initialValues) {
            InitialValues = initialValues;
            workingValues = (int[,])initialValues.Clone();
            InitializeGraph();
        }

        /*
         * 
         */
        public static Grid GenerateRandomFilled() {

            int[,] values = new int[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

            // Create a random row and insert it randomly
            int[] randomRow = Enumerable.Range(1, 9).OrderBy(a => RANDOM_NUMBER_GENERATOR.Next()).ToArray();
            int randomRowIndex = RANDOM_NUMBER_GENERATOR.Next(0, 9);
            for (int i = 0; i < 9; i++) {
                values[randomRowIndex, i] = randomRow[i];
            }

            // Add a random column - must shuffle it until it fits with existing row
            int randomColIndex = RANDOM_NUMBER_GENERATOR.Next(0, 9);
            int[] randomCol;
            while (!IsColumnThirdSafe(randomColIndex, randomRowIndex / 3, values)) {
                randomCol = Enumerable.Range(1, 9).OrderBy(a => RANDOM_NUMBER_GENERATOR.Next()).ToArray();
                for (int j = 0; j < 9; j++) {
                    values[j, randomColIndex] = randomCol[j];
                }
            }

            Grid grid = new(values);
            grid.Solve(1);
            // Creating yet another grid is a hack to replace the initial values with the solution
            Grid finalGrid = new Grid((int[,])grid.workingValues.Clone());
            finalGrid.Solve(1);
            return finalGrid;
        }

        /*
         * 
         */
        public static Grid GenerateRandomUniqueSparse() {
            Grid grid = GenerateRandomFilled();
            int valueRemoved = 0;
            Element randomNonEmptyElement = new(0, 0);
            Grid lastWithSingleSolution = new((int[,])grid.InitialValues.Clone());
            DualWrite($"\n{grid.Solutions.Count.ToString()}\n");
            while (grid.Solutions.Count == 1) {
                lastWithSingleSolution = new((int[,])grid.InitialValues.Clone());
                // Remove random filled cells until the solution stops being unique
                randomNonEmptyElement = grid.nonEmptyElements.ElementAt(RANDOM_NUMBER_GENERATOR.Next(0, grid.nonEmptyElements.Count));
                valueRemoved = grid.RemoveInitialValueAndRefreshGraph(randomNonEmptyElement);
                //grid.InitializeGraph();
                grid.Solve();
                DualWrite(grid.Solutions.Count.ToString());
            }
            // As soon the solutions count exceeds 1, undo the last removal
            //grid.AddInitialValueAndRefreshGraph(randomNonEmptyElement, valueRemoved);
            //grid.InitializeGraph();
            //grid.Solve();
            //return grid;
            lastWithSingleSolution.Solve();
            return lastWithSingleSolution;
        }

        public bool IsSolved(int solutionNumber) { 
            for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                if (!IsColumnSafe(col, Solutions[solutionNumber])) return false;
            }
            return true;
        }

        private static bool IsColumnSafe(int col, int[,] grid) {
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                if (!IsSafe(grid, new Element(row, col), grid[row, col])) {
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
                if (!IsSafe(grid, new Element(row, col), grid[row, col])) {
                    return false;
                }
            }
            return true;
        }

        private static bool IsSafe(int[,] grid, Element element, int num) {
            if (num == 0) return false;
            if (num < 0 || num > 9) {
                throw new ArgumentOutOfRangeException($"Illegal Sudoku value: {num}");
            };
            int row = element.row, col = element.col;

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
                if (IsSafe(workingValues, el, num)) {
                    workingValues[el.row, el.col] = num;
                    graph.RemoveAt(graph.Count - 1);
                    Element newNonEmptyElement = new Element(el.row, el.col);
                    nonEmptyElements.Add(newNonEmptyElement);
                    if (Solve(maxSolutions)) return true;
                    nonEmptyElements.Remove(newNonEmptyElement);
                    graph.Add(el);
                    graph.Sort((Element a, Element b) => b.Candidates.Count - a.Candidates.Count);
                    workingValues[el.row, el.col] = 0;
                }
            }
            return false;
        }

        private void InitializeGraph() {
            Solutions.Clear();
            graph.Clear();
            nonEmptyElements.Clear();
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                    if (InitialValues[row, col] == 0) {
                        Element el = new Element(row, col);
                        AddCandidatesToElement(el);
                        graph.Add(el);
                    } else {
                        nonEmptyElements.Add(new Element(row, col));
                    }
                }
            }
        }

        //private void AddInitialValueAndRefreshGraph(Element elementToAdd, int valueToAdd) {
        //    Solutions.Clear();
        //    InitialValues[elementToAdd.row, elementToAdd.col] = valueToAdd;
        //    nonEmptyElements.Add(new Element(elementToAdd.row, elementToAdd.col));
        //    Element? graphElementToRemove = null;
           
        //    // Recalculate the candidates of elements in the same row, col and subgrid
        //    foreach (Element el in graph) {
        //        if (el.row == elementToAdd.row && el.col == elementToAdd.col) {
        //            graphElementToRemove = el;
        //        } else if (elementToAdd.AffectsCandidatesForOther(el)) {
        //            AddCandidatesToElement(el);
        //        }
        //    }

        //    // Remove this element from the graph
        //    if (graphElementToRemove == null) {
        //        throw new DataMisalignedException("Expected to find a graph element for " +
        //            $"({elementToAdd.row}, {elementToAdd.col})");
        //    }
        //    graph.Remove(graphElementToRemove);
        //}

        private int RemoveInitialValueAndRefreshGraph(Element elementToRemove) {
            Solutions.Clear();
            int valueToRemove = InitialValues[elementToRemove.row, elementToRemove.col];
            if (valueToRemove == 0) {
                throw new DataMisalignedException("No initial value to remove at " +
                    $"({elementToRemove.row}, {elementToRemove.col})");
            }

            // Remove the initial value
            InitialValues[elementToRemove.row, elementToRemove.col] = 0;
            nonEmptyElements.Remove(elementToRemove);

            // Recalculate the candidates of elements in the same row, col and subgrid
            foreach (Element el in graph) {
                if (elementToRemove.AffectsCandidatesForOther(el)) {
                    AddCandidatesToElement(el);
                }
            }

            // Add a new graph element for the newly blank cell
            AddCandidatesToElement(elementToRemove);
            graph.Add(elementToRemove);
            return valueToRemove;
        }

        private void AddCandidatesToElement(Element element) {
            element.Candidates.Clear();
            for (int num = 1; num <= SUDOKU_ROWS_COLS; num++) {
                if (IsSafe(InitialValues, element, num)) {
                    element.Candidates.Add(num);
                }
            }
        }

        /*
         * Writes to both Console and Debug
         */
        private static void DualWrite(String str) {
            Console.Write(str);
            Debug.Write(str);
        }
        
        public static void PrintValues(int[,] grid) {
            for (int i = 0; i < grid.GetLength(0); i++) {
                for (int j = 0; j < grid.GetLength(1); j++) {
                    DualWrite(grid[i, j] + " ");
                }
                DualWrite("\n");
            }
            DualWrite("\n");
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

