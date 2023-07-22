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
        public static bool CheckArrayEquivalence(T[,] arr1, T[,] arr2) {
            if (arr1.GetLength(0) != arr2.GetLength(0) ||
                arr1.GetLength(1) != arr2.GetLength(1)) return false;

            for (int i = 0; i < arr1.GetLength(0); i++) {
                for (int j = 0; j < arr1.GetLength(1); j++) {
                    if (!arr1[i, j].Equals(arr2[i, j])) return false;
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

        public static IList<List<T>> Listify2DArray(T[,] array) {
            return Enumerable.Range(0, array.GetLength(0))
                    .Select(row => Enumerable.Range(0, array.GetLength(1))
                                .Select(col => array[row, col])
                                .ToList())
                    .ToList();
        }

        // TODO: maybe should change to lists-of-lists to avoid use of this
        /*
         * Convert a list-of-lists to a 2D array.
         * null if no data or if nested lists are of different lengths.
         */
        public static T[,]? Create2DArray(IList<List<T>> listOfLists) {
            T[,]? array = null;
            int len1 = listOfLists.Count;
            if (len1 > 0) {
                int len2 = listOfLists[0].Count;
                if (len2 > 0) { 
                    array = new T[len1, len2];
                    for (int i = 0; i < len1; i++) {
                        if (listOfLists[i].Count != len2) return null;
                        for (int j = 0; j < len2; j++) {
                            array[i, j] = listOfLists[i][j];
                        }
                    }
                }
            }
            return array;
        }

        public static IList<T> Shuffle(IEnumerable<T> enumerable) {
            return enumerable.OrderBy(a => RANDOM_NUMBER_GENERATOR.Next()).ToList();
        }

        public static T SelectRandomElement(IEnumerable<T> enumerable) {
            return enumerable.ElementAt(RANDOM_NUMBER_GENERATOR.Next(0, enumerable.Count<T>()));
        }

    }
}