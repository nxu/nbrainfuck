namespace BFIDE
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Xml;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using BrainfuckInterpreter;
    using Microsoft.Win32;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Brainfuck interpreter.
        /// </summary>
        private BFInterpreter bfi;

        /// <summary>
        /// Brainfuck debugger.
        /// </summary>
        private BFDebugger bfd;

        /// <summary>
        /// Filename of the currently opened source code file.
        /// </summary>
        private string currentFileName;

        /// <summary>
        /// Input file name.
        /// </summary>
        private string inputFileName;

        /// <summary>
        /// Input function.
        /// </summary>
        private InputManager inputManager;

        /// <summary>
        /// Starting index of the memory cells displayed.
        /// </summary>
        private int memoryFrom;

        /// <summary>
        /// Ending index of the memory cells displayed.
        /// </summary>
        private int memoryTo;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.InitializeHighlighting();
            this.InitializeCharMap();

            this.memoryFrom = 0;
            this.memoryTo = 20;
            this.currentFileName = string.Empty;
        }

        /// <summary>
        /// Initializes the highlighting of the code editor.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        private void InitializeHighlighting()
        {
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("BFIDE.bfsd.xshd"))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not find embedded resource");
                }
                using (XmlReader reader = new XmlTextReader(s))
                {
                    HighlightingManager manager = new HighlightingManager();
                    this.BEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, manager);
                }
            }
        }

        /// <summary>
        /// Initializes a new interpreter.
        /// </summary>
        /// <param name="memorySize">Size of the memory.</param>
        private void InitializeInterpreter(int memorySize)
        {
            // Output action
            Action<int> output = this.GetOutputMethod();

            // Input function
            Func<int> input = this.GetInputMethod();

            if (input == null)
                return;
            
            // Initialize interpreter object
            this.bfi = new BFInterpreter(memorySize, input, output, this.BEditor.Text);
        }

        /// <summary>
        /// Initializes character map.
        /// </summary>
        private void InitializeCharMap()
        {
            for (int i = 32; i < 128; ++i)
            {
                this.LBCharMap.Items.Add(string.Format("{0}:\t'{1}'", i, (char)i));
            }
        }

        /// <summary>
        /// Initializes the input manager.
        /// </summary>
        /// <returns>Brainfuck input function.</returns>
        private Func<int> GetInputMethod()
        {
            if (true == this.CBUseFileInput.IsChecked)
            {
                // Use file as input source
                if (this.inputFileName == null || this.inputFileName.Length < 1)
                {
                    // No file specified!
                    MessageBox.Show("No input file specified", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                try
                {
                    // Open file for read
                    Stream source = File.OpenRead(this.inputFileName);

                    if (this.CBInputFileMethod.SelectedIndex == 0)
                    {
                        // Binary mode
                        this.inputManager = new BinaryInputManager(source);
                    }
                    else
                    {
                        // Text mode
                        this.inputManager = new TextInputManager(source);
                    }
                }
                catch (Exception ex)
                {
                    // File I/O error
                    MessageBox.Show("Error while opening file:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.CBUseFileInput.IsChecked = false;
                    return null;
                }
            }
            else
            {
                // Use inputbox for input (text mode)
                this.inputManager = new TextInputManager(this.TBXInputBox.Text);
            }

            // Use input manager for input function
            return new Func<int>(() => this.inputManager.Read());
        }

        /// <summary>
        /// Gets the output method.
        /// </summary>
        /// <returns>An output method for BF Interpreters.</returns>
        private Action<int> GetOutputMethod()
        {
            return new Action<int>(delegate(int i)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    this.OutputBox.Text += (char)i;
                }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            });
        }

        /// <summary>
        /// Handles the AsyncExecutionFinished event of the Brainfuck Interpreter object.
        /// </summary>
        /// <param name="executionResult">Result of the execution.</param>
        private void BFI_AsyncExecutionFinished(JITExecutionResult executionResult)
        {
            MessageBox.Show("Execution finished: " + executionResult, "Finished");

            // GUI
            this.Dispatcher.Invoke((Action)delegate
            {
                this.RunWithoutDebug.IsEnabled = true;
                this.StopRunning.IsEnabled = false;
                this.ShowCommonElements_StopRun();
            });
        }

        /// <summary>
        /// Handles the click event of the StopRunning button.
        /// Stops the running interpretation.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void StopRunning_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.bfi.CancelAsyncExecution();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles the Executed event of the CmdNew control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void CmdNew_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.currentFileName = null;
            this.BEditor.Text = string.Empty;
        }

        /// <summary>
        /// Handles the Executed event of the Save control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void Save_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (this.currentFileName == string.Empty)
            {
                this.SaveAs_Executed(null, null);
            }
            else
            {
                this.SaveFileTo(this.currentFileName);
            }
        }

        /// <summary>
        /// Handles the CanExecute event of the Save control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs" /> instance containing the event data.</param>
        private void Save_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Handles the CanExecute event of the SaveAs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs" /> instance containing the event data.</param>
        private void SaveAs_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Handles the Executed event of the SaveAs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void SaveAs_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            // Initialize save file dialog
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save as...";
            sfd.Filter = "Brainfuck code file (*.b)|*.b|All files (*.*)|*.*";
            sfd.DefaultExt = ".b";
            sfd.AddExtension = true;
            sfd.CheckPathExists = true;

            if (true == sfd.ShowDialog())
            {
                this.SaveFileTo(sfd.FileName);
            }
        }

        /// <summary>
        /// Handles the Executed event of the Open control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void Open_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            // Initialize open file dialog
            OpenFileDialog ofd = new OpenFileDialog(); 
            ofd.Title = "Open...";
            ofd.Filter = "Brainfuck code file (*.b)|*.b|All files (*.*)|*.*";
            ofd.DefaultExt = ".b";
            ofd.CheckFileExists = true;

            if (true == ofd.ShowDialog())
            {
                this.OpenFileFrom(ofd.FileName);
            }
        }

        /// <summary>
        /// Handles the CanExecute event of the Open control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs" /> instance containing the event data.</param>
        private void Open_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Handles the Click event of the Quit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Handles the Click event of the Import control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Bitmap files (*.bmp, *.png)|*.bmp;*.png";
            ofd.ShowDialog();

            if (ofd.FileName != null && ofd.FileName != string.Empty)
            {
                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "nbrainloller.exe";
                psi.Arguments = string.Format("-d {0}", ofd.FileName);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;

                // Encode
                p.StartInfo = psi;
                p.Start();
                this.OutputBox.Text = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                if (p.ExitCode > 0)
                {
                    MessageBox.Show("There has been az error while decoding the image to brainfuck code. See the output box for details",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    this.BEditor.Text = this.OutputBox.Text;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Export control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentFileName == string.Empty)
            {
                MessageBox.Show("You must save the code first.", "Exporting to brainloller.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Portable Network Graphics (*.png)|*.png";
            sfd.ShowDialog();
            if (sfd.SafeFileName != null && sfd.SafeFileName != string.Empty)
            {
                int width = 0;
                width = nbrainloller.Program.CleanCode(this.BEditor.Text).Length;
                width = (int)Math.Sqrt(width);

                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "nbrainloller.exe";
                psi.Arguments = string.Format("-w {0} -o {1} {2}", width, sfd.FileName, this.currentFileName);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;

                // Encode
                p.StartInfo = psi;
                p.Start();
                this.OutputBox.Text = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (p.ExitCode > 0)
                {
                    MessageBox.Show("There was an error while exporting to brainloller. See the output window for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    Process.Start(sfd.FileName);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the StopDebug control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void StopDebug_Click(object sender, RoutedEventArgs e)
        {
            // Dereference the debugger object and let the GC collect it
            this.bfd = null;
            
            // GUI
            this.ShowCommonElements_StopRun();
        }

        /// <summary>
        /// Handles the Click event of the RunUntilBP control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RunUntilBP_Click(object sender, RoutedEventArgs e)
        {
            // Perform a step and save its result
            JITExecutionResult result = this.bfd.NextStep();

            while (result == JITExecutionResult.Succesful)
            {
                // Keep running until breakpoint / EOF hit
                result = this.bfd.NextStep();
            }

            if (result == JITExecutionResult.EndOfCodeReached)
            {
                // EOF reached
                this.StopDebug_Click(null, null);
            }
            else
            {
                // Breakpoint hit
                this.DisplayDebugInfo();
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the TBInstructionPointer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private void TBInstructionPointer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.BEditor.Text.Length > 0 && this.bfd.InstructionPointer < this.BEditor.Text.Length)
            {
                this.BEditor.CaretOffset = this.bfd.InstructionPointer;
                this.BEditor.Select(this.bfd.InstructionPointer, 1);
                this.BEditor.Focus();
            }
        }

        /// <summary>
        /// Handles the Executed event of the CmdNextStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void CmdNextStep_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.DoNextStep();
        }

        /// <summary>
        /// Handles the Executed event of the CmdExecute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void CmdExecute_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.StartExecute();
        }

        /// <summary>
        /// Handles the Executed event of the CmdStartDebug control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void CmdStartDebug_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.DoStartDebug();
        }

        /// <summary>
        /// Handles the Click event of the AboutMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the SelectInputFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SelectInputFile_Click(object sender, RoutedEventArgs e)
        {
            // Initialize open file dialog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select input file";
            ofd.CheckFileExists = true;

            if (true == ofd.ShowDialog())
            {
                this.inputFileName = ofd.FileName;
                this.TBXInputFileName.Text = ofd.FileName;
            }
        }

        /// <summary>
        /// Handles the Click event of the MemoryDebugInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MemoryDebugInfo_Click(object sender, RoutedEventArgs e)
        {
            string content = this.TBXMemoryCells.Text;

            string[] lim = content.Split(new char[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
            int from, to;

            if (lim.Length != 2 || !int.TryParse(lim[0], out from) || !int.TryParse(lim[1], out to))
            {
                MessageBox.Show("Please input limit in following format:\n 0-10", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                this.memoryFrom = from;
                this.memoryTo = to;
            }

            this.DisplayDebugInfo();
        }

        /// <summary>
        /// Saves the code to a file.
        /// </summary>
        /// <param name="filename">The file of the saved code.</param>
        private void SaveFileTo(string filename)
        {
            try
            {
                File.WriteAllText(filename, BEditor.Text);
                this.currentFileName = filename;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving code:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens a given file.
        /// </summary>
        /// <param name="filename">File to open.</param>
        private void OpenFileFrom(string filename)
        {
            try
            {
                BEditor.Text = File.ReadAllText(filename);
                this.currentFileName = filename;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while opening code:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Displays debug information.
        /// </summary>
        private void DisplayDebugInfo()
        {
            if (this.bfd == null)
                return;

            // Instruction and memory pointer
            this.TBInstructionPointer.Text = this.bfd.InstructionPointer.ToString();
            this.TBMemoryPointer.Text = this.bfd.ThePointer.ToString();
            
            // Memory cells
            this.DisplayMemoryCells();

            // Select next instruction
            this.TBInstructionPointer_MouseDown(null, null);
        }

        /// <summary>
        /// Hides common elements when starting BF program.
        /// </summary>
        private void HideCommonElements_StartRun()
        {
            // GUI initialization
            this.OutputBox.Text = string.Empty;
            this.BEditor.IsReadOnly = true;
            this.FileMenu.IsEnabled = false;
            this.EditMenu.IsEnabled = false;
            this.InputTab.IsEnabled = false;
            this.IOTab.SelectedIndex = 0;
        }

        /// <summary>
        /// Shows common elements when stopping BF program.
        /// </summary>
        private void ShowCommonElements_StopRun()
        {
            // GUI initialization
            this.BEditor.IsReadOnly = false;
            this.FileMenu.IsEnabled = true;
            this.EditMenu.IsEnabled = true;
            this.InterpreterMenu.IsEnabled = true;
            this.DebuggerMenu.IsEnabled = true;
            this.StartDebug.IsEnabled = true;
            this.StopDebug.IsEnabled = false;
            this.NextStep.IsEnabled = false;
            this.RunUntilBP.IsEnabled = false;
            this.Title = "NXU Brainfuck Developer";
            this.DebuggerTab.IsEnabled = false;
            this.RightTab.SelectedIndex = 0;
            this.InputTab.IsEnabled = true;

            // Dereference input manager
            this.inputManager = null;
        }

        /// <summary>
        /// Starts executing BF program.
        /// </summary>
        private void StartExecute()
        {
            if (this.RunWithoutDebug.IsEnabled)
            {
                // GUI
                this.HideCommonElements_StartRun();
                this.DebuggerMenu.IsEnabled = false;
                this.RunWithoutDebug.IsEnabled = false;
                this.StopRunning.IsEnabled = true;
                this.Title = "NXU Brainfuck Developer - Running";

                // Initialize interpreter
                this.InitializeInterpreter(30000);
                if (this.bfi == null)
                {
                    return;
                }

                // Set events / event handlers
                this.bfi.AsyncExecutionFinished += new BFInterpreter.AsyncExecutionFinishedEventHandler(BFI_AsyncExecutionFinished);

                // Execute code
                this.bfi.ExecuteAsync(this.BEditor.Text);

                // Focus output box - auto scroll
                this.OutputBox.Focus();
            }
        }

        /// <summary>
        /// Does the next debugging step.
        /// </summary>
        private void DoNextStep()
        {
            if (this.NextStep.IsEnabled)
            {
                if (this.bfd.NextStep() == JITExecutionResult.EndOfCodeReached)
                {
                    this.StopDebug_Click(null, null);
                }

                this.DisplayDebugInfo();
            }
        }

        /// <summary>
        /// Starts debugging.
        /// </summary>
        private void DoStartDebug()
        {
            if (this.StartDebug.IsEnabled)
            {
                // Return if no code
                if (this.BEditor.Text.Length < 1)
                    return;

                // Set GUI
                this.HideCommonElements_StartRun();
                this.InterpreterMenu.IsEnabled = false;
                this.StartDebug.IsEnabled = false;
                this.Title = "NXU Brainfuck Developer - Debugging";
                this.DebuggerTab.IsEnabled = true;
                this.RightTab.SelectedIndex = 1;

                // Output action
                Action<int> output = this.GetOutputMethod();

                // Input action
                Func<int> input = this.GetInputMethod();
                if (input == null)
                    return;

                // Initialize debugger
                this.bfd = new BFDebugger(30000, input, output, this.BEditor.Text);

                // Display debug infos
                this.DisplayDebugInfo();

                // Last GUI steps for debugging
                this.StopDebug.IsEnabled = true;
                this.NextStep.IsEnabled = true;
                this.RunUntilBP.IsEnabled = true;
            }
        }

        /// <summary>
        /// Displays selected memory cells.
        /// </summary>
        private void DisplayMemoryCells()
        {
            // Memory cells
            this.LBMemoryCells.Items.Clear();
            for (int i = this.memoryFrom; i < this.bfd.ApplicationMemory.Length && i <= this.memoryTo; ++i)
            {
                int it = this.bfd.ApplicationMemory[i];
                string character = it > 31 ? string.Format("\t'{0}'", (char)it) : string.Empty;

                TextBlock content = new TextBlock();
                content.Text = string.Format("{0}\t{1}{2}", i, this.bfd.ApplicationMemory[i], character);
                if (this.bfd.ThePointer == i)
                    content.FontWeight = FontWeights.Bold;
                this.LBMemoryCells.Items.Add(content);
            }

            // Limits
            this.TBXMemoryCells.Text = string.Format("{0}-{1}", this.memoryFrom, this.memoryTo);
        }
    }
}