// <copyright file="Program.cs" company="nXu.hu">
//    Copyright (c) 2014 nXu - Released under the MIT License
// </copyright>
// <summary>
//   An extremely simple brainfuck - brainloller compiler.
// </summary>
namespace nbrainloller
{
    using System;
    using System.Drawing;
    using System.Text;

    /// <summary>
    /// An extremely simple brainfuck - brainloller compiler.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void Main(string[] args)
        {
        }


        /// <summary>
        /// Creates the brainloller image.
        /// </summary>
        /// <param name="codebase">The codebase.</param>
        /// <param name="filename">The filename.</param>
        private void CreateBrainlollerImage(string codebase, string filename)
        {

        }

        /// <summary>
        /// Cleans the code.
        /// </summary>
        /// <param name="codebase">The codebase.</param>
        /// <returns>The clean code containing only brainfuck operations.</returns>
        private string CleanCode(string codebase)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var character in codebase)
            {
                switch (character)
                {
                    case '>':
                    case '<':
                    case '.':
                    case ',':
                    case '[':
                    case ']':
                    case '-':
                    case '+':
                        sb.Append(character);
                        break;
                    default:
                        break;
                }
            }

            return sb.ToString();
        }
    }
}
