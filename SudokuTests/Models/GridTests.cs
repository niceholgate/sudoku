﻿namespace SudokuTests;

using System.Linq;
using Sudoku.Models;

[TestClass]
public class GridTests {

    [TestMethod]
    public void TestGenerateRandomFilled() {
        Enumerable.Range(0, 5).Select(i => Grid.GenerateRandomFilled()).ToList().ForEach(grid => {
                Assert.IsTrue(Grid.IsSolved(grid.InitialElements));
                Assert.AreEqual(1, grid.Solutions.Count);
                Assert.IsTrue(grid.AllSolutionsValid());
            });
    }

    [TestMethod]
    public void TestGenerateRandomUniqueSparse() {
        Enumerable.Range(0, 5).Select(i => Grid.GenerateRandomUniqueSparse()).ToList().ForEach(grid => {
            Assert.IsFalse(Grid.IsSolved(grid.InitialElements));
            Assert.AreEqual(1, grid.Solutions.Count);
            Assert.IsTrue(grid.AllSolutionsValid());
        });
    }

    [TestMethod]
    public void TestIsSolved_True() {
        int[,] solvedGrid = {
                         { 3, 1, 6, 5, 7, 8, 4, 9, 2 },
                         { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                         { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                         { 2, 6, 3, 4, 1, 5, 9, 8, 7 },
                         { 9, 7, 4, 8, 6, 3, 1, 2, 5 },
                         { 8, 5, 1, 7, 9, 2, 6, 4, 3 },
                         { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                         { 6, 9, 2, 3, 5, 1, 8, 7, 4 },
                         { 7, 4, 5, 2, 8, 6, 3, 1, 9 } };

        Grid grid = new(solvedGrid);
        Assert.IsTrue(Grid.IsSolved(grid.InitialElements));

    }

    [TestMethod]
    public void TestIsSolved_False() {
        int[,] solvedGrid = {
                         { 3, 1, 6, 5, 7, 8, 4, 9, 2 },
                         { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                         { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                         { 2, 6, 3, 4, 1, 5, 9, 8, 7 },
                         { 9, 7, 4, 8, 6, 3, 1, 2, 5 },
                         { 8, 5, 1, 7, 9, 2, 6, 4, 3 },
                         { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                         { 6, 9, 2, 3, 5, 1, 8, 7, 4 },
                         { 7, 4, 5, 2, 8, 6, 3, 1, 0 } };

        Grid grid = new(solvedGrid);
        Assert.IsFalse(Grid.IsSolved(grid.InitialElements));
    }

    [TestMethod]
    public void TestSolve_HappyPathTrivial() {
        int[,] initialValues = {
                         { 3, 1, 6, 5, 7, 8, 4, 9, 2 },
                         { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                         { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                         { 2, 6, 3, 4, 1, 5, 9, 8, 7 },
                         { 9, 7, 4, 8, 6, 3, 1, 2, 5 },
                         { 8, 5, 1, 7, 9, 2, 6, 4, 3 },
                         { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                         { 6, 9, 2, 3, 5, 1, 8, 7, 4 },
                         { 7, 4, 5, 2, 8, 6, 3, 1, 0 } };

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

        Assert.AreEqual(1, grid.Solutions.Count);
        Assert.IsTrue(grid.AllSolutionsValid());
        Assert.IsTrue(Utils<int>.CheckGridEquivalence(expectedSolution, grid.Solutions[0]));
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

        Assert.AreEqual(1, grid.Solutions.Count);
        Assert.IsTrue(grid.AllSolutionsValid());
        Assert.IsTrue(Utils<int>.CheckGridEquivalence(expectedSolution, grid.Solutions[0]));
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

        Assert.AreEqual(6, grid.Solutions.Count);
        Assert.IsTrue(grid.AllSolutionsValid());
        int numMatchingExpectedSolution = grid.Solutions.Select(soln => Utils<int>.CheckGridEquivalence(expectedSolution, soln)).Count(x => x == true);
        Assert.AreEqual(1, numMatchingExpectedSolution);
    }



 
}

