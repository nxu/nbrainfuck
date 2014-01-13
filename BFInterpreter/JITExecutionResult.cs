namespace BrainfuckInterpreter
{
    /// <summary>
    /// Result of a JIT execution.
    /// </summary>
    public enum JITExecutionResult
    {
        Succesful,
        BreakpointHit,
        ExecutionCancelled,
        CompileError_PointerNegative,
        CompileError_PointerOutOfMemoryRange,
        CompileError_UnknownLoopEnd,
        CompileError_OpenLoopLeft,
        EndOfCodeReached
    }
}
