namespace nbrainfuck
{
    using System;
    using System.IO;
    using System.Linq;
    using BrainfuckInterpreter;

    public static class Program
    {
        /// <summary>
        /// Input function.
        /// </summary>
        private static InputManager inputManager = null;

        /// <summary>
        /// Output action.
        /// </summary>
        private static Action<int> output = null;
        
        /// <summary>
        /// Input function.
        /// </summary>
        private static Func<int> input = null;

        /// <summary>
        /// Memory limit.
        /// </summary>
        private static int memoryLimit = 30000;

        /// <summary>
        /// Source code.
        /// </summary>
        private static string codebase = null;

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            // Set start parameters
            SetStartParameters(args);

            // Initialize output / input
            InitializeOutputAction();
            InitializeInputFunction();

            // Initialize interpreter
            BFInterpreter bfi = new BFInterpreter(memoryLimit, input, output, codebase);

            // Run
            bfi.Execute(codebase);
        }

        /// <summary>
        /// Initializes the output action.
        /// </summary>
        private static void InitializeOutputAction()
        {
            output = new Action<int>((int i) =>
                {
                    Console.Write((char)i);
                });
        }

        /// <summary>
        /// Initializes input function.
        /// </summary>
        private static void InitializeInputFunction()
        {
            if (inputManager == null)
            {
                input = new Func<int>(() =>
                    {
                        Console.Write("\nInput a character: ");
                        return (int)Console.ReadKey(false).KeyChar;
                    });
            }
            else
            {
                input = new Func<int>(() => inputManager.Read());
            }
        }

        /// <summary>
        /// Sets start parameters.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void SetStartParameters(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                Environment.Exit(2);
            }

            bool memSet = false;

            // Step through the arguments
            foreach (var arg in args)
            {
                if (arg.StartsWith("-it:") || arg.StartsWith("-ib:"))
                {
                    // Input method
                    if (inputManager != null)
                    {
                        Error("Multiple input method definiton: " + arg, 2);
                    }
                    if (arg.Length <= 4)
                    {
                        Error("Invalid parameter \"-it:\" - no file specified", 2);
                    }
                    else
                    {
                        Stream source = null;
                        try
                        {
                             source = File.OpenRead(arg.Substring(4, arg.Length - 4));
                        }
                        catch(Exception ex)
                        {
                            Error("Can't open file for input: " + ex.Message, 2);
                        }

                        if (arg.StartsWith("-it:"))
                        {
                            // Text mode
                            inputManager = new TextInputManager(source);
                        }
                        else
                        {
                            // Binary mode
                            inputManager = new BinaryInputManager(source);
                        }
                    }
                }
                else if (arg.StartsWith("-m:"))
                {
                    int mem = -1;

                    // Memory limit
                    if (memSet)
                    {
                        Error("Multiple memory limit definitions", 2);
                    }
                    else if (arg.Length <= 3)
                    {
                        Error("No memory limit specified", 2);
                    }
                    else if (!int.TryParse(arg.Substring(3, arg.Length - 3), out mem) || mem < 0)
                    {
                        Error("Invalid memory limit: " + arg.Substring(3, arg.Length - 3), 2);
                    }
                    else
                    {
                        memoryLimit = mem;
                        memSet = true;
                    }
                }
                else
                {
                    if (codebase != null)
                        Error("Multiple source files defined or invalid argument: " + arg, 2);

                    try
                    {
                        codebase = File.ReadAllText(arg);
                    }
                    catch 
                    {
                        Error("Error while opening source file: " + arg, 2);
                    }
                }
            }
        }

        /// <summary>
        /// Prints the usage to the screen.
        /// </summary>
        private static void PrintUsage()
        {
                             //--------------------------------------------------------------------------------
            Console.WriteLine("nbrainfuck - Command-line Brainfuck Interpreter");
            Console.WriteLine("(c) 2012 nXu - http://brainfuck.nXu.hu\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("   nbrainfuck [options] filename\n");
            Console.WriteLine("Options:");
            Console.WriteLine("   -it:filename   Uses the specified file as input source.");
            Console.WriteLine("                  Text mode: file will be read char-by-char, ");
            Console.WriteLine("                             therefore Unicode is supported\n");
            Console.WriteLine("   -ib:filename   Uses the specified file as input source.");
            Console.WriteLine("                  Binary mode: file will be read byte-by-byte.\n");
            Console.WriteLine("   -m:number      Sets memory limit to the given number.\n");
            Console.WriteLine("Defaults:");
            Console.WriteLine("   - Default memory limit is 30000");
            Console.WriteLine("   - Default input method is user input");
        }

        /// <summary>
        /// Shows an error and exits.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <param name="exitCode">Exit code.</param>
        private static void Error(string error, int exitCode)
        {
            Console.WriteLine("[ERROR] {0}", error);
            Environment.Exit(exitCode);
        }
    }
}
