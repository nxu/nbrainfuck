// <copyright file="JITExecutionResult.cs" company="nXu.hu">
//     Copyright nXu. Licensed under the MIT License.
// </copyright>
// <author>nXu</author>

namespace BrainfuckInterpreter
{
    /// <summary>
    /// Result of a JIT execution.
    /// </summary>
    public enum JITExecutionResult
    {
        /// <summary>
        /// Execution successful.
        /// </summary>
        Succesful,
        
        /// <summary>
        /// A breakpoint was hit.
        /// </summary>
        BreakpointHit,

        /// <summary>
        /// Execution has been cancelled.
        /// </summary>
        ExecutionCancelled,

        /// <summary>
        /// The pointer became negative.
        /// </summary>
        CompileError_PointerNegative,

        /// <summary>
        /// The pointer is out of range.
        /// </summary>
        CompileError_PointerOutOfMemoryRange,

        /// <summary>
        /// A loop has no end.
        /// </summary>
        CompileError_UnknownLoopEnd,

        /// <summary>
        /// There was an open loop left.
        /// </summary>
        CompileError_OpenLoopLeft,

        /// <summary>
        /// End of code was reached.
        /// </summary>
        EndOfCodeReached
    }
}
