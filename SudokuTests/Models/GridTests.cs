namespace SudokuTests;
using Sudoku.Models;

[TestClass]
public class GridTests
{
    [TestMethod]
    public void TestSolve()
    {
        Grid grid = new Grid();
        bool result = grid.Solve();
        int a = 2;
    }
}
