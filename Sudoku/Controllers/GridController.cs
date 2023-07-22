using Microsoft.AspNetCore.Mvc;
using Sudoku.Models;

namespace Sudoku.Controllers;

[ApiController]
[Route("[controller]")]
public class GridController : ControllerBase
{
    private readonly ILogger<GridController> _logger;

    public GridController(ILogger<GridController> logger)
    {
        _logger = logger;
    }

    // TODO: caching of Grid objects to improve performance?
    // TODO: should have a service layer between Model and Controller
    [HttpGet]
    public string GetRandomUniqueSparse()
    {
        int[,] grid = Grid.GenerateRandomUniqueSparse().InitialValues;
        string gridString = ArrayToString(grid);
        _logger.Log(LogLevel.Information, gridString);
        return gridString;
    }

    // Get a hint according to the solver's order of operations
    // Will restructure Solutions to include order of operations data before implementing
    [HttpGet]
    public string GetHint() {
        int[,] grid = Grid.GenerateRandomUniqueSparse().InitialValues;
        string gridString = ArrayToString(grid);
        _logger.Log(LogLevel.Information, gridString);
        return gridString;
    }

    public static string ArrayToString(int[,] grid) {
        string str = "";
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                str += grid[i, j];
                if (!(i == grid.GetLength(0)-1 && j == grid.GetLength(1) - 1)) {
                    str += ",";
                }
            }
            str += "\n";
        }
        return str;
    }
}

