// <copyright file="BFInterpreter.cs" company="nXu.hu">
//     Copyright nXu. Licensed under the MIT License.
// </copyright>
// <author>nXu</author>

namespace BrainfuckInterpreter
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// The implementation of the interpretation engine.
    /// </summary>
    public class BFInterpreter : BFInterpretationEngine
    {
        #region -- Constructors ---------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="BFInterpreter"/> class.
        /// </summary>
        /// <param name="memorySize">The size of the memory.</param>
        /// <param name="inputFunction">Function to call at input.</param>
        /// <param name="outputAction">Action to do at output.</param>
        /// <param name="codebase">The codebase to interpret.</param>
        public BFInterpreter(int memorySize, Func<int> inputFunction, Action<int> outputAction, string codebase) 
            : base(memorySize, inputFunction, outputAction, codebase)
        { 
        }
        #endregion

        #region -- Events and delegates -------------------------------------
        /// <summary>
        /// Event handler for the AsyncExecutionFinished event.
        /// </summary>
        /// <param name="executionResult">The result o the execution.</param>
        public delegate void AsyncExecutionFinishedEventHandler(JITExecutionResult executionResult);

        /// <summary>
        /// Event occurring when async execution finishes.
        /// </summary>
        public event AsyncExecutionFinishedEventHandler AsyncExecutionFinished;
        #endregion

        #region -- Public methods -------------------------------------------
        /// <summary>
        /// Interprets and executes a given brainfuck code in a 
        /// synchronous (blocking) way. Ignores breakpoints.
        /// </summary>
        /// <param name="codebase">Brainfuck code to parse.</param>
        /// <returns>The result of the execution</returns>
        public JITExecutionResult Execute(string codebase)
        {
            while (this.instructionPointer < codebase.Length)
            {
                // For cleaner code, check worker cancellation here
                // instead of the loop declaration
                if (this.asyncExecutionWorker != null && this.asyncExecutionWorker.CancellationPending)
                {
                    return JITExecutionResult.ExecutionCancelled;
                }

                // Parse next char
                char cur = codebase[instructionPointer];
                Action what;

                // Get the action and try to do it
                if (this.instructionSet.TryGetValue(cur, out what))
                {
                    try
                    {
                        what();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                if (!this.ipManuallySet)
                {
                    this.instructionPointer++;
                }
                else
                {
                    this.ipManuallySet = false;
                }
            }

            return JITExecutionResult.Succesful;
        }

        /// <summary>
        /// Interprets a given brainfuck code and executes it
        /// in a separate thread. Fires <see cref="AsyncExecutionFinished"/>
        /// event when done. Ignores breakpoints.
        /// </summary>
        /// <param name="codebase">Brainfuck code to parse.</param>
        public void ExecuteAsync(string codebase)
        {
            // Initialize a background worker
            this.asyncExecutionWorker = new BackgroundWorker();
            this.asyncExecutionWorker.WorkerSupportsCancellation = true;

            // Execution
            this.asyncExecutionWorker.DoWork += new DoWorkEventHandler((object sender, DoWorkEventArgs e) =>
                {
                    e.Result = this.Execute(e.Argument as string);
                });

            // Fire finished event when finished
            this.asyncExecutionWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((object sender, RunWorkerCompletedEventArgs e) =>
                {
                    this.OnAsyncExecutionFinished((JITExecutionResult)e.Result);
                });

            // Start working
            this.asyncExecutionWorker.RunWorkerAsync(codebase);
        }

        /// <summary>
        /// Cancels a running asynchronous execution.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when there's no execution in progress.
        /// </exception>
        public void CancelAsyncExecution()
        {
            // Check if execution is in progress
            if (this.asyncExecutionWorker == null)
            {
                throw new InvalidOperationException("No execution in progress.");
            }

            // Cancel execution
            this.asyncExecutionWorker.CancelAsync();
        }
        #endregion

        #region -- Private methods ------------------------------------------
        /// <summary>
        /// Fires the AsyncExecutionFinished event and dereferences
        /// the background worker.
        /// </summary>
        /// <param name="result">The result of the execution.</param>
        private void OnAsyncExecutionFinished(JITExecutionResult result)
        {
            // Dispose and dereference background worker
            this.asyncExecutionWorker.Dispose();
            this.asyncExecutionWorker = null;

            // Fire finished event
            if (this.AsyncExecutionFinished != null)
            {
                this.AsyncExecutionFinished(result);
            }
        }
        #endregion
    }
}
