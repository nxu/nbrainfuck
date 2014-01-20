// <copyright file="Options.cs" company="nXu.hu">
//     (c) 2014 nXu - Licensed under the MIT License
// </copyright>
// <summary>
//     Command line options for the bfcompiler.
// </summary>
namespace  bfcompiler
{
    using System;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// Command line options for the bfcompiler.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the brainfuck source code to compile.
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "Input source code to compile.")]
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the output file.
        /// </summary>
        [Option('o', "output", Required = false, HelpText = "Path and name of the compiled assembly. Default: out.exe", DefaultValue = "out.exe")]
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the parser state.
        /// </summary>
        [ParserState]
        public IParserState LastParserState { get; set; }

        /// <summary>
        /// Gets or sets the usage information.
        /// </summary>
        /// <returns>Usage information.</returns>
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
