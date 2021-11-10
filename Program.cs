namespace PDPC_QuickSort {

    /**
     * Program.
     * Main.
     */
    class Program {

        // Main.
        static void Main(string[] args) {
            // Create Parallel Quick Sort and Run.
            new ParallelQuickSort("test_quicksort", 15, 23, 5).Run();
        }
    }
}
