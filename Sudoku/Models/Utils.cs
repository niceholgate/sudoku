using System.Collections.Immutable;
using System.Diagnostics;

namespace Sudoku.Models {

    public static class Utils<T> where T : IEquatable<T> {

        public static Random RANDOM_NUMBER_GENERATOR = new();

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
        public static void Print2DArray(T[,] values) {
            for (int i = 0; i < values.GetLength(0); i++) {
                for (int j = 0; j < values.GetLength(1); j++) {
                    DualWrite(values[i, j] + " ");
                }
                DualWriteNewline();
            }
        }

        /*
        * Check two 2D multidimensional arrays are the same.
        */
        public static bool CheckGridEquivalence(T[,] grid1, T[,] grid2) {
            if (grid1.GetLength(0) != grid2.GetLength(0) ||
                grid1.GetLength(1) != grid2.GetLength(1)) return false;

            for (int i = 0; i < grid1.GetLength(0); i++) {
                for (int j = 0; j < grid1.GetLength(1); j++) {
                    if (!grid1[i, j].Equals(grid2[i, j])) return false;
                }
            }

            return true;
        }

        public static IList<T> Flatten2DArray(T[,] array) {
            List<T> list = new();
            for (int row = 0; row < array.GetLength(0); row++) {
                for (int col = 0; col < array.GetLength(1); col++) {
                    list.Add(array[row, col]);
                }
            }
            return list;
        }

        public static IList<T> Shuffle(IEnumerable<T> enumerable) {
            return enumerable.OrderBy(a => RANDOM_NUMBER_GENERATOR.Next()).ToList();
        }

        public static T SelectRandomElement(IEnumerable<T> enumerable) {
            return enumerable.ElementAt(RANDOM_NUMBER_GENERATOR.Next(0, enumerable.Count<T>()));
        }

    }
}