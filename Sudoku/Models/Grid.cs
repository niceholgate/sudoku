namespace Sudoku.Models {

    // TODO: implements IClonable
	public class Grid {
        public static readonly int SUDOKU_ROWS_COLS = 9;

        public static readonly int SUDOKU_SUBGRID = 3;

        public static readonly int MAX_SOLUTIONS_LIMIT = 500;

        public int[,] InitialValues { get; }

        public List<int[,]> Solutions { get; } = new List<int[,]>();

        private Element[,] workingValues = new Element[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

        private List<Element> graph = new();

        public List<Element> initiallyNonEmptyElements = new();

        public Grid(int[,] initialValues) {
            InitialValues = initialValues;
            Initialize();
        }

        public Grid(int[,] initialValues, List<int[,]> solutions, Element[,] workingValues, List<Element> graph, List<Element> nonEmptyElements) {
            this.InitialValues = initialValues;
            this.Solutions = solutions;
            this.workingValues = workingValues;
            this.graph = graph;
            this.initiallyNonEmptyElements = nonEmptyElements;
        }

        public Grid Clone() {
            return new Grid(
                (int[,])this.InitialValues.Clone(),
                new List<int[,]>(this.Solutions),
                (Element[,])this.workingValues.Clone(),
                new List<Element>(this.graph),
                new List<Element>(this.initiallyNonEmptyElements));
        }

        /*
         * Generate a random completely filled grid based on a random initial row + column solved using 
         * the backtracking solver (deterministic based on the initial values).
         */
        public static Grid GenerateRandomFilled() {

            int[,] values = new int[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

            // Create a random row and insert it randomly
            int[] randomRow = Utils<int>.ShuffleToArray(Enumerable.Range(1, 9));
            int randomRowIndex = Utils<int>.RANDOM_NUMBER_GENERATOR.Next(0, 9);
            for (int i = 0; i < 9; i++) {
                values[randomRowIndex, i] = randomRow[i];
            }

            Element[,] elementGrid = new Element[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                    elementGrid[row, col] = new Element(row, col, values[row, col]);
                }
            }

            // Add a random column - must shuffle it until it fits with existing row
            int randomColIndex = Utils<int>.RANDOM_NUMBER_GENERATOR.Next(0, 9);
            int[] randomCol;
            while (!IsColumnThirdSafe(randomColIndex, randomRowIndex / 3, elementGrid)) {
                randomCol = Utils<int>.ShuffleToArray(Enumerable.Range(1, 9));
                for (int j = 0; j < 9; j++) {
                    elementGrid[j, randomColIndex].finalValue = randomCol[j];
                }
            }

            Grid grid = new(elementGrid);
            grid.Solve(1);
            // Creating yet another grid is a hack to replace the initial values with the solution
            Grid finalGrid = new((int[,])grid.workingValues.Clone());
            finalGrid.Solve(1);
            return finalGrid;
        }

        /*
         * Generate a random new puzzle with a unique solution. Typically gives a puzzle with between ~20 and ~50 empty cells;
         * indicative of the difficulty of the problem.
         * Plotted a histogram of 400 initially empty cell counts(81 - initiallyNonEmptyElements)
         * and propose the following difficulty cutoffs:
         * Very easy : <= 25 empty
         * Easy      : 25 < empty <= 30
         * Medium    : 30 < empty <= 40
         * Hard      : 40 < empty <= 45
         * Very Hard : > 45 empty
         */
        public static Grid GenerateRandomUniqueSparse() {
            Grid grid = GenerateRandomFilled();

            // Remove random filled cells until the solution stops being unique
            Grid previousGrid = grid.Clone();
            while (grid.Solutions.Count == 1) {
                previousGrid = grid.Clone();
                Element randomNonEmptyElement = Utils<Element>.SelectRandomElement(grid.initiallyNonEmptyElements);
                grid.RemoveInitialValueAndRefresh(randomNonEmptyElement);
            }
            return previousGrid;
        }

        public static bool IsSolved(int[,] grid) {
            Element[,] elementGrid = new Element[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                    elementGrid[row, col] = new Element(row, col, grid[row, col]);
                }
            }
            for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                if (!IsColumnSafe(col, elementGrid)) return false;
            }
            return true;
        }

        public static bool IsSolved(Element[,] grid) { 
            for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                if (!IsColumnSafe(col, grid)) return false;
            }
            return true;
        }

        public bool Solve() {
            return Solve(MAX_SOLUTIONS_LIMIT);
        }

        // TODO: flag if all solutions were found and added to Solutions
        private bool Solve(int maxSolutions) {
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
                if (el.IsSafe(workingValues, num)) {
                    el.finalValue = num;
                    graph.RemoveAt(graph.Count - 1);
                    if (Solve(maxSolutions)) return true;
                    graph.Add(el);
                    el.finalValue = 0;
                    graph.Sort((Element a, Element b) => b.Candidates.Count - a.Candidates.Count);
                }
            }
            return false;
        }

        private void Initialize() {
            Solutions.Clear();
            graph.Clear();
            initiallyNonEmptyElements.Clear();
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                    Element el = new(row, col);
                    workingValues[row, col] = el;
                    if (InitialValues[row, col] == 0) {
                        el.RefreshCandidates(InitialValues);
                        graph.Add(el);
                    } else {
                        initiallyNonEmptyElements.Add(el);
                    }
                }
            }
        }

        private void RemoveInitialValueAndRefresh(Element elementToRemove) {
            Solutions.Clear();

            if (InitialValues[elementToRemove.row, elementToRemove.col] == 0) {
                throw new DataMisalignedException("No initial value to remove at " +
                    $"({elementToRemove.row}, {elementToRemove.col})");
            }

            // Remove the initial value
            InitialValues[elementToRemove.row, elementToRemove.col] = 0;
            initiallyNonEmptyElements.Remove(elementToRemove);

            // Recalculate the candidates of elements in the same row, col and subgrid
            foreach (Element el in graph) {
                if (elementToRemove.AffectsCandidatesForOther(el)) {
                    el.RefreshCandidates(InitialValues);
                }
            }

            // Add a new graph element for the newly blank cell
            elementToRemove.RefreshCandidates(InitialValues);
            graph.Add(elementToRemove);
            
            Solve();
        }

        private static bool IsColumnSafe(int col, Element[,] grid) {
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                if (!grid[row, col].IsSafe(grid)) {
                    return false;
                }
            }
            return true;
        }

        private static bool IsColumnThirdSafe(int col, int colThird, Element[,] grid) {
            int[] validThirds = { 0, 1, 2 };
            if (!validThirds.Contains(colThird)) {
                throw new ArgumentException($"Must specify a column third from within {validThirds}");
            }
            for (int row = colThird * 3; row <= colThird * 3 + 2; row++) {
                if (!grid[row, col].IsSafe(grid)) return false;
            }
            return true;
        }

        private static int[,] ExtractFinalValuesFromElementGrid(Element[,] elementGrid) {
            int[,] finalValues = new int[elementGrid.GetLength(0), elementGrid.GetLength(1)];

        }

        private
    }
}

