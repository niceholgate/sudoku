using Microsoft.AspNetCore.Mvc;
using Sudoku.Models;

namespace Sudoku.Controllers;

[ApiController]
[Route("{controller}/{action}")]
public class GridController : ControllerBase
{
    private readonly ILogger<GridController> _logger;

    public GridController(ILogger<GridController> logger) {
        _logger = logger;
    }

    // TODO: caching of Grid objects to improve performance?
    // TODO: should have a service layer between Model and Controller
    // 
    // GET: /Grid/GetRandomUniqueSparse/ 
    [HttpGet]
    public IActionResult GetNewProblem() {
        int[,] newSudokuProblem = Grid.GenerateRandomUniqueSparse().InitialValues;
        return Ok(Utils<int>.Listify2DArray(newSudokuProblem));
    }

    // TODO: what if Solve takes too long? even some 17 element problems are slow to find a solution. maybe need to randomise search if taking a long time.

    // Get a hint according to the solver's order of operations
    // With a caching system, could return multiple hints in a row without re-solving a Grid
    // Will restructure Solutions to include order of operations data before implementing
    [HttpPost]
    public IActionResult GetHint(List<List<int>> currentGrid) {
        // TODO: validate the input - and tidy exception logic
        //(int row, int col, int num)? hint = null;
        int[,]? initialValuesArray = Utils<int>.Create2DArray(currentGrid);
        try {
            if (initialValuesArray != null) {
                Grid grid = new(initialValuesArray);
                grid.Solve(1);
                if (grid.Solutions.Count > 0) {
                    List<Solution.Step> steps = grid.Solutions.ElementAt(0).Steps;
                    if (steps.Count > 0) {
                        return Ok(steps.First());
                    }
                }
            }
        } catch (ArgumentException e) {
            return BadRequest(e.Message);
        }

        return BadRequest("Could not get a hint");
    }

    //public static string ArrayToString(int[,] grid) {
    //    string str = "";
    //    for (int i = 0; i < grid.GetLength(0); i++) {
    //        for (int j = 0; j < grid.GetLength(1); j++) {
    //            str += grid[i, j];
    //            if (!(i == grid.GetLength(0)-1 && j == grid.GetLength(1) - 1)) {
    //                str += ",";
    //            }
    //        }
    //        str += "\n";
    //    }
    //    return str;
    //}


}

