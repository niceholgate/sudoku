namespace Sudoku.Models {

    // TODO: implements IClonable
	public class Grid {
        public static readonly int SUDOKU_ROWS_COLS = 9;

        public static readonly int SUDOKU_SUBGRID = 3;

        public static readonly int MAX_SOLUTIONS_LIMIT = 500;

        public Element[,] InitialElements { get; }

        public List<int[,]> Solutions { get; } = new();

        /*
         * Used to look for Solutions.
         */
        private Element[,] workingValues = new Element[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

        /*
         * Used to look for Solutions - refers to workingValues elements.
         */
        private List<Element> graph = new();

        /*
         * TODO: what is this?
         */
        public List<Element> initiallyNonEmptyElements = new();

        /*
         * Constructors for new grid.
         */
        public Grid(int[,] initialValues) {
            InitialElements = FinalValuesGridToUninitializedElementGrid(initialValues);
            Initialize();
        }

        public Grid(Element[,] initialElements) {
            InitialElements = initialElements;
            Initialize();
        }

        /*
         * Constructor for cloning.
         */
        public Grid(int[,] initialValues, List<int[,]> solutions, Element[,] workingValues, List<Element> graph, List<Element> nonEmptyElements) {
            this.InitialElements = FinalValuesGridToUninitializedElementGrid(initialValues);
            this.Solutions = solutions;
            this.workingValues = workingValues;
            this.graph = graph;
            this.initiallyNonEmptyElements = nonEmptyElements;
        }

        public Grid Clone() {
            return new Grid(
                ElementGridToFinalValuesGrid(this.InitialElements),
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
            for (int col = 0; col < 9; col++) {
                values[randomRowIndex, col] = randomRow[col];
            }

            Element[,] elementGrid = FinalValuesGridToUninitializedElementGrid(values);

            // Add a random column - must shuffle it until it fits with existing row
            int randomColIndex = Utils<int>.RANDOM_NUMBER_GENERATOR.Next(0, 9);
            int[] randomCol;
            while (!IsSubColumnSafe(randomColIndex, randomRowIndex / 3, randomRowIndex / 3 * 3 + 2, elementGrid)) {
                randomCol = Utils<int>.ShuffleToArray(Enumerable.Range(1, 9));
                for (int row = 0; row < 9; row++) {
                    elementGrid[row, randomColIndex] = new Element(row, randomColIndex, randomCol[row]);
                }
            }

            Grid grid = new(ElementGridToFinalValuesGrid(elementGrid));
            grid.Solve(1);
            // Creating yet another grid is a hack to replace the initial values with the solution
            Grid finalGrid = new(grid.Solutions[0]);
            finalGrid.Solve(1);
            return finalGrid;
        }

        /*
         * Generate a random new puzzle with a unique solution. Typically gives a puzzle with between ~20 and ~50 empty cells;
         * indicative of the difficulty of the problem.
         * Plotted a histogram of 400 initially empty cell counts (81 - initiallyNonEmptyElements)
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

        public static bool IsSolved(Element[,] grid) { 
            for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                if (!IsColumnSafe(col, grid)) return false;
            }
            return true;
        }

        private static bool IsColumnSafe(int col, Element[,] grid) {
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                if (!grid[row, col].IsSafe(grid)) {
                    return false;
                }
            }
            return true;
        }

        private static bool IsSubColumnSafe(int col, int rowStart, int rowEnd, Element[,] grid) {
            for (int row = rowStart; row <= rowEnd; row++) {
                if (!grid[row, col].IsSafe(grid)) return false;
            }
            return true;
        }

        private static int[,] ElementGridToFinalValuesGrid(Element[,] elementGrid) {
            int[,] finalValues = new int[elementGrid.GetLength(0), elementGrid.GetLength(1)];
            for (int row = 0; row < elementGrid.GetLength(0); row++) {
                for (int col = 0; col < elementGrid.GetLength(1); col++) {
                    finalValues[row, col] = elementGrid[row, col].FinalValue;
                }
            }
            return finalValues;
        }

        private static Element[,] FinalValuesGridToUninitializedElementGrid(int[,] finalValues) {
            Element[,] elementGrid = new Element[finalValues.GetLength(0), finalValues.GetLength(1)];
            for (int row = 0; row < finalValues.GetLength(0); row++) {
                for (int col = 0; col < finalValues.GetLength(1); col++) {
                    elementGrid[row, col] = new Element(row, col, finalValues[row, col]);
                }
            }
            return elementGrid;
        }

        public bool AllSolutionsValid() {
            foreach (int[,] soln in Solutions) {
                if (!IsSolved(FinalValuesGridToUninitializedElementGrid(soln))) return false;
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
                Solutions.Add(ElementGridToFinalValuesGrid(workingValues));
                // Return false unless decide to finish because have enough solutions (or all?)
                if (Solutions.Count >= maxSolutions) return true;
                return false;
            }

            graph.Sort((Element a, Element b) => b.Candidates.Count - a.Candidates.Count);
            Element el = graph.ElementAt(graph.Count - 1);


            List<int> oldCandidates = new List<int>(el.Candidates);
            for (int i = 0; i < oldCandidates.Count; ++i) { // TODO: i++ ?
                int num = el.Candidates.ElementAt(i);
                if (el.IsSafe(workingValues, num)) {
                    el.Candidates = new List<int>(){ num };
                    graph.RemoveAt(graph.Count - 1);
                    if (Solve(maxSolutions)) return true;
                    graph.Add(el);
                    el.Candidates = oldCandidates;
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
                    Element el = new(row, col, InitialElements[row, col].FinalValue);
                    workingValues[row, col] = el;
                    if (el.FinalValue == 0) {
                        el.RefreshCandidates(InitialElements);
                        graph.Add(el);
                    } else {
                        initiallyNonEmptyElements.Add(el);
                    }
                }
            }
        }

        private void RemoveInitialValueAndRefresh(Element elementToRemove) {
            Solutions.Clear();

            if (InitialElements[elementToRemove.row, elementToRemove.col].FinalValue == 0) {
                throw new DataMisalignedException("No initial value to remove at " +
                    $"({elementToRemove.row}, {elementToRemove.col})");
            }

            // Remove the initial value by giving it multiple Candidates (initial value Elements should have a single Candidate; refresh to find real candidates later)
            // TODO: can just do the refresh here?
            InitialElements[elementToRemove.row, elementToRemove.col].Candidates = new List<int>() { 1 , 2 };
            initiallyNonEmptyElements.Remove(elementToRemove);

            // Recalculate the candidates of elements in the same row, col and subgrid
            foreach (Element el in graph) {
                if (elementToRemove.AffectsCandidatesForOther(el)) {
                    el.RefreshCandidates(InitialElements);
                }
            }

            // Add a new graph element for the newly blank cell
            elementToRemove.RefreshCandidates(InitialElements);
            graph.Add(elementToRemove);
            
            Solve();
        }

    }
}

