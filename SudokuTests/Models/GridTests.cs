namespace SudokuTests;

using Newtonsoft.Json.Linq;
using Sudoku.Models;

[TestClass]
public class GridTests {
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

        Grid grid = new Grid(initialValues);
        bool result = grid.Solve();

        Assert.IsTrue(result);
        Assert.IsTrue(grid.CheckValuesEqual(expectedSolution));
        Assert.IsTrue(grid.IsSolved());
        Assert.IsTrue(grid.IsUnique());
    }

    [TestMethod]
    public void TestSolve_HappyPathNonUnique() {
        int[,] initialValues = {
                         { 3, 0, 6, 5, 0, 8, 4, 0, 0 },
                         { 5, 2, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 8, 7, 0, 0, 0, 0, 3, 1 },
                         { 0, 0, 3, 0, 1, 0, 0, 8, 0 },
                         { 9, 0, 0, 8, 6, 3, 0, 0, 5 },
                         { 0, 5, 0, 0, 9, 0, 6, 0, 0 },
                         { 1, 3, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 5, 2, 0, 6, 0, 0, 0 } };

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

        Grid grid = new Grid(initialValues);
        bool result = grid.Solve();

        Assert.IsTrue(result);
        grid.PrintValues();
        PrintValues(expectedSolution);
        Assert.IsTrue(grid.IsSolved());
        Assert.IsFalse(grid.IsUnique());
        //Assert.IsTrue(grid.CheckValuesEqual(expectedSolution));
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

    // An independent solution validator


    //[TestMethod]
    //public void TestSolve_EmptyInitial() {
    //    int[,] initialValues = {
    //                     { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //                     { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //                     { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //                     { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //                     { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //                     { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //                     { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //                     { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //                     { 0, 0, 0, 0, 0, 0, 0, 0, 0 } };

    //    Grid grid = new Grid(initialValues);
    //    bool result = grid.Solve();

    //    Assert.IsTrue(result);
    //}
}
