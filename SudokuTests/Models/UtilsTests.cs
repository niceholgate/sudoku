namespace SudokuTests;

using Sudoku.Models;

[TestClass]
public class UtilsTests {

    private static readonly int[,] grid1 = {
                         { 3, 1, 6 },
                         { 5, 2, 9 },
                         { 4, 8, 7 } };

    private static readonly List<int> bigList = Enumerable.Range(0, 10000).ToList();

    [TestMethod]
    public void TestCheckGridEquivalence_True() {
        int[,] grid2 = {
                         { 3, 1, 6 },
                         { 5, 2, 9 },
                         { 4, 8, 7 } };

        Assert.IsTrue(Utils<int>.CheckGridEquivalence(grid1, grid2));
    }

    [TestMethod]
    public void TestCheckGridEquivalence_False() {
        int[,] grid2 = {
                         { 1, 1, 6 },
                         { 5, 2, 9 },
                         { 4, 8, 7 } };

        Assert.IsFalse(Utils<int>.CheckGridEquivalence(grid1, grid2));
    }

    [TestMethod]
    public void TestFlatten2DArray() {
        List<int> expectedList = new() { 3, 1, 6, 5, 2, 9, 4, 8, 7 };

        Assert.IsTrue(expectedList.SequenceEqual(Utils<int>.Flatten2DArray(grid1)));
    }

    [TestMethod]
    public void TestShuffle_DoesNotRepeat() {
        List<int> firstShuffle = (List<int>)Utils<int>.Shuffle(bigList);
        List<int> secondShuffle = (List<int>)Utils<int>.Shuffle(bigList);
        Assert.AreNotEqual(bigList, firstShuffle);
        Assert.AreNotEqual(firstShuffle, secondShuffle);
    }

    [TestMethod]
    public void TestSelectRandomElement_DoesNotRepeat() {
        int first = Utils<int>.SelectRandomElement(bigList);
        int second = Utils<int>.SelectRandomElement(bigList);
        Assert.AreNotEqual(first, second);
    }
}