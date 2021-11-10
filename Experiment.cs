using System;
using System.IO;

namespace PDPC_QuickSort {

    /**
     * The Experiment Class.
     * Executes Several Times.
     */
    public abstract class Experiment<T> {

        // The Log File Name.
        public string Log { get; protected set; }

        // The Minimum Loop Value.
        public int XMin { get; protected set; }

        // The Maximum Loop Value.
        public int XMax { get; protected set; }

        // The Amount of Iterations for Each Test.
        public int NumExecutions { get; protected set; }

        // Constructor.
        protected Experiment(string Log, int XMin, int XMax, int NumExecutions) {
            // Set Data.
            this.Log = Log.EndsWith(".csv") ? Log : $"{Log}.csv";
            this.XMin = XMin;
            this.XMax = XMax;
            this.NumExecutions = NumExecutions;
        }

        // Runs the Experiment.
        public void Run() {
            // If the Log File Exists, delete it.
            if (File.Exists(Log))
                File.Delete(Log);

            // Open the Log File.
            StreamWriter LogWriter = new StreamWriter(File.OpenWrite(Log)) { AutoFlush = true };
            
            // Write Header.
            LogWriter.Write("x,N,Mean Execution Time");

            // Get the Start Date Time.
            DateTime Start = DateTime.Now;

            // External Loop.
            for (int x = XMin; x <= XMax; x++) {
                // Calculate N.
                int n = (int) Math.Pow(2, x);

                // Notify.
                Console.WriteLine($"Experiment for x = {x}, n = {n}");

                // Generate Input.
                T Input = GenerateInput(n);

                // Max Execution Time.
                double ExecutionTime = 0;

                // For each Execution.
                for (int e = 0; e < NumExecutions; e++) {
                    // Notify.
                    Console.WriteLine($"=> x = {x}: Iteration {e + 1}...");

                    // Create a Copy of the Input for this Iteration.
                    T InputCopy = CloneInput(Input);

                    // Get the Iteration Start Time.
                    DateTime IterationStart = DateTime.Now;

                    // Run the Iteration.
                    ExecuteIteration(InputCopy);

                    // Add to Execution Time.
                    ExecutionTime += (DateTime.Now - IterationStart).TotalMilliseconds;
                }

                // Calculate Mean Time.
                ExecutionTime /= NumExecutions;

                // Write Values to Log.
                LogWriter.Write($"\n{x},{n},{ExecutionTime}");
            }

            // Notify Finish.
            Console.WriteLine($"Experiment finished in {(DateTime.Now - Start).TotalMilliseconds} ms");

            // Close the Log File.
            LogWriter.Close();
        }

        // Generates the Input for the Iteration.
        public abstract T GenerateInput(int N);

        // Clones the Input.
        public abstract T CloneInput(T Input);

        // Executes a Single Iteration.
        public abstract void ExecuteIteration(T Input);
    }
}
