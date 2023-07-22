namespace Sudoku.Models {
    // Solution recording solution steps.
    // Immutable after it is finalized by adding finalValues.
    public class Solution {
        private Stack<Step> steps = new();

        private int[,]? finalValues = null;

        public List<Step> Steps {
            get { return new List<Step>(steps); }
        }

        public int[,] FinalValues {
            get {
                if (finalValues == null) {
                    throw new InvalidOperationException("Tried to get finalValues for a Solution that was not yet finalized.");
                }
                return finalValues; }
            set {
                if (finalValues == null) {
                    // TODO: check if it's a valid solution - move existing methods here?
                    finalValues = value;
                }
            }
        }

        public void AddNewStep(Step step) {
            if (finalValues != null) {
                throw new InvalidOperationException("Tried to add a step to Solution that was already finalized.");
            }
            steps.Push(step);
        }

        public void RemoveLastStep() {
            if (finalValues != null) {
                throw new InvalidOperationException("Tried to remove a step from Solution that was already finalized.");
            }
            if (steps.Count > 0) {
                steps.Pop();
            }
        }

        public record Step (int Row, int Col, int Num);

    }
}
