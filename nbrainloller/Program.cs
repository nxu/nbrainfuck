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
    using System.IO;
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
            #region Initialize
            // Parse arguments
            if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                Environment.Exit(1);
            }

            // Check width
            if (!options.Decode)
            {
                if (options.ImageWidth < 3)
                {
                    Console.WriteLine(options.GetUsage());
                    Console.WriteLine("\nERROR: width must be at least 3.");
                    Environment.Exit(1);
                }
                else if (options.OutputFile == string.Empty || options.OutputFile == null)
                {
                    Console.WriteLine(options.GetUsage());
                    Console.WriteLine("\nERROR: output file must be specified.");
                }
            }

            // Check input filename
            if (!File.Exists(options.Filename))
            {
                Console.WriteLine(options.GetUsage());
                Console.WriteLine("\nERROR: Input file does not exist.");
                Environment.Exit(1);
            }
            #endregion

            if (options.Decode)
            {
                // Decode
                string code = DecodeBrainlollerImage(options.Filename);
                
                if (options.OutputFile != null && options.OutputFile != string.Empty)
                {
                    try
                    {
                        File.WriteAllText(options.OutputFile, code);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: could not write output file:\n" + ex.Message);
                        Environment.Exit(1);
                    }
                }
                else
                {
                    Console.WriteLine(code);
                }
            }
            else
            {
                // Encode
                string code = File.ReadAllText(options.Filename);
                code = CleanCode(code);
                CreateBrainlollerImage(options.ImageWidth, code, options.OutputFile);
                Environment.Exit(0);
            }

        }

        /// <summary>
        /// Creates the brainloller image.
        /// </summary>
        /// <param name="codebase">The codebase.</param>
        /// <param name="filename">The filename.</param>
        private static void CreateBrainlollerImage(int width, string codebase, string filename)
        {
            // Initialize
            // Coordinate: X(row) Y(col) - (2,4)->2nd row, 4th column
            double _height = (double)codebase.Length / (width - 2);
            int height = 0;
            if (_height == (int)_height)
            {
                height = (int)_height;
            }
            else
            {
                height = (int)_height + 1;
            }

            Direction currentDirection = Direction.Right;
            Coordinate c = new Coordinate() { X = 0, Y = 0 };
            
            // Create bitmap and graphics object
            using (Bitmap bmp = new Bitmap(width, height))
            {
                for (int i = 0; i < codebase.Length; ++i)
                {
                    // Check if it's the last character. If so, just write it and done.
                    if (i == codebase.Length - 1)
                    {
                        // EOC
                        bmp.SetPixel(c.Y, c.X, CommandToColor(codebase[i]));
                        break;
                    }

                    switch (currentDirection)
                    {
                        case Direction.Up:
                            break;
                        case Direction.Down:
                            if (c.Y == 0)
                            {
                                // First of the row, turn left (new direction is left -> right)
                                currentDirection = Direction.Right;
                                bmp.SetPixel(c.Y, c.X, CommandToColor('L'));
                                --i;
                                ++c.Y;
                            }
                            else if (c.Y == width - 1)
                            {
                                // Last of the row, turn right (new direction is left <- right)
                                currentDirection = Direction.Left;
                                bmp.SetPixel(c.Y, c.X, CommandToColor('R'));
                                --i;
                                --c.Y;
                            }
                            break;
                        case Direction.Left:
                            // Left <- Right
                            if (c.Y == 0)
                            {
                                // First of the row
                                currentDirection = Direction.Down;
                                bmp.SetPixel(c.Y, c.X, CommandToColor('L'));
                                --i;
                                ++c.X;
                            }
                            else 
                            {
                                // Not the first one
                                bmp.SetPixel(c.Y, c.X, CommandToColor(codebase[i]));
                                --c.Y;
                            }
                            break;
                        case Direction.Right:
                            // Left -> Right
                            if (c.Y == width - 1)
                            {
                                // Last of the row
                                currentDirection = Direction.Down;
                                bmp.SetPixel(c.Y, c.X, CommandToColor('R'));
                                --i;
                                ++c.X;
                            }
                            else 
                            {
                                // Not the last one
                                bmp.SetPixel(c.Y, c.X, CommandToColor(codebase[i]));
                                ++c.Y;
                            }
                            break;
                    }
                }

                // Save image
                try
                {
                    bmp.Save(options.OutputFile, System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not save the image file: \n" + ex.Message);
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Decodes the brainloller image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Bitmap to decode.</returns>
        private static string DecodeBrainlollerImage(string path)
        {
            #region Initialize bitmap
            Bitmap bmp = null;

            try 
            { 
                bmp = (Bitmap)Bitmap.FromFile(path); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: could not open input file:\n" + ex.Message);
                Environment.Exit(1);
            }
            #endregion

            Coordinate c = new Coordinate() { X = 0, Y = 0 };
            Direction currentDirection = Direction.Right;
            StringBuilder code = new StringBuilder();

            while (c.X >= 0 && c.Y >= 0 && c.X < bmp.Height && c.Y < bmp.Width)
            {
                char cmd = ColorToCommand(bmp.GetPixel(c.Y, c.X));

                if (cmd == 'R')
                {
                    // Turn right
                    switch (currentDirection)
                    {
                        case Direction.Up:
                            currentDirection = Direction.Right;
                            break;
                        case Direction.Down:
                            currentDirection = Direction.Left;
                            break;
                        case Direction.Left:
                            currentDirection = Direction.Up;
                            break;
                        case Direction.Right:
                            currentDirection = Direction.Down;
                            break;
                    }
                }
                else if (cmd == 'L')
                {
                    // Turn left
                    switch (currentDirection)
                    {
                        case Direction.Up:
                            currentDirection = Direction.Left;
                            break;
                        case Direction.Down:
                            currentDirection = Direction.Right;
                            break;
                        case Direction.Left:
                            currentDirection = Direction.Down;
                            break;
                        case Direction.Right:
                            currentDirection = Direction.Up;
                            break;
                    }
                }
                else if (cmd != 'X')
                {
                    code.Append(cmd);
                }

                // Step according to the current direction
                switch (currentDirection)
                {
                    case Direction.Up:
                        --c.X;
                        break;
                    case Direction.Down:
                        ++c.X;
                        break;
                    case Direction.Left:
                        --c.Y;
                        break;
                    case Direction.Right:
                        ++c.Y;
                        break;
                }
            }

            return code.ToString();
        }

        /// <summary>
        /// Cleans the code.
        /// </summary>
        /// <param name="codebase">The codebase.</param>
        /// <returns>The clean code containing only brainfuck operations.</returns>
        public static string CleanCode(string codebase)
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

        /// <summary>
        /// Converts a command to a color.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>Color of the command.</returns>
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

        /// <summary>
        /// Converts a braincopter color to command.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The command in brainfuck chars.</returns>
        private static char BraincopterColorToCommand(Color color)
        {
            int commandCode = 0;
            commandCode = -2 * color.R + 3 * color.G + color.B;
            commandCode = commandCode % 11;

            switch (commandCode)
            {
                case 0:
                    return '>';
                case 1:
                    return '<';
                case 2:
                    return '+';
                case 3:
                    return '-';
                case 4:
                    return '.';
                case 5:
                    return ',';
                case 6:
                    return '[';
                case 7:
                    return ']';
                case 8:
                    return 'R';
                case 9:
                    return 'L';
                default:
                    return 'N';
            }
        }

        /// <summary>
        /// Converts a color to command.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>Brainfuck command</returns>
        private static char ColorToCommand(Color color)
        {
            if (color == Color.FromArgb(255, 0, 0))
            {
                return '>';
            }
            else if (color == Color.FromArgb(128, 0, 0))
            {
                return '<';
            }
            else if (color == Color.FromArgb(0, 255, 0))
            {
                return '+';
            }
            else if (color == Color.FromArgb(0, 128, 0))
            {
                return '-';
            }
            else if (color == Color.FromArgb(0, 0, 255))
            {
                return '.';
            }
            else if (color == Color.FromArgb(0, 0, 128))
            {
                return ',';
            }
            else if (color == Color.FromArgb(0, 255, 255))
            {
                return 'R';
            }
            else if (color == Color.FromArgb(0, 128, 128))
            {
                return 'L';
            }
            else
            {
                return 'X';
            }
        }
    }
}
