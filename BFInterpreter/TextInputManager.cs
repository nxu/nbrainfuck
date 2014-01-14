// <copyright file="TextInputManager.cs" company="nXu.hu">
//     Copyright nXu. Licensed under the MIT License.
// </copyright>
// <author>nXu</author>

namespace BrainfuckInterpreter
{
    using System;
    using System.IO;

    /// <summary>
    /// A text input manager class.
    /// </summary>
    public class TextInputManager : InputManager, IDisposable
    {
        /// <summary>
        /// The string to read.
        /// </summary>
        private string stringToRead;

        /// <summary>
        /// The stream to read.
        /// </summary>
        private Stream streamToRead;

        /// <summary>
        /// Stream reader for the stream.
        /// </summary>
        private TextReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputManager"/> class.
        /// </summary>
        /// <param name="source">String to read.</param>
        public TextInputManager(string source)
        {
            this.stringToRead = source;
            this.reader = new StringReader(this.stringToRead);

            this.streamToRead = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputManager"/> class.
        /// </summary>
        /// <param name="source">Stream to read.</param>
        public TextInputManager(Stream source)
        {
            this.streamToRead = source;
            this.reader = new StreamReader(this.streamToRead);

            this.stringToRead = null;
        }

        /// <summary>
        /// Reads the next integer from the source.
        /// </summary>
        /// <returns>Next read integer, 0 if end of input.</returns>
        public override int Read()
        {
            char[] c = new char[1];

            if (this.reader.Read(c, 0, c.Length) > 0)
            {
                return (int)c[0];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Disposes managed resources.
        /// </summary>
        public void Dispose()
        {
            if (this.reader != null)
            {
                this.reader.Dispose();
            }

            if (this.streamToRead != null)
            {
                this.streamToRead.Dispose();
            }
        }
    }
}
