using System;
using System.Threading;

namespace PDPC_QuickSort {

    /**
     * The Parallel Quick Sort.
     * Tis an Experiment.
     */
    public class ParallelQuickSort : Experiment<int[]> {

        // The Amount of Threads.
        private readonly int AmountOfThreads;

        // Default Constructor.
        public ParallelQuickSort(string Log, int XMin, int XMax, int NumExecutions) : base(Log, XMin, XMax, NumExecutions) {
            // Calculate Amount of Threads.
            AmountOfThreads = Environment.ProcessorCount;

            // Dirty Way to Find the Lowest Power of 2 value...
            while (!IsPowerOfX(AmountOfThreads, 2))
                AmountOfThreads -= 1;
        }

        // Checks if a Number is Power of X.
        private bool IsPowerOfX(int Number, int X) {
            // 0 isn't power of anything.
            if (Number == 0)
                return false;

            // Calculate Division.
            double Division = Math.Log(Number) / Math.Log(X);

            // If this is true, then, we are power of X.
            return (int) Math.Ceiling(Division) == (int) Math.Floor(Division);
        }

        // The Parameters for the Parallel QuickSort.
        protected class ParallelQuickSortParams {

            // The Array.
            public int[] Input;

            // The Low Value.
            public int Low;

            // The High Value.
            public int High;

            // The Level.
            public int Level;

            // Constructor.
            public ParallelQuickSortParams(ref int[] Input, int Low, int High, int Level) {
                // Set Data.
                this.Input = Input;
                this.Low = Low;
                this.High = High;
                this.Level = Level;
            }
        }

        // Actually Implements the Quicksort.
        private void Quicksort_Implementation(object Params) {
            // Cast Parameters.
            ParallelQuickSortParams ThisParams = Params as ParallelQuickSortParams;

            // Partition.
            int P = Partition(ref ThisParams.Input, ThisParams.Low, ThisParams.High);

            // Calculate Next Level.
            int NextLevel = ThisParams.Level + 1;

            // Check if Next Level is Parallel.
            bool NextIsParallel = (int) Math.Pow(2, NextLevel) <= AmountOfThreads;

            // Call Quicksort for the Left and Right Values.
            Thread Q1 = Quicksort(ref ThisParams.Input, ThisParams.Low, P, NextLevel, NextIsParallel);
            Thread Q2 = Quicksort(ref ThisParams.Input, P + 1, ThisParams.High, NextLevel, NextIsParallel);

            // Attempt to Join Both Threads.
            Q1?.Join();
            Q2?.Join();
        }

        // Runs Quicksort.
        private Thread Quicksort(ref int[] Input, int Low, int High, int Level, bool Parallel = true) {
            // Make sure Values are in bounds.
            if (Low < 0 || High < 0 || Low >= High)
                return null;

            // Create Parameters.
            ParallelQuickSortParams Params = new ParallelQuickSortParams(ref Input, Low, High, Level);

            // Check for Parallel.
            if (Parallel) {
                // Create new Thread with the Parameters.
                Thread QuicksortThread = new Thread(Quicksort_Implementation);

                // Start the Thread.
                QuicksortThread.Start(Params);

                // Return it.
                return QuicksortThread;
            }

            // It isn't Parallel, call the Implementation.
            Quicksort_Implementation(Params);

            // Return null.
            return null;
        }

        // Partitions the Array.
        private int Partition(ref int[] Input, int Low, int High) {
            // Get Pivot.
            int Pivot = Input[(int)Math.Floor((High + Low) / 2f)];

            // Get the Left and Right Indexes.
            int Left = Low - 1;
            int Right = High + 1;

            // Loop Forever... (not really)
            while (true) {
                // Increment Left until it is >= Pivot.
                do Left++; while (Input[Left] < Pivot);

                // Decrease Right until it is <= Pivot.
                do Right--; while (Input[Right] > Pivot);

                // If the Indices are Crossed, return.
                if (Left >= Right)
                    return Right;

                // Swap Left and Right Indices.
                SwapIndices(ref Input, Left, Right);
            }
        }

        // Generates a Random Input.
        public override int[] GenerateInput(int N) {
            // Create Result.
            int[] Result = new int[N];

            // Create new Random.
            Random Rand = new Random();

            // Generate Random Numbers.
            for (int i = 0; i < Result.Length; i++)
                Result[i] = Rand.Next();

            // Return it.
            return Result;
        }

        // Clones the Input for Each Iteration.
        public override int[] CloneInput(int[] Input) {
            // Create Result.
            int[] Result = new int[Input.Length];

            // Copy Input to Result.
            Input.CopyTo(Result, 0);

            // Return Result.
            return Result;
        }

        // Executes a Single Iteration.
        public override void ExecuteIteration(int[] Input) {
            // Call Quicksort, Parallel.
            Quicksort(ref Input, 0, Input.Length - 1, 0)?.Join();
        }

        // Swaps two indexes in the Array, using the Semaphore.
        private void SwapIndices(ref int[] Input, int X, int Y) {
            // Swap Values.
            (Input[X], Input[Y]) = (Input[Y], Input[X]);
        }
    }
}
