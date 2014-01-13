namespace BFIDE
{
    using System.Windows.Input;
    
    /// <summary>
    /// Static class for commands.
    /// </summary>
    public static class CommandShortcuts
    {
        /// <summary>
        /// Static constructor for the command shortcuts.
        /// </summary>
        static CommandShortcuts()
        {
            CommandShortcuts.DebugNextStepCommand = new RoutedUICommand("Executes the next step of the debugged code.", "DebugNextStepCommand", typeof(MainWindow));
            CommandShortcuts.DebugNextStepCommand.InputGestures.Add(new KeyGesture(Key.F11));

            CommandShortcuts.ExecuteCommand = new RoutedUICommand("Executes the given brainfuck code", "ExecuteCommand", typeof(MainWindow));
            CommandShortcuts.ExecuteCommand.InputGestures.Add(new KeyGesture(Key.F5, ModifierKeys.Control));

            CommandShortcuts.StartDebugCommand = new RoutedUICommand("Starts the debugging of the given brainfuck code", "StartDebugCommand", typeof(MainWindow));
            CommandShortcuts.StartDebugCommand.InputGestures.Add(new KeyGesture(Key.F5));
        }

        /// <summary>
        /// Command for execution.
        /// </summary>
        public static RoutedUICommand ExecuteCommand { get; set; }

        /// <summary>
        /// Command for starting debug.
        /// </summary>
        public static RoutedUICommand StartDebugCommand { get; set; }

        /// <summary>
        /// Command for next step in debug.
        /// </summary>
        public static RoutedUICommand DebugNextStepCommand { get; set; }
    }
}
