// <copyright file="Program.cs" company="nXu.hu">
//     (c) 2014 nXu - Licensed under the MIT License
// </copyright>
// <summary>
//     Main class of the bfcompiler.
// </summary>
namespace bfcompiler
{
    using System;
    using System.Reflection;
    using CommandLine;

    /// <summary>
    /// Main class of the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Options set by the command line arguments.
        /// </summary>
        private static Options options = new Options();

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void Main(string[] args)
        {
            #region Initialize
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Environment.Exit(1);
            }

            // Show version
            Console.WriteLine("nbfrainfuck " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\n");
            #endregion
        }

        private static void ShowErrorMessage(string message)
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = c;
        }
    }
}