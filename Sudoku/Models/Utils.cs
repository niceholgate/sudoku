using System.Diagnostics;

namespace Sudoku.Models {

    public static class Utils<T> {

        public static Random RANDOM_NUMBER_GENERATOR = new Random();

        /*
        * Writes to both Console and Debug.
        */
        public static void DualWrite(String str) {
            Console.Write(str);
            Debug.Write(str);
        }

        public static void DualWriteNewline(String str) {
            DualWrite(str + "\n");
        }

        public static void DualWriteNewline() {
            DualWrite("\n");
        }

        /*
        * Print a 2D multidimensional array to both Console and Debug.
        */
        public static void PrintGridValues(int[,] grid) {
            for (int i = 0; i < grid.GetLength(0); i++) {
                for (int j = 0; j < grid.GetLength(1); j++) {
                    DualWrite(grid[i, j] + " ");
                }
                DualWriteNewline();
            }
            DualWriteNewline();
        }

        /*
        * Check two 2D multidimensional arrays are the same.
        */
        public static bool CheckGridEquivalence(int[,] grid1, int[,] grid2) {
            if (grid1.GetLength(0) != grid2.GetLength(0) ||
                grid1.GetLength(1) != grid2.GetLength(1)) return false;

            for (int i = 0; i < grid1.GetLength(0); i++) {
                for (int j = 0; j < grid1.GetLength(1); j++) {
                    if (grid1[i, j] != grid2[i, j]) return false;
                }
            }

            return true;
        }

        public static T[] ShuffleToArray(IEnumerable<T> enumerable) {
            return enumerable.OrderBy(a => RANDOM_NUMBER_GENERATOR.Next()).ToArray();
        }

        public static T SelectRandomElement(IEnumerable<T> enumerable) {
            return enumerable.ElementAt(RANDOM_NUMBER_GENERATOR.Next(0, enumerable.Count<T>()));
        }

    }
}