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
        /// The command line options.
        /// </summary>
        private static Options options = new Options();

        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void Main(string[] args)
        {
            // Parse arguments
            if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                Environment.Exit(1);
            }

            
        }


        /// <summary>
        /// Creates the brainloller image.
        /// </summary>
        /// <param name="codebase">The codebase.</param>
        /// <param name="filename">The filename.</param>
        private void CreateBrainlollerImage(int width, string codebase, string filename)
        {
            // Initialize
            // Coordinate: X(row) Y(col) - (2,4)->2nd row, 4th column
            int height = 0;
            Direction currentDirection = Direction.Right;
            Coordinate c = new Coordinate() { X = 0, Y = 0 };

            // Set height
            if (((float)((float)codebase.Length / (float)(width - 2))) % 1 == 0)
            {
                height = codebase.Length / width;
            }
            else
            {
                height = codebase.Length / width + 1;
            }
            
            // Create bitmap and graphics object
            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    for (int i = 0; i < codebase.Length; ++i)
                    {
                        // Check if it's the last character. If so, just write it and done.
                        if (i == codebase.Length - 1)
                        {
                            // EOC
                            bmp.SetPixel(c.X, c.Y, CommandToColor(codebase[i]));
                            break;
                        }

                        switch (currentDirection)
                        {
                            case Direction.Up:
                                break;
                            case Direction.Down:
                                break;
                            case Direction.Left:
                                break;
                            case Direction.Right:
                                // Left -> Right
                                break;
                        }
                    }
                }
            }
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

        private static Color CommandToColor(char cmd)
        {
            switch (cmd)
            {
                case '+':
                    return Color.FromArgb(0, 255, 0);
                case '-':
                    return Color.FromArgb(0, 128, 0);
                case '>':
                    return Color.FromArgb(255, 0, 0);
                case '<':
                    return Color.FromArgb(128, 0, 0);
                case '.':
                    return Color.FromArgb(0, 0, 255);
                case ',':
                    return Color.FromArgb(0, 0, 128);
                case '[':
                    return Color.FromArgb(255, 255, 0);
                case ']':
                    return Color.FromArgb(128, 128, 0);
                case 'R':
                    return Color.FromArgb(0, 255, 255);
                case 'L':
                    return Color.FromArgb(0, 128, 128);
                default:
                    return Color.FromArgb(0, 0, 0);
            }
        }
    }
}
