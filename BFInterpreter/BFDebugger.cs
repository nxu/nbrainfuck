// <copyright file="BFDebugger.cs" company="nXu.hu">
//     Copyright nXu. Licensed under the MIT License.
// </copyright>
// <author>nXu</author>

namespace BrainfuckInterpreter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The debugger implementation of the interpretation engine.
    /// </summary>
    public class BFDebugger : BFInterpretationEngine
    {
        /// <summary>
        /// Brainfuck code to debug.
        /// </summary>
        private string codebase;

        /// <summary>
        /// Initializes a new instance of the <see cref="BFDebugger"/> class.
        /// </summary>
        /// <param name="memorySize">Count of the memory cells.</param>
        /// <param name="inputFunction">Function to call at input.</param>
        /// <param name="outputAction">Action to do at output.</param>
        /// <param name="codebase">Brainfuck code to debug.</param>
        public BFDebugger(int memorySize, Func<int> inputFunction, Action<int> outputAction, string codebase)
            : base(memorySize, inputFunction, outputAction)
        {
            // No code specified
            if (codebase == null || codebase.Length < 1)
            {
                throw new ArgumentNullException("Invalid code");
            }

            // Set code base
            this.codebase = codebase;

            // Set IP to -1
            this.instructionPointer = -1;

            // Initialize to the first valid command
            this.GetNextValidCommand();
        }

        /// <summary>
        /// Does the next step in debugging.
        /// </summary>
        /// <returns>A <c>JITExecutionResult</c> value containing the 
        /// result of the current step.</returns>
        public JITExecutionResult NextStep()
        {
            // Get current character
            char cur = this.codebase[this.instructionPointer];

            // Breakpoint
            if (cur == '#')
            {
                this.GetNextValidCommand();
                return JITExecutionResult.BreakpointHit;
            }

            // Do the action
            Action what = this.instructionSet[cur];
            try
            {
                what();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Check for end of code and get next valid command
            if (!this.GetNextValidCommand())
            {
                return JITExecutionResult.EndOfCodeReached;
            }

            return JITExecutionResult.Succesful;
        }

        /// <summary>
        /// Gets the next valid command and sets the instruction pointer.
        /// </summary>
        /// <returns><c>true</c> if a valid command was found,
        /// <c>false</c> if end of valid code.</returns>
        private bool GetNextValidCommand()
        {
            if (this.ipManuallySet)
            {
                this.ipManuallySet = false;
                return true;
            }

            this.instructionPointer++;

            while (this.instructionPointer < this.codebase.Length)
            {
                char cur = this.codebase[this.instructionPointer];
                if (this.instructionSet.ContainsKey(cur) 
                    || cur == '#')
                {
                    return true;
                }
                else
                {
                    this.instructionPointer++;
                }
            }

            return false;
        }
    }
}
