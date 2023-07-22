namespace SudokuTests;

using Sudoku.Models;
using System.Linq;

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

        Assert.IsTrue(Utils<int>.CheckArrayEquivalence(grid1, grid2));
    }

    [TestMethod]
    public void TestCheckGridEquivalence_False() {
        int[,] grid2 = {
                         { 1, 1, 6 },
                         { 5, 2, 9 },
                         { 4, 8, 7 } };

        Assert.IsFalse(Utils<int>.CheckArrayEquivalence(grid1, grid2));
    }

    [TestMethod]
    public void TestFlatten2DArray() {
        List<int> expectedList = new() { 3, 1, 6, 5, 2, 9, 4, 8, 7 };

        Assert.IsTrue(expectedList.SequenceEqual(Utils<int>.Flatten2DArray(grid1)));
    }

    [TestMethod]
    public void TestListify2DArray() {
        List<List<int>> expectedList = new() {  new() { 3, 1, 6 },
                                                new() { 5, 2, 9 },
                                                new() { 4, 8, 7 } };
        IList<List<int>> resultList = Utils<int>.Listify2DArray(grid1);

        Assert.AreEqual(expectedList.Count, resultList.Count);
        for (int i = 0; i < resultList.Count; i++) {
            Assert.IsTrue(expectedList.ElementAt(i).SequenceEqual(resultList.ElementAt(i)));
        }
    }

    [TestMethod]
    public void TestCreate2DArray_Successful() {
        List<List<int>> listOfLists = new() {   new() { 1, 2, 4 },
                                                new() { 1, 7, 7 },
                                                new() { 5, 8, 4 } };

        int[,]? expectedArray = {{ 1, 2, 4 },
                                 { 1, 7, 7 },
                                 { 5, 8, 4 } };

        int[,] result = Utils<int>.Create2DArray(listOfLists) ?? grid1;
        Assert.IsTrue(Utils<int>.CheckArrayEquivalence(expectedArray, result));
    }

    [TestMethod]
    public void TestCreate2DArray_NoData() {
        List<List<int>> listOfLists = new() { new() {} };
        Assert.IsNull(Utils<int>.Create2DArray(listOfLists));
    }

    [TestMethod]
    public void TestCreate2DArray_UnevenLists() {
        List<List<int>> listOfLists = new() {   new() { 3, 1, 6 },
                                                new() { 5, 2 },
                                                new() { 4, 8, 7 } };
        Assert.IsNull(Utils<int>.Create2DArray(listOfLists));
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