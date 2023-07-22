using System;

namespace Sudoku.Models {

	public class Grid : ICloneable {
        public static readonly int SUDOKU_ROWS_COLS = 9;

        public static readonly int SUDOKU_SUBGRID = 3;

        public static readonly int MAX_SOLUTIONS_LIMIT = 500;

        public Element[,] InitialElements { get; }

        public int[,] InitialValues { get { return ElementGridToIntGrid(InitialElements); } }

        public List<Solution> Solutions { get; } = new();

        /*
         * Used to track the steps used in arriving at each Solution. Add it to Solutions list once it is finalized.
         */
        private Solution workingSolution = new();

        /*
         * Used to look for Solutions.
         */
        private Element[,] workingElements = new Element[SUDOKU_ROWS_COLS, SUDOKU_ROWS_COLS];

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
        public Grid(int[,] initialValues, List<Solution> solutions, Element[,] workingValues, List<Element> graph) {
            this.InitialElements = IntGridToUninitializedElementGrid(initialValues);
            this.Solutions = solutions;
            this.workingElements = workingValues;
            this.graph = graph;
        }

        public object Clone() {
            return new Grid(
                ElementGridToIntGrid(this.InitialElements),
                new List<Solution>(this.Solutions),
                (Element[,])this.workingElements.Clone(),
                new List<Element>(this.graph));
        }

        // It seems like one or both of the below methods is slow sometimes.
        // Maybe could analyse Solve() stack depths and come up with kill criteria which predict when it will be slow (and restart).
        // Or actually fix the algo...

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
            Grid finalGrid = new(grid.Solutions[0].FinalValues);
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
            Grid previousGrid = (Grid)grid.Clone();
            while (grid.Solutions.Count == 1) {
                previousGrid = (Grid)grid.Clone();
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
            foreach (Solution soln in Solutions) {
                if (!IsSolved(IntGridToUninitializedElementGrid(soln.FinalValues))) return false;
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

        private bool Solve(int maxSolutions) {
            if (maxSolutions > MAX_SOLUTIONS_LIMIT) {
                throw new ArgumentException($"Must specify a maximum number of solutions <= {MAX_SOLUTIONS_LIMIT}");
            } 

            if (graph.Count == 0) {
                int[,] finalValues = ElementGridToIntGrid(workingElements);
                workingSolution.FinalValues = finalValues;
                Solutions.Add(workingSolution);
                workingSolution = new Solution();
                // Return false unless decide to finish because reached maxSolutions
                if (Solutions.Count == maxSolutions) return true;
                return false;
            }

            graph.Sort((Element a, Element b) => b.Candidates.Count - a.Candidates.Count);
            Element el = graph.ElementAt(graph.Count - 1);

            // Testing out a Candidate is done by setting it as the only one
            // If no solutions are found for any of this Element's Candidates, put them all back afterwards
            List<int> candidates = new(el.Candidates);
            foreach (int c in candidates) {
                el.Candidates = new List<int>() { c };
                if (el.IsSafe(workingElements)) {
                    workingSolution.AddNewStep(new Solution.Step(el.row, el.col, el.FinalValue));
                    graph.RemoveAt(graph.Count - 1);
                    if (Solve(maxSolutions)) {
                        workingSolution.RemoveLastStep();
                        return true;
                    }
                    workingSolution.RemoveLastStep();
                    graph.Add(el);
                }
            }
            el.Candidates = candidates;
            return false;
        }

        /*
         * Prepares the Grid for solving given current InitialElements according to:
         * clear the Solutions and graph,
         * copy the InitialValues to the workingValues,
         * then populate the graph and find Candidates for graph Elements.
         */
        private void Initialize() {
            Solutions.Clear();
            graph.Clear();
            for (int row = 0; row < SUDOKU_ROWS_COLS; row++) {
                for (int col = 0; col < SUDOKU_ROWS_COLS; col++) {
                    Element el = new(row, col, InitialElements[row, col].FinalValue);
                    workingElements[row, col] = el;
                    if (el.FinalValue == 0) {
                        el.RefreshCandidates(InitialElements);
                        graph.Add(el);
                    }
                }
            }
        }

        /*
         * More efficient initialization if one InitialElement is removed. It is now unknown so gets added to the graph.
         * Then only need to refresh Candidates for that Element and others in the graph which are possibly affected by it (same row/col/subgrid).
         */
        private void Reinitialize(Element initialElementRemoved) {
            Solutions.Clear();
            workingElements[initialElementRemoved.row, initialElementRemoved.col] = new Element(initialElementRemoved.row, initialElementRemoved.col, 0);
            graph.Add(workingElements[initialElementRemoved.row, initialElementRemoved.col]);
            foreach (Element el in graph) {
                if (initialElementRemoved.AffectsCandidatesForOther(el)) {
                    el.RefreshCandidates(InitialElements);
                }
            }
        }

    }
}

