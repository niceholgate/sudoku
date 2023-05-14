namespace Sudoku.Models {

    // TODO: implements IClonable
	public class Grid {
        public static readonly int SUDOKU_ROWS_COLS = 9;

        public static readonly int SUDOKU_SUBGRID = 3;

        public static readonly int MAX_SOLUTIONS_LIMIT = 500;

        private static Random RANDOM_NUMBER_GENERATOR = new Random();

        public int[,] InitialValues { get; }

        public List<int[,]> Solutions { get; } = new List<int[,]>();

        private int[,] workingValues = new int[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

        private List<Element> graph = new();

        public List<Element> initiallyNonEmptyElements = new();

        public Grid(int[,] initialValues) {
            InitialValues = initialValues;
            workingValues = (int[,])initialValues.Clone();
            Initialize();
        }

        public Grid(int[,] initialValues, List<int[,]> solutions, int[,] workingValues, List<Element> graph, List<Element> nonEmptyElements) {
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
                (int[,])this.workingValues.Clone(),
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
                Element randomNonEmptyElement = grid.initiallyNonEmptyElements.ElementAt(RANDOM_NUMBER_GENERATOR.Next(0, grid.initiallyNonEmptyElements.Count));
                grid.RemoveInitialValueAndRefresh(randomNonEmptyElement);
            }
            return previousGrid;
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

        // TODO: refactor to Element class
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
                    //nonEmptyElements.Add(newNonEmptyElement);
                    if (Solve(maxSolutions)) return true;
                    //nonEmptyElements.Remove(newNonEmptyElement);
                    graph.Add(el);
                    graph.Sort((Element a, Element b) => b.Candidates.Count - a.Candidates.Count);
                    workingValues[el.row, el.col] = 0;
                }
            }
            return false;
        }

        public void Initialize() {
            Solutions.Clear();
            graph.Clear();
            initiallyNonEmptyElements.Clear();
            workingValues = InitialValues;
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                    if (InitialValues[row, col] == 0) {
                        Element el = new Element(row, col);
                        RefreshCandidates(el);
                        graph.Add(el);
                    } else {
                        initiallyNonEmptyElements.Add(new Element(row, col));
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
                    RefreshCandidates(el);
                }
            }

            // Add a new graph element for the newly blank cell
            RefreshCandidates(elementToRemove);
            graph.Add(elementToRemove);
            
            Solve();
        }

        /* TODO: refactor to Element class
         * Add the candidates to an element given the InitialValues grid.
         */
        private void RefreshCandidates(Element element) {
            element.Candidates.Clear();
            for (int num = 1; num <= SUDOKU_ROWS_COLS; num++) {
                if (IsSafe(InitialValues, element, num)) {
                    element.Candidates.Add(num);
                }
            }
        }

	}
}

