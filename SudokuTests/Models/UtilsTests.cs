namespace SudokuTests;

using System.Linq;
using Sudoku.Models;

[TestClass]
public class UtilsTests {
    [TestMethod]
    public void TestCheckGridEquivalence_True() {
        int[,] grid1 = {
                         { 3, 1, 6 },
                         { 5, 2, 9 },
                         { 4, 8, 7 } };

        int[,] grid2 = {
                         { 3, 1, 6 },
                         { 5, 2, 9 },
                         { 4, 8, 7 } };

        Assert.IsTrue(Utils<int>.CheckGridEquivalence(grid1, grid2));
    }

    [TestMethod]
    public void TestCheckGridEquivalence_False() {
        int[,] grid1 = {
                         { 3, 1, 6 },
                         { 5, 2, 9 },
                         { 4, 8, 7 } };

        int[,] grid2 = {
                         { 1, 1, 6 },
                         { 5, 2, 9 },
                         { 4, 8, 7 } };

        Assert.IsFalse(Utils<int>.CheckGridEquivalence(grid1, grid2));
    }
}