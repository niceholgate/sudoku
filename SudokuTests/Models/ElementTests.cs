namespace SudokuTests;

using Sudoku.Models;

[TestClass]
public  class ElementTests {
    [TestMethod]
    public void TestInitialInvalidation_IllegalRow() {
        var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Element(9, 0, 0));
        Assert.AreEqual("Tried to make an Element with an illegal Sudoku row value: 9 (Parameter 'row')", ex.Message);
        ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Element(-1, 0, 0));
        Assert.AreEqual("Tried to make an Element with an illegal Sudoku row value: -1 (Parameter 'row')", ex.Message);
    }

    [TestMethod]
    public void TestInitialInvalidation_IllegalCol() {
        var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Element(0, 9, 0));
        Assert.AreEqual("Tried to make an Element with an illegal Sudoku col value: 9 (Parameter 'col')", ex.Message);
        ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Element(0, -1, 0));
        Assert.AreEqual("Tried to make an Element with an illegal Sudoku col value: -1 (Parameter 'col')", ex.Message);
    }

    [TestMethod]
    public void TestInitialInvalidation_IllegalValue() {
        var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Element(0, 0, 10));
        Assert.AreEqual("Tried to make an Element with an illegal Sudoku cell value: 10 (Parameter 'finalValue')", ex.Message);
        ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Element(0, 0, -1));
        Assert.AreEqual("Tried to make an Element with an illegal Sudoku cell value: -1 (Parameter 'finalValue')", ex.Message);
    }

    [TestMethod]
    public void TestIsSafe_True() {
        int[,] initialValues = {
                         { 3, 1, 6, 5, 7, 8, 4, 9, 2 },
                         { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                         { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                         { 2, 6, 3, 4, 1, 5, 9, 8, 7 },
                         { 9, 7, 4, 8, 6, 3, 1, 2, 5 },
                         { 8, 5, 1, 7, 9, 2, 6, 4, 3 },
                         { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                         { 6, 9, 2, 3, 5, 1, 8, 7, 0 },
                         { 7, 0, 5, 2, 8, 6, 3, 1, 0 } };

        Grid grid = new(initialValues);
        Element el88 = new(8, 8, 4);

        Assert.IsTrue(el88.IsSafe(grid.InitialElements));
    }

    [TestMethod]
    public void TestIsSafe_False() {
        int[,] initialValues = {
                         { 3, 1, 6, 5, 7, 8, 4, 9, 2 },
                         { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                         { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                         { 2, 6, 3, 4, 1, 5, 9, 8, 7 },
                         { 9, 7, 4, 8, 6, 3, 1, 2, 5 },
                         { 8, 5, 1, 7, 9, 2, 6, 4, 3 },
                         { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                         { 6, 9, 2, 3, 5, 1, 8, 7, 0 },
                         { 7, 0, 5, 2, 8, 6, 3, 1, 0 } };

        Grid grid = new(initialValues);
        Element el88 = new(8, 8, 1);

        Assert.IsFalse(el88.IsSafe(grid.InitialElements));
    }

    [TestMethod]
    public void TestIsSafe_ZeroFalse() {
        int[,] initialValues = {
                         { 3, 1, 6, 5, 7, 8, 4, 9, 2 },
                         { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                         { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                         { 2, 6, 3, 4, 1, 5, 9, 8, 7 },
                         { 9, 7, 4, 8, 6, 3, 1, 2, 5 },
                         { 8, 5, 1, 7, 9, 2, 6, 4, 3 },
                         { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                         { 6, 9, 2, 3, 5, 1, 8, 7, 0 },
                         { 7, 0, 5, 2, 8, 6, 3, 1, 0 } };

        Grid grid = new(initialValues);
        Element el88 = new(8, 8, 0);

        Assert.IsFalse(el88.IsSafe(grid.InitialElements));
    }

    [TestMethod]
    public void TestRefreshCandidates() {
        int[,] initialValues = {
                         { 3, 1, 6, 5, 7, 8, 4, 9, 2 },
                         { 5, 2, 9, 1, 3, 4, 7, 6, 8 },
                         { 4, 8, 7, 6, 2, 9, 5, 3, 1 },
                         { 2, 6, 3, 4, 1, 5, 9, 8, 7 },
                         { 9, 7, 4, 8, 6, 3, 1, 2, 5 },
                         { 8, 5, 1, 7, 9, 2, 6, 4, 3 },
                         { 1, 3, 8, 9, 4, 7, 2, 5, 6 },
                         { 6, 9, 2, 3, 5, 1, 8, 7, 0 },
                         { 7, 0, 5, 2, 8, 6, 3, 1, 0 } };

        Grid grid = new(initialValues);
        Element el88 = new(8, 8, 0);
        Element el78 = new(7, 8, 0);
        Assert.IsTrue(el78.Candidates.SequenceEqual(new List<int>()));
        Assert.IsTrue(el88.Candidates.SequenceEqual(new List<int>()));
        el78.RefreshCandidates(grid.InitialElements);
        el88.RefreshCandidates(grid.InitialElements);
        Assert.IsTrue(el78.Candidates.SequenceEqual(new List<int>() { 4 }));
        Assert.IsTrue(el88.Candidates.SequenceEqual(new List<int>() { 4, 9 }));
    }

    [TestMethod]
    public void TestAffectsCandidatesForOthers_SameRow() {
        Element elThis = new(1, 1, 0);
        Element elThat = new(1, 6, 0);
       
        Assert.IsTrue(elThis.AffectsCandidatesForOther(elThat));
        Assert.IsTrue(elThat.AffectsCandidatesForOther(elThis));
    }

    [TestMethod]
    public void TestAffectsCandidatesForOthers_SameCol() {
        Element elThis = new(1, 1, 0);
        Element elThat = new(6, 1, 0);

        Assert.IsTrue(elThis.AffectsCandidatesForOther(elThat));
        Assert.IsTrue(elThat.AffectsCandidatesForOther(elThis));
    }

    [TestMethod]
    public void TestAffectsCandidatesForOthers_SameSubGrid() {
        Element elThis = new(1, 1, 0);
        Element elThat = new(2, 2, 0);

        Assert.IsTrue(elThis.AffectsCandidatesForOther(elThat));
        Assert.IsTrue(elThat.AffectsCandidatesForOther(elThis));
    }

    [TestMethod]
    public void TestAffectsCandidatesForOthers_False() {
        Element elThis = new(1, 1, 0);
        Element elThat = new(3, 3, 0);

        Assert.IsFalse(elThis.AffectsCandidatesForOther(elThat));
        Assert.IsFalse(elThat.AffectsCandidatesForOther(elThis));
    }

    [TestMethod]
    public void TestEquals_True() {
        Element elThis = new(1, 1, 0) { Candidates = new List<int>() { 1, 2 } };
        Element elThat = new(1, 1, 0) { Candidates = new List<int>() { 1, 2 } };

        Assert.AreEqual(elThis, elThat);
    }

    [TestMethod]
    public void TestEquals_DifferentCoords() {
        Element elThis = new(1, 1, 0) { Candidates = new List<int>() { 1, 2 } };
        Element elThat = new(1, 5, 0) { Candidates = new List<int>() { 1, 2 } };

        Assert.AreNotEqual(elThis, elThat);
    }

    [TestMethod]
    public void TestEquals_DifferentCandidates() {
        Element elThis = new(1, 1, 0) { Candidates = new List<int>() { 1, 2 } };
        Element elThat = new(1, 1, 0) { Candidates = new List<int>() { 1, 3 } };

        Assert.AreNotEqual(elThis, elThat);
    }
}

