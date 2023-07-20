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
         * Constructors for new grid.
         */
        public Grid(int[,] initialValues) {
            InitialElements = IntGridToUninitializedElementGrid(initialValues);
            Initialize();
        }

        public Grid(Element[,] initialElements) {
            InitialElements = initialElements;
            Initialize();
        }

        /*
         * Constructor for cloning.
         */
        public Grid(int[,] initialValues, List<int[,]> solutions, Element[,] workingValues, List<Element> graph) {
            this.InitialElements = IntGridToUninitializedElementGrid(initialValues);
            this.Solutions = solutions;
            this.workingValues = workingValues;
            this.graph = graph;
        }

        public Grid Clone() {
            return new Grid(
                ElementGridToIntGrid(this.InitialElements),
                new List<int[,]>(this.Solutions),
                (Element[,])this.workingValues.Clone(),
                new List<Element>(this.graph));
        }

        /*
         * Generate a random completely filled grid based on a random initial row + column solved using 
         * the backtracking solver (deterministic based on the initial values).
         */
        public static Grid GenerateRandomFilled() {

            int[,] values = new int[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

            // Create a random row and insert it randomly
            IList<int> randomRow = Utils<int>.Shuffle(Enumerable.Range(1, 9));
            int randomRowIndex = Utils<int>.RANDOM_NUMBER_GENERATOR.Next(0, 9);
            for (int col = 0; col < 9; col++) {
                values[randomRowIndex, col] = randomRow[col];
            }

            Element[,] elementGrid = IntGridToUninitializedElementGrid(values);

            // Add a random column - must shuffle it until it fits with existing row
            int randomColIndex = Utils<int>.RANDOM_NUMBER_GENERATOR.Next(0, 9);
            int subGridRow = randomRowIndex / 3;
            while (!IsSubColumnSafe(randomColIndex, subGridRow, subGridRow * 3 + 2, elementGrid)) {
                IList<int> randomCol = Utils<int>.Shuffle(Enumerable.Range(1, 9));
                for (int row = 0; row < 9; row++) {
                    elementGrid[row, randomColIndex] = new Element(row, randomColIndex, randomCol[row]);
                }
            }

            Grid grid = new(elementGrid);
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

            // Generate (then maintain) a shuffled list of the filled Elements
            Queue<Element> shuffledInitialElements = new(
                Utils<Element>.Shuffle(
                    Utils<Element>.Flatten2DArray(grid.InitialElements)));

            // Remove random filled elements until the solution stops being unique
            Grid previousGrid = grid.Clone();
            while (grid.Solutions.Count == 1) {
                previousGrid = grid.Clone();
                Element newlyBlankInitial = shuffledInitialElements.Dequeue();
                newlyBlankInitial.Candidates.Clear();
                grid.Reinitialize(newlyBlankInitial);
                grid.Solve();
            }
            return previousGrid;
        }

        public static bool IsSolved(Element[,] grid) { 
            for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                if (!IsSubColumnSafe(col, 0, SUDOKU_ROWS_COLS-1, grid)) return false;
            }
            return true;
        }

        public bool AllSolutionsValid() {
            foreach (int[,] soln in Solutions) {
                if (!IsSolved(IntGridToUninitializedElementGrid(soln))) return false;
            }
            return true;
        }

        public bool Solve() {
            return Solve(MAX_SOLUTIONS_LIMIT);
        }

        private static bool IsSubColumnSafe(int col, int rowStart, int rowEnd, Element[,] grid) {
            for (int row = rowStart; row <= rowEnd; row++) {
                if (!grid[row, col].IsSafe(grid)) return false;
            }
            return true;
        }

        private static int[,] ElementGridToIntGrid(Element[,] elementGrid) {
            int[,] finalValues = new int[elementGrid.GetLength(0), elementGrid.GetLength(1)];
            for (int row = 0; row < elementGrid.GetLength(0); row++) {
                for (int col = 0; col < elementGrid.GetLength(1); col++) {
                    finalValues[row, col] = elementGrid[row, col].FinalValue;
                }
            }
            return finalValues;
        }

        private static Element[,] IntGridToUninitializedElementGrid(int[,] finalValues) {
            Element[,] elementGrid = new Element[finalValues.GetLength(0), finalValues.GetLength(1)];
            for (int row = 0; row < finalValues.GetLength(0); row++) {
                for (int col = 0; col < finalValues.GetLength(1); col++) {
                    elementGrid[row, col] = new Element(row, col, finalValues[row, col]);
                }
            }
            return elementGrid;
        }

        // TODO: flag if all solutions were found and added to Solutions
        private bool Solve(int maxSolutions) {
            if (maxSolutions > MAX_SOLUTIONS_LIMIT) {
                throw new ArgumentException($"Must specify a maximum number of solutions <= {MAX_SOLUTIONS_LIMIT}");
            } 

            if (graph.Count == 0) {
                Solutions.Add(ElementGridToIntGrid(workingValues));
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
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                    Element el = new(row, col, InitialElements[row, col].FinalValue);
                    workingValues[row, col] = el;
                    if (el.FinalValue == 0) {
                        el.RefreshCandidates(InitialElements);
                        graph.Add(el);
                    }
                }
            }
        }

        /*
         * If one InitialElement is removed, it is now unknown so gets added to the graph.
         * Then only need to refresh Candidates for that Element and others in the graph which are possibly affected by it (same row/col/subgrid).
         */
        private void Reinitialize(Element initialElementRemoved) {
            Solutions.Clear();
            workingValues[initialElementRemoved.row, initialElementRemoved.col] = new Element(initialElementRemoved.row, initialElementRemoved.col, 0);
            graph.Add(workingValues[initialElementRemoved.row, initialElementRemoved.col]);
            foreach (Element el in graph) {
                if (initialElementRemoved.AffectsCandidatesForOther(el)) {
                    el.RefreshCandidates(InitialElements);
                }
            }
        }

    }
}

