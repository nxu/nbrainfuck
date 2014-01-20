// <copyright file="BFInterpretationEngine.cs" company="nXu.hu">
//     Copyright nXu. Licensed under the MIT License.
// </copyright>
// <author>nXu</author>

namespace BrainfuckInterpreter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// A brainfuck interpretation engine.
    /// </summary>
    public abstract class BFInterpretationEngine
    {
        #region -- Fields --------------------------------------------------
        /// <summary>
        /// Application memory.
        /// </summary>
        protected int[] applicationMemory;

        /// <summary>
        /// Instruction pointer.
        /// </summary>
        protected int instructionPointer;

        /// <summary>
        /// The pointer (current memory index).
        /// </summary>
        protected int thePointer;

        /// <summary>
        /// Action to do at output (.).
        /// </summary>
        protected Action<int> outputAction;

        /// <summary>
        /// Function to call at input (,).
        /// </summary>
        protected Func<int> inputFunction;

        /// <summary>
        /// Instruction set.
        /// </summary>
        protected Dictionary<char, Action> instructionSet;

        /// <summary>
        /// Worker for the asynchronous execution.
        /// </summary>
        protected BackgroundWorker asyncExecutionWorker;

        /// <summary>
        /// Gets or sets a value indicating whether the instruction
        /// pointer has been manually set by the last operation.
        /// </summary>
        protected bool ipManuallySet;

        /// <summary>
        /// Gets or sets the positions of the loop begins and ends.
        /// </summary>
        protected Dictionary<int, int> loopPairs;

        #endregion

        #region -- Constructors --------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="BFInterpretationEngine"/> class.
        /// </summary>
        /// <param name="memorySize">Count of memory cells.</param>
        /// <param name="inputFunction">Function to call at input.</param>
        /// <param name="outputAction">Action to do at output.</param>
        /// <param name="codebase">Codebase to interpret, it will be only used to preprocess the code for the loops.</param>
        protected BFInterpretationEngine(int memorySize, Func<int> inputFunction, Action<int> outputAction, string codebase)
        {
            // IP set automatically
            this.ipManuallySet = false;

            // Initialize memory
            this.applicationMemory = new int[memorySize];
            for (int i = 0; i < memorySize; ++i)
            {
                this.applicationMemory[i] = 0;
            }

            // Initialize pointers
            this.instructionPointer = 0;
            this.thePointer = 0;

            // Initialize loop start and ends
            this.loopPairs = this.GetLoopPositions(codebase);

            // Initialize I/O
            this.inputFunction = inputFunction;
            this.outputAction = outputAction;

            // Create the instruction set
            this.instructionSet = new Dictionary<char, Action>();
            this.instructionSet.Add('+', new Action(this.Instruction_Add));
            this.instructionSet.Add('-', new Action(this.Instruction_Sub));
            this.instructionSet.Add('>', new Action(this.Instruction_Right));
            this.instructionSet.Add('<', new Action(this.Instruction_Left));
            this.instructionSet.Add('.', new Action(this.Instruction_Output));
            this.instructionSet.Add(',', new Action(this.Instruction_Input));
            this.instructionSet.Add('[', new Action(this.Instruction_BeginLoop));
            this.instructionSet.Add(']', new Action(this.Instruction_EndLoop));
        }
        #endregion

        #region -- Properties ----------------------------------------------
        /// <summary>
        /// Gets the application memory.
        /// </summary>
        public int[] ApplicationMemory
        {
            get
            {
                return this.applicationMemory;
            }

            private set
            {
                this.applicationMemory = value;
            }
        }

        /// <summary>
        /// Gets the instruction pointer.
        /// </summary>
        public int InstructionPointer
        {
            get
            {
                return this.instructionPointer;
            }

            private set
            {
                this.instructionPointer = value;
            }
        }

        /// <summary>
        /// Gets The Pointer.
        /// </summary>
        public int ThePointer
        {
            get
            {
                return this.thePointer;
            }

            private set
            {
                this.thePointer = value;
            }
        }
        #endregion

        #region -- Private / protected methods -----------------------------
        /// <summary>
        /// Instruction: +.
        /// </summary>
        private void Instruction_Add()
        {
            this.Helper_Instruction_AddOrSub(1);
        }

        /// <summary>
        /// Instruction: -.
        /// </summary>
        private void Instruction_Sub()
        {
            this.Helper_Instruction_AddOrSub(-1);
        }

        /// <summary>
        /// Instruction: &gt;.
        /// </summary>
        private void Instruction_Right()
        {
            this.Helper_Instruction_LeftOrRight(1);
        }

        /// <summary>
        /// Instruction: &lt;.
        /// </summary>
        private void Instruction_Left()
        {
            this.Helper_Instruction_LeftOrRight(-1);
        }

        /// <summary>
        /// Instruction: ,.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Input function is null.</exception>
        private void Instruction_Input()
        {
            if (this.inputFunction == null)
            {
                throw new InvalidOperationException("No input function found");
            }
            else
            {
                this.Helper_Instruction_SetMemoryValue(this.inputFunction());
            }
        }

        /// <summary>
        /// Instruction: dot.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Output action is null.</exception>
        private void Instruction_Output()
        {
            if (this.outputAction == null)
            {
                throw new InvalidOperationException("No output action found");
            }
            else
            {
                this.outputAction(this.Helper_Instruction_GetMemoryValue());
            }
        }

        /// <summary>
        /// Instruction: [.
        /// </summary>
        private void Instruction_BeginLoop()
        {
            if (this.Helper_Instruction_GetMemoryValue() == 0)
            {
                // Zero, skip the loop
                this.instructionPointer = this.loopPairs[this.instructionPointer];

                // Don't set the ipManuallySet flag, the next instruction is the instruction after
                // the loop end anyways, so 1 has to be added.
            }
            else
            {
                // It's nonzero, execute the loop.
                // Call stack was preprocessed, nothing left to do
                return;
            }
        }

        /// <summary>
        /// Instruction: ].
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Call stack is empty.</exception>
        private void Instruction_EndLoop()
        {
            if (this.Helper_Instruction_GetMemoryValue() != 0)
            {
                // Not zero, jump back to loop start
                this.instructionPointer = (from d in this.loopPairs
                                           where d.Value == this.instructionPointer
                                           select d.Key).SingleOrDefault();

                // Manually set = 1 is not added to the IP
                this.ipManuallySet = true;
            }
            else
            {
                // Zero, continue execution. 
                // Call stack was preprocessed, nothing left to do
                return;
            }
        }

        /// <summary>
        /// Adds or subtracts a value to/from the current memory cell.
        /// </summary>
        /// <param name="amount">Amount to add.</param>
        private void Helper_Instruction_AddOrSub(int amount)
        {
            if (this.thePointer >= this.applicationMemory.Length)
            {
                throw new IndexOutOfRangeException("Pointer points above memory limit");
            }
            else if (this.thePointer < 0)
            {
                throw new IndexOutOfRangeException("Pointer is negative");
            }
            else
            {
                this.applicationMemory[this.thePointer] += amount;
            }
        }

        /// <summary>
        /// Sets the current memory cell to a value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        private void Helper_Instruction_SetMemoryValue(int value)
        {
            if (this.thePointer >= this.applicationMemory.Length)
            {
                throw new IndexOutOfRangeException("Pointer points above memory limit");
            }
            else if (this.thePointer < 0)
            {
                throw new IndexOutOfRangeException("Pointer is negative");
            }
            else
            {
                this.applicationMemory[this.thePointer] = value;
            }
        }

        /// <summary>
        /// Gets the value of the memory at the current pointer.
        /// </summary>
        /// <returns>Value of the currently selected memory cell.</returns>
        /// <exception cref="System.IndexOutOfRangeException">Memory pointer is out of range.</exception>
        private int Helper_Instruction_GetMemoryValue()
        {
            if (this.thePointer >= this.applicationMemory.Length)
            {
                throw new IndexOutOfRangeException("Pointer points above memory limit");
            }
            else if (this.thePointer < 0)
            {
                throw new IndexOutOfRangeException("Pointer is negative");
            }
            else
            {
                return this.applicationMemory[this.thePointer];
            }
        }

        /// <summary>
        /// Increments or decrements the pointer.
        /// </summary>
        /// <param name="amount">Amount to add.</param>
        private void Helper_Instruction_LeftOrRight(int amount)
        {
            this.thePointer += amount;
        }

        /// <summary>
        /// Code preprocession to find the positions for each corresponding loop start and end.
        /// </summary>
        /// <param name="codebase">The codebase to process.</param>
        /// <returns>Loop start and end position pairs.</returns>
        private Dictionary<int, int> GetLoopPositions(string codebase)
        {
            var dict = new Dictionary<int, int>();
            var tmpStack = new Stack<int>();

            for (int i = 0; i < codebase.Length; ++i)
            {
                if (codebase[i] == '[')
                {
                    tmpStack.Push(i);
                }
                else if (codebase[i] == ']')
                {
                    if (tmpStack.Count == 0)
                    {
                        throw new InvalidOperationException("Start of loop not found. Position: " + i.ToString());
                    }
                    else
                    {
                        dict.Add(tmpStack.Pop(), i);
                    }
                }
            }
            
            if (tmpStack.Count > 0)
            {
                // Loop not closed
                throw new InvalidOperationException("Loop left open. Position: " + tmpStack.Pop().ToString());
            }

            return dict;
        }
        #endregion
    }
}
