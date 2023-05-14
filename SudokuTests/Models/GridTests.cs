namespace SudokuTests;

using System.Diagnostics;
using System.Linq;
using Sudoku.Models;


[TestClass]
public class GridTests {
    [TestMethod]
    public void TestIsSolved_True() {
        int[,] expectedSolution = {
                         { 3, 1, 6, 5, 7, 8, 4, 9, 2 },
                         { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                         { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                         { 2, 6, 3, 4, 1, 5, 9, 8, 7 },
                         { 9, 7, 4, 8, 6, 3, 1, 2, 5 },
                         { 8, 5, 1, 7, 9, 2, 6, 4, 3 },
                         { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                         { 6, 9, 2, 3, 5, 1, 8, 7, 4 },
                         { 7, 4, 5, 2, 8, 6, 3, 1, 9 } };

        Grid grid = new(expectedSolution);
        grid.Solve();
        Assert.AreEqual(grid.Solutions.Count, 1);
        Assert.IsTrue(grid.IsSolved(0));
        Assert.IsTrue(Utils.CheckGridEquivalence(expectedSolution, grid.Solutions[0]));
    } 

    [TestMethod]
    public void TestSolve_HappyPathUnique() {
         int[,] initialValues = {
                         { 3, 0, 6, 5, 0, 8, 4, 0, 0 },
                         { 5, 2, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 8, 7, 0, 0, 0, 0, 3, 1 },
                         { 0, 0, 3, 0, 1, 0, 0, 8, 0 },
                         { 9, 0, 0, 8, 6, 3, 0, 0, 5 },
                         { 0, 5, 0, 0, 9, 0, 6, 0, 0 },
                         { 1, 3, 0, 0, 0, 0, 2, 5, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 7, 4 },
                         { 0, 0, 5, 2, 0, 6, 3, 0, 0 } };

        int[,] expectedSolution = {
                         { 3, 1, 6, 5, 7, 8, 4, 9, 2 },
                         { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                         { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                         { 2, 6, 3, 4, 1, 5, 9, 8, 7 },
                         { 9, 7, 4, 8, 6, 3, 1, 2, 5 },
                         { 8, 5, 1, 7, 9, 2, 6, 4, 3 },
                         { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                         { 6, 9, 2, 3, 5, 1, 8, 7, 4 },
                         { 7, 4, 5, 2, 8, 6, 3, 1, 9 } };

        Grid grid = new(initialValues);
        grid.Solve();

        Assert.AreEqual(grid.Solutions.Count, 1);
        Assert.IsTrue(grid.IsSolved(0));
        Assert.IsTrue(Utils.CheckGridEquivalence(expectedSolution, grid.Solutions[0]));
    }

    [TestMethod]
    public void TestSolve_HappyPathNonUnique() {
        // Checked the valid solution count with 
        // https://www.thonky.com/sudoku/solution-count?puzzle=3.65.84..52........87....31..3.1..8.9..863..5.5..9.6..13..............9...52.6.14
        int[,] initialValues = {
                         { 3, 0, 6, 5, 0, 8, 4, 0, 0 },
                         { 5, 2, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 8, 7, 0, 0, 0, 0, 3, 1 },
                         { 0, 0, 3, 0, 1, 0, 0, 8, 0 },
                         { 9, 0, 0, 8, 6, 3, 0, 0, 5 },
                         { 0, 5, 0, 0, 9, 0, 6, 0, 0 },
                         { 1, 3, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 9, 0 },
                         { 0, 0, 5, 2, 0, 6, 0, 1, 4 } };

        int[,] expectedSolution = {
                        { 3, 1, 6, 5, 7, 8, 4, 2, 9 },
                        { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                        { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                        { 6, 4, 3, 7, 1, 5, 9, 8, 2 },
                        { 9, 7, 2, 8, 6, 3, 1, 4, 5 },
                        { 8, 5, 1, 4, 9, 2, 6, 7, 3 },
                        { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                        { 2, 6, 4, 3, 5, 1, 8, 9, 7 },
                        { 7, 9, 5, 2, 8, 6, 3, 1, 4 } };

        Grid grid = new(initialValues);
        grid.Solve();

        Assert.AreEqual(grid.Solutions.Count, 6);
        Assert.IsTrue(Enumerable.Range(0, grid.Solutions.Count).Select(grid.IsSolved).All(x => x == true));
        Assert.AreEqual(Enumerable.Range(0, grid.Solutions.Count).Select(i => Utils.CheckGridEquivalence(expectedSolution, grid.Solutions[i])).Count(x => x == true), 1);
    }

    // Print any grid
    public void PrintValues(int [,] values) {
        for (int i = 0; i < values.GetLength(0); i++) {
            for (int j = 0; j < values.GetLength(1); j++) {
                Console.Write(values[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }


    [TestMethod]
    public void TestGenerateRandomFilled() {
        List<Grid> randomGrids = Enumerable.Range(0, 10).Select(x => Grid.GenerateRandomFilled()).ToList();
        foreach (Grid grid in randomGrids) {
            Assert.AreEqual(1, grid.Solutions.Count);
            Assert.IsTrue(grid.IsSolved(0));
        }
    }

    [TestMethod]
    public void TestGenerateRandomUniqueSparse() {
        List<Grid> randomGrids = Enumerable.Range(0, 5).Select(x => Grid.GenerateRandomUniqueSparse()).ToList();
        foreach (Grid grid in randomGrids) {
            Assert.IsTrue(grid.initiallyNonEmptyElements.Count > 0);
            Assert.AreEqual(1, grid.Solutions.Count);
            Assert.IsTrue(grid.IsSolved(0));
        }
    }
}

// Various initial values tests
//int[,] initialValues = {
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
//                 { 0, 0, 0, 8, 0, 0, 0, 0, 0 },
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
