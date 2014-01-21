// <copyright file="Options.cs" company="nXu.hu">
//     Copyright (c) 2014 nXu. Released under the MIT License.
// </copyright>
// <summary>Command line options</summary.>

namespace nbrainloller
{
    using System;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// Command line options class.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets a value indicating whether decode an image instead of encoding one.
        /// </summary>
        /// <value>
        ///   <c>true</c> if decode; otherwise, <c>false</c>.
        /// </value>
        [Option('d', "decode", 
            HelpText = "Decode a brainloller image to brainfuck code and print it to the output file (or stdout if none specified). ")]
        public bool Encode { get; set; }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        [Option('w', "width", 
            HelpText = "The width of the created image. Must be 1 or higher." )]
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets the output file.
        /// </summary>
        /// <value>
        /// The output file.
        /// </value>
        [Option('o', "option", 
            HelpText = "The name (and path) of the output file. ")]
        public int OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        [ValueOption(0)]
        public string Filename { get; set; }

        /// <summary>
        /// Gets the usage.
        /// </summary>
        /// <returns>Usage text.</returns>
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
